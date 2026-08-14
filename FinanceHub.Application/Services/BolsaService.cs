using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Responses;

namespace FinanceHub.Application.Services
{
    public class BolsaService : IBolsaService
    {
        private readonly IBolsaRepository _bolsaRepository;

        public BolsaService(IBolsaRepository bolsaRepository)
        {
            _bolsaRepository = bolsaRepository;
        }

        public async Task<IEnumerable<BolsaResponse>> BuscarTodasAsync()
        {
            var bolsas = await _bolsaRepository.BuscarTodasAsync();

            return bolsas.Select(b => new BolsaResponse
            {
                Id = b.Id,
                Nome = b.Nome
            });
        }
    }
}
