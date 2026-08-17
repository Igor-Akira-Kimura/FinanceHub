using FinanceHub.Application.Requests.Carteiras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Application.Interfaces.Services
{
    public interface ICompraService
    {
        Task ComprarAsync(ComprarAtivoRequest request);
    }
}
