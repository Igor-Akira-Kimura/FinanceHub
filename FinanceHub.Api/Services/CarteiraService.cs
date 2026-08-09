using FinanceHub.Api.Application.Requests.Carteiras;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;
using FluentValidation;

namespace FinanceHub.Api.Services
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

        public CarteiraService(ICarteiraRepository carteiraRepository, IUsuarioRepository usuarioRepository, IAtivoRepository ativoRepository, IPosicaoRepository posicaoRepository, IMovimentacaoRepository movimentacaoRepository, IValidator<ComprarAtivoRequest> comprarAtivoRequestValidator, IValidator<VenderAtivoRequest> venderAtivoRequestValidator)
        {
            _carteiraRepository = carteiraRepository;
            _usuarioRepository = usuarioRepository;
            _ativoRepository = ativoRepository;
            _posicaoRepository = posicaoRepository;
            _movimentacaoRepository = movimentacaoRepository;
            _comprarValidator = comprarAtivoRequestValidator;
            _venderValidator = venderAtivoRequestValidator;
        }

        public async Task<Guid> CriarAsync(CriarCarteiraRequest request)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(request.UsuarioId);
            if (usuario is null)
                throw new UsuarioNaoEncontradoException(request.UsuarioId);

            if (!usuario.Ativo)
                throw new UsuarioInativoException(request.UsuarioId);

            var carteiraExistente = await _carteiraRepository.BuscarPorNomeAsync(request.UsuarioId, request.Nome);
            if (carteiraExistente != null)
                throw new CarteiraJaCadastradaException(request.Nome);

            var novaCarteira = new Carteira(request.Nome, request.UsuarioId);

            await _carteiraRepository.CriarAsync(novaCarteira);

            await _carteiraRepository.SalvarAlteracoesAsync();

            return novaCarteira.Id;
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

        public async Task<CarteiraResponse> BuscarPorIdAsync(Guid id)
        {
            var carteira = await _carteiraRepository.BuscarPorIdAsync(id);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(id);

            return new CarteiraResponse
            {
                Id = carteira.Id,
                Nome = carteira.Nome
            };
        }

        public async Task ComprarAtivoAsync(ComprarAtivoRequest request)
        {
            await _comprarValidator.ValidateAndThrowAsync(request);

            var carteira = await _carteiraRepository
                .BuscarPorIdAsync(request.CarteiraId);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(request.CarteiraId);

            var ativo = await _ativoRepository
                .BuscarPorIdAsync(request.AtivoId);

            if (ativo is null)
                throw new AtivoNaoEncontradoException(request.AtivoId);

            var posicao = await _posicaoRepository
                .BuscarPorCarteiraEAtivoAsync(
                    request.CarteiraId,
                    request.AtivoId);

            Movimentacao movimentacao;

            if (posicao is null)
            {
                posicao = new Posicao(
                    request.CarteiraId,
                    request.AtivoId,
                    request.Quantidade,
                    request.Preco);

                movimentacao = Movimentacao.CriarCompra(
                    posicao.Id,
                    request.Quantidade,
                    request.Preco);

                await _posicaoRepository.CriarAsync(posicao);
            }
            else
            {
                movimentacao = posicao.Comprar(
                    request.Quantidade,
                    request.Preco);
            }

            await _movimentacaoRepository.CriarAsync(movimentacao);

            await _movimentacaoRepository.SalvarAlteracoesAsync();
        }

        public async Task VenderAtivoAsync(VenderAtivoRequest request)
        {
            await _venderValidator.ValidateAndThrowAsync(request);

            var carteira = await _carteiraRepository.BuscarPorIdAsync(request.CarteiraId);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(request.CarteiraId);

            var posicao = await _posicaoRepository.BuscarPorCarteiraEAtivoAsync(
                request.CarteiraId,
                request.AtivoId);

            if (posicao is null)
                throw new PosicaoNaoEncontradaException(request.AtivoId);

            var movimentacao = posicao.Vender(
                request.Quantidade,
                request.Preco);

            await _movimentacaoRepository.CriarAsync(movimentacao);

            await _movimentacaoRepository.SalvarAlteracoesAsync();
        }
    }
}
