using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Domain.ValueObject
{
    public record Dinheiro
    {
        public decimal Valor { get; }
        public string Moeda { get; }

        public Dinheiro(decimal valor, string moeda)
        {
            if (valor < 0)
                throw new ArgumentException();

            Valor = valor;
            Moeda = moeda;
        }
    }
}
