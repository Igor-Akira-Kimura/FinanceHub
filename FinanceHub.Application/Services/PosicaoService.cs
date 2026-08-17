using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;

namespace FinanceHub.Application.Services
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

            var ativo = await _ativoRepository.BuscarPorIdAsync(request.AtivoId);

            if (ativo is null)
                throw new AtivoNaoEncontradoException(request.AtivoId);

            var posicao = await _posicaoRepository.BuscarPorCarteiraEAtivoAsync(request.CarteiraId, request.AtivoId);

            if (posicao is not null)
            {
                var movimentacao = posicao.Comprar(request.Quantidade, ativo.Preco);

                await _movimentacaoRepository.CriarAsync(movimentacao);

                await _posicaoRepository.SalvarAlteracoesAsync();
            }
            else
            {
                posicao = new Posicao(request.CarteiraId, request.AtivoId, request.Quantidade, ativo.Preco);
                await _posicaoRepository.CriarAsync(posicao);
            }
        }
    }
}
