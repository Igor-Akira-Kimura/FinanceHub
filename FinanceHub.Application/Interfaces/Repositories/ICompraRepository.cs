using FinanceHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface ICompraRepository
    {
        Task CriarAsync(Compra compra);
    }
}
