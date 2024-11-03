using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IContactUsRepository
    {
        Task<Tuple<bool, ContactU>> Create(ContactU contact);
        Task<Tuple<bool, List<ContactUs_View>>> GetAlll();
        Task<Tuple<bool>> DeleteContact(int Id);
    }
}
