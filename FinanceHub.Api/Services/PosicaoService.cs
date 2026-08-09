using FinanceHub.Api.Application.Requests.Carteiras;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Repositories;
using FinanceHub.Api.Requests;

namespace FinanceHub.Api.Services
{
    public class PosicaoService : IPosicaoService
    {
        private readonly IPosicaoRepository _posicaoRepository;
        private readonly ICarteiraRepository _carteiraRepository;
        private readonly IAtivoRepository _ativoRepository;
        private readonly IMovimentacaoRepository _movimentacaoRepository;

        public PosicaoService(IPosicaoRepository posicaoRepository, ICarteiraRepository carteiraRepository, IAtivoRepository ativoRepository, IMovimentacaoRepository movimentacaoRepository)
        {
            _posicaoRepository = posicaoRepository;
            _carteiraRepository = carteiraRepository;
            _ativoRepository = ativoRepository;
            _movimentacaoRepository = movimentacaoRepository;
        }

        public async Task ComprarAtivoAsync(ComprarAtivoRequest request)
        {
            var carteira = await _carteiraRepository.BuscarPorIdAsync(request.CarteiraId);

            if (carteira is null)
                throw new CarteiraNaoEncontradaException(request.CarteiraId);

            var posicao = await _posicaoRepository.BuscarPorCarteiraEAtivoAsync(request.CarteiraId, request.AtivoId);

            if (posicao is not null)
            {
                var movimentacao = posicao.Comprar(request.Quantidade, request.Preco);

                await _movimentacaoRepository.CriarAsync(movimentacao);

                await _posicaoRepository.SalvarAlteracoesAsync();
            }
            else
            {
                posicao = new Posicao(request.CarteiraId, request.AtivoId, request.Quantidade, request.Preco);
                await _posicaoRepository.CriarAsync(posicao);
            }
        }
    }
}
