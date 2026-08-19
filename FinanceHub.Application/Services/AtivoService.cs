using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Responses;
using FinanceHub.Application.Validators;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentValidation;

namespace FinanceHub.Application.Services
{
    public class AtivoService : IAtivoService
    {
        private readonly IAtivoRepository _ativoRepository;
        private readonly IBolsaRepository _bolsaRepository;
        private readonly IValidator<CriarAtivoRequest> _criarAtivoRequestValidator;

        public AtivoService(IAtivoRepository ativoRepository, IBolsaRepository bolsaRepository, IValidator<CriarAtivoRequest> criarAtivoRequestValidator)
        {
            _ativoRepository = ativoRepository;
            _bolsaRepository = bolsaRepository;
            _criarAtivoRequestValidator = criarAtivoRequestValidator;
        }

        public async Task<AtivoResponse> CriarAsync(CriarAtivoRequest request)
        {
            await _criarAtivoRequestValidator.ValidateAndThrowAsync(request);

            var ativoExistente = await _ativoRepository.BuscarPorTickerAsync(request.Ticker);

            if (ativoExistente is not null)
            {
                throw new TickerJaCadastradoException(request.Ticker);
            }

            var bolsa = await _bolsaRepository.BuscarPorIdAsync(request.BolsaId);

            if (bolsa is null)
                throw new BolsaNaoEncontradaException(request.BolsaId);

            var ativo = new Ativo(
                request.Nome,
                request.Ticker,
                request.Tipo,
                request.BolsaId,
                request.Preco);

            await _ativoRepository.CriarAsync(ativo);

            await _ativoRepository.SalvarAlteracoesAsync();

            return new AtivoResponse
            {
                Id = ativo.Id,
                Nome = ativo.Nome,
                Ticker = ativo.Ticker,
                Tipo = ativo.Tipo,
                Bolsa = bolsa.Nome
            };
        }

        public async Task<AtivoResponse> BuscarPorIdAsync(Guid id)
        {
            var ativo = await _ativoRepository.BuscarPorIdAsync(id);

            return ativo is null ? throw new AtivoNaoEncontradoException(id) : AtivoResponse.FromEntity(ativo);
        }

        public async Task<IEnumerable<AtivoResponse>> BuscarTodosAsync()
        {
            var ativos = await _ativoRepository.BuscarTodosAsync();

            return ativos.Select(AtivoResponse.FromEntity);
        }

        public async Task AtualizarAsync(Guid id, AtualizarAtivoRequest request)
        {
            var ativo = await _ativoRepository.BuscarPorIdAsync(id) ?? throw new AtivoNaoEncontradoException(id);

            var ativoComMesmoTicker = await _ativoRepository.BuscarPorTickerAsync(request.Ticker);

            if (ativoComMesmoTicker is not null && ativoComMesmoTicker.Id != id)
            {
                throw new TickerJaCadastradoException(request.Ticker);
            }

            ativo.Atualizar(request.Nome, request.Ticker, request.Tipo);

            await _ativoRepository.SalvarAlteracoesAsync();
        }

        public async Task DesativarAsync(Guid id)
        {
            var ativo = await _ativoRepository.BuscarPorIdAsync(id) ?? throw new AtivoNaoEncontradoException(id);

            ativo.Desativar();

            await _ativoRepository.SalvarAlteracoesAsync();
        }
    }
}
