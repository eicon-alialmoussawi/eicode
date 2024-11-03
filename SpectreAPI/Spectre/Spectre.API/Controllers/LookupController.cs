using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/Lookup")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LookupController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILookupRepository ILookupRepository;

        public LookupController(IMapper mapper, ILookupRepository ILookupRepository)
        {
            this.ILookupRepository = ILookupRepository;
            this.mapper = mapper;
        }
        [HttpGet("GetLookupByCode")]
        public async Task<ActionResult<IEnumerable<Lookup>>> GetLookupByCode(string Code)
        {
            var result = await ILookupRepository.GetLookupByCode(Code);
            return Ok(result);
        }
        [HttpGet("GetLookupsByParantCode")]
        public async Task<ActionResult<IEnumerable<Lookup>>> GetLookupsByParantCode(string Code)
        {
            var result = await ILookupRepository.GetLookupsByParantCode(Code);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            var Result = await ILookupRepository.GetById(Id);
            if (Result == null)
                return BadRequest();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Result = await ILookupRepository.GetAll();

            var LookupData = GetLookupData((List<Lookup>)Result, null);
            return Ok(LookupData);
        }
        private dynamic GetLookupData(List<Lookup> myLookups, Lookup myLookup)
        {
            if (myLookups != null)
            {
                var q = from r in myLookups
                        select new
                        {
                            Id = r.Id,
                            ValueEn = (r.Name == null || r.Name == "") ? r.NameAr : r.Name,
                            ValueAr = r.NameAr,
                            Code = r.LookupCode,
                            UserDefined = r.UserDefined,
                            ParentId = r.ParentId,
                            ActualValueEn = r.Name,
                            creationDate = r.CreationDate,
                            parentName = getParentName(myLookups, r.ParentId)
                        };
                return q;
            }
            else
            {
                myLookup.Name = (myLookup.Name == null || myLookup.Name == "") ? myLookup.NameAr : myLookup.Name;
                return myLookup;
            }
        }
        private dynamic getParentName(List<Lookup> myLookups, int? ParentId)
        {
            var mylookyp = new Lookup();
            if (ParentId != null)
            {
                foreach (Lookup lookup in myLookups)
                {
                    if (lookup.Id == ParentId)
                    {
                        mylookyp = lookup; break;
                    }
                }
                return mylookyp;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Lookup Lookup)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (Lookup.Id == 0)
            {
                Lookup.CreationDate = DateTime.Now;
                //  Lookup.CreatedBy = UserId;
                var Result = await ILookupRepository.Create(Lookup);
                if (!Result.Item1)
                    return BadRequest();



                return Ok(Result.Item2);
            }

            else
            {
                var res = await ILookupRepository.GetById(Lookup.Id);
                Lookup.CreationDate = res.CreationDate;
                var Result = await ILookupRepository.Update(Lookup);

                if (!Result.Item1)
                    return BadRequest();


                return Ok(Result.Item2);
            }
        }

    }
}
