using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Enums;

namespace FinanceHub.Api.Responses
{
    public class AtivoResponse
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Ticker { get; set; } = string.Empty;

        public TipoAtivo Tipo { get; set; }

        public string Bolsa { get; set; } = string.Empty;

        public static AtivoResponse FromEntity(Ativo ativo)
        {
            return new AtivoResponse
            {
                Id = ativo.Id,
                Nome = ativo.Nome,
                Ticker = ativo.Ticker,
                Tipo = ativo.Tipo,
                Bolsa = ativo.Bolsa.Nome
            };
        }
    }
}
