using Spectre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface  IRegisterationRequestRepository
    {
        Task<IEnumerable<RegisterationRequest>> GetRegisterationRequests(int StatusId );


        Task<RegisterationRequest> GetRegisterationRequestById(int RequestId);
        Task<Tuple<bool, RegisterationRequest>> Create(RegisterationRequest RegisterationRequest);
    }
}
