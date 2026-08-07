using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Services
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
