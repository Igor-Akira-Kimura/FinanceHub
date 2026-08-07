using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Services
{
    public class AtivoService : IAtivoService
    {
        private readonly IAtivoRepository _ativoRepository;
        private readonly IBolsaRepository _bolsaRepository;

        public AtivoService(IAtivoRepository ativoRepository, IBolsaRepository bolsaRepository)
        {
            _ativoRepository = ativoRepository;
            _bolsaRepository = bolsaRepository;
        }

        public async Task<AtivoResponse> CriarAsync(CriarAtivoRequest request)
        {
            var ativoExistente = await _ativoRepository.BuscarPorTickerAsync(request.Ticker);

            if (ativoExistente is not null)
            {
                throw new TickerJaCadastradoException(request.Ticker);
            }

            _ = await _bolsaRepository.BuscarPorIdAsync(request.BolsaId) ?? throw new BolsaNaoEncontradaException(request.BolsaId);

            var ativo = new Ativo(
                request.Nome,
                request.Ticker,
                request.Tipo,
                request.BolsaId);

            await _ativoRepository.CriarAsync(ativo);

            await _ativoRepository.SalvarAlteracoesAsync();

            return AtivoResponse.FromEntity(ativo);
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
