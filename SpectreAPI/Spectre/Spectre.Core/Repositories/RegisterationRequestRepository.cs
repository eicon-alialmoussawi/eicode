using Microsoft.EntityFrameworkCore;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spectre.Core.Repositories
{
    public class RegisterationRequestRepository : Repository<RegisterationRequest>, IRegisterationRequestRepository
    {
        private readonly ILogger Logger;
        public RegisterationRequestRepository(SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, RegisterationRequest>> Create(RegisterationRequest RegisterationRequest)
        {
            try
            {
                var Result = await MyDbContext.RegisterationRequests.AddAsync(RegisterationRequest);
                MyDbContext.SaveChanges();

                return new Tuple<bool, RegisterationRequest>(true, Result.Entity);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, RegisterationRequest>(false, null);
            }
        
        }

        public async Task<RegisterationRequest> GetRegisterationRequestById(int RequestId)
        {
            return await MyDbContext.RegisterationRequests.Where(m => m.Id == RequestId).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<RegisterationRequest>> GetRegisterationRequests(int StatusId)
        {
            return await MyDbContext.RegisterationRequests.Where(m => m.StatusId == StatusId).ToListAsync();
        }

  
    }
}
