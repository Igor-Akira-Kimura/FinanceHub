using FinanceHub.Application.Cache;
using FinanceHub.Application.Common;
using FinanceHub.Application.Common.Events;
using FinanceHub.Application.Common.Outbox;
using FinanceHub.Application.Interfaces.Cache;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Application.Responses;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FinanceHub.Application.Services
{
    public class CarteiraService : ICarteiraService
    {
        private readonly ICarteiraRepository _carteiraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAtivoRepository _ativoRepository;
        private readonly IPosicaoRepository _posicaoRepository;
        private readonly IMovimentacaoRepository _movimentacaoRepository;
        private readonly IValidator<ComprarAtivoRequest> _comprarValidator;
        private readonly IValidator<VenderAtivoRequest> _venderValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompraRepository _compraRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IValidator<CriarCarteiraRequest> _criarCarteiraValidator;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CarteiraService> _logger;
        public CarteiraService(ICarteiraRepository carteiraRepository, IUsuarioRepository usuarioRepository, IAtivoRepository ativoRepository, IPosicaoRepository posicaoRepository, IMovimentacaoRepository movimentacaoRepository, IValidator<ComprarAtivoRequest> comprarAtivoRequestValidator, IValidator<VenderAtivoRequest> venderAtivoRequestValidator, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ICompraRepository compraRepository, IOutboxRepository outboxRepository, IValidator<CriarCarteiraRequest> criarCarteiraRequestValidator, ICacheService cacheService, ILogger<CarteiraService> logger)
        {
            _carteiraRepository = carteiraRepository;
            _usuarioRepository = usuarioRepository;
            _ativoRepository = ativoRepository;
            _posicaoRepository = posicaoRepository;
            _movimentacaoRepository = movimentacaoRepository;
            _comprarValidator = comprarAtivoRequestValidator;
            _venderValidator = venderAtivoRequestValidator;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _compraRepository = compraRepository;
            _outboxRepository = outboxRepository;
            _criarCarteiraValidator = criarCarteiraRequestValidator;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Guid> CriarAsync(CriarCarteiraRequest request)
        {
            await _criarCarteiraValidator.ValidateAndThrowAsync(request);

            var usuario = await _usuarioRepository.BuscarPorIdAsync(_currentUserService.Usuario.Id);
            if (usuario is null)
                throw new UsuarioNaoEncontradoException(_currentUserService.Usuario.Id);

            if (!usuario.Ativo)
                throw new UsuarioInativoException(_currentUserService.Usuario.Id);

            var carteiraExistente = await _carteiraRepository.BuscarPorNomeAsync(_currentUserService.Usuario.Id, request.Nome);
            if (carteiraExistente != null)
                throw new CarteiraJaCadastradaException(request.Nome);

            var novaCarteira = new Carteira(request.Nome, _currentUserService.Usuario.Id);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _carteiraRepository.CriarAsync(novaCarteira);

                await _unitOfWork.CommitAsync();

                return novaCarteira.Id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CarteiraResponse>> BuscarTodasAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioId);

            if (usuario is null)
                throw new UsuarioNaoEncontradoException(usuarioId);

            if (!usuario.Ativo)
                throw new UsuarioInativoException(usuarioId);

            var carteiras = await _carteiraRepository.BuscarTodasAsync(usuarioId);

            return carteiras.Select(c => new CarteiraResponse
            {
                Id = c.Id,
                Nome = c.Nome
            });
        }

        public async Task<IEnumerable<CarteiraResponse>> BuscarMinhasAsync()
        {
            return await BuscarTodasAsync(_currentUserService.Usuario.Id);
        }

        public async Task<CarteiraResponse> BuscarPorIdAsync(Guid id)
        {
            var carteira = await _carteiraRepository.BuscarPorIdAsync(id);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(id);

            if (carteira.UsuarioId != _currentUserService.Usuario.Id)
                throw new CarteiraNaoPertenceAoUsuarioException(id);

            return new CarteiraResponse
            {
                Id = carteira.Id,
                Nome = carteira.Nome
            };
        }

        public async Task ComprarAtivoAsync(ComprarAtivoRequest request)
        {
            _logger.LogInformation(
                "Iniciando compra. CarteiraId: {CarteiraId}, AtivoId: {AtivoId}, Quantidade: {Quantidade}",
                request.CarteiraId,
                request.AtivoId,
                request.Quantidade);

            await _comprarValidator
                .ValidateAndThrowAsync(request);

            var carteira = await _carteiraRepository
                .BuscarPorIdAsync(request.CarteiraId);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(
                    request.CarteiraId);

            _logger.LogDebug(
                "Carteira encontrada. CarteiraId: {CarteiraId}",
                carteira.Id);

            if (carteira.UsuarioId != _currentUserService.Usuario.Id)
                throw new CarteiraNaoPertenceAoUsuarioException(
                    request.CarteiraId);

            var cacheKey = $"ativo-preco:{request.AtivoId}";

            var cachedPreco =
                await _cacheService.GetAsync(cacheKey);

            decimal preco;

            if (cachedPreco is not null)
            {
                _logger.LogDebug(
                    "Cache hit. CacheKey: {CacheKey}",
                    cacheKey);

                preco =
                    JsonSerializer
                        .Deserialize<AtivoPrecoCache>(
                            cachedPreco)!
                        .Preco;
            }
            else
            {
                _logger.LogDebug(
                    "Cache miss. CacheKey: {CacheKey}",
                    cacheKey);

                var ativo =
                    await _ativoRepository
                        .BuscarPorIdAsync(request.AtivoId);

                if (ativo is null)
                    throw new AtivoNaoEncontradoException(
                        request.AtivoId);

                _logger.LogDebug(
                    "Ativo encontrado. AtivoId: {AtivoId}",
                    ativo.Id);

                preco = ativo.Preco;

                var cache =
                    new AtivoPrecoCache
                    {
                        Preco = preco
                    };

                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(cache),
                    TimeSpan.FromMinutes(5));
            }

            var posicao = await _posicaoRepository
                .BuscarPorCarteiraEAtivoAsync(
                    request.CarteiraId,
                    request.AtivoId);

            var valorTotal =
                request.Quantidade * preco;

            var compraExistente =
                await _compraRepository
                    .BuscarPorIdempotencyKeyAsync(
                        request.IdempotencyKey);

            if (compraExistente is not null)
                return;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var linhasAfetadas =
                    await _carteiraRepository
                        .DebitarSaldoAsync(
                            request.CarteiraId,
                            valorTotal);

                if (linhasAfetadas == 0)
                    throw new SaldoInsuficienteException(
                        request.CarteiraId,
                        valorTotal);

                Movimentacao movimentacao;

                if (posicao is null)
                {
                    posicao = new Posicao(
                        request.CarteiraId,
                        request.AtivoId,
                        request.Quantidade,
                        preco);

                    movimentacao = Movimentacao.CriarCompra(
                        posicao.Id,
                        request.Quantidade,
                        preco);

                    await _posicaoRepository
                        .CriarAsync(posicao);
                }
                else
                {
                    movimentacao = posicao.Comprar(
                        request.Quantidade,
                        preco);
                }

                await _movimentacaoRepository
                    .CriarAsync(movimentacao);

                var compra = new Compra(
                    request.CarteiraId,
                    request.AtivoId,
                    request.Quantidade,
                    preco,
                    request.IdempotencyKey);

                await _compraRepository
                    .CriarAsync(compra);

                var evento = new CompraCriadaEvent(
                    compra.Id,
                    compra.CarteiraId,
                    compra.AtivoId,
                    compra.Quantidade,
                    compra.Preco);

                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = nameof(CompraCriadaEvent),
                    Payload = JsonSerializer.Serialize(evento),
                    CreatedAt = DateTime.UtcNow
                };

                await _outboxRepository
                    .CriarAsync(outboxMessage);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation(
                    "Compra realizada. CarteiraId: {CarteiraId}, AtivoId: {AtivoId}, Quantidade: {Quantidade}",
                    request.CarteiraId,
                    request.AtivoId,
                    request.Quantidade);
            }
            catch (IdempotencyKeyJaProcessadaException)
            {
                await _unitOfWork.RollbackAsync();
                return;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task VenderAtivoAsync(VenderAtivoRequest request)
        {
            await _venderValidator.ValidateAndThrowAsync(request);

            var carteira = await _carteiraRepository.BuscarPorIdAsync(request.CarteiraId);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(request.CarteiraId);

            if (carteira.UsuarioId != _currentUserService.Usuario.Id)
                throw new CarteiraNaoPertenceAoUsuarioException(
                    request.CarteiraId);

            var ativo = await _ativoRepository.BuscarPorIdAsync(request.AtivoId);

            if (ativo is null)
                throw new AtivoNaoEncontradoException(
                    request.AtivoId);

            var posicao = await _posicaoRepository.BuscarPorCarteiraEAtivoAsync(
                request.CarteiraId,
                request.AtivoId);

            if (posicao is null)
                throw new PosicaoNaoEncontradaException(request.AtivoId);

            var valorTotal = request.Quantidade * ativo.Preco;

            var movimentacao = posicao.Vender(
                request.Quantidade,
                ativo.Preco);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _movimentacaoRepository.CriarAsync(movimentacao);

                var linhasAfetadas =
                    await _carteiraRepository
                        .CreditarSaldoAsync(
                            request.CarteiraId,
                            valorTotal);

                if (linhasAfetadas == 0)
                    throw new CarteiraNaoEncontradaException(
                        request.CarteiraId);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
