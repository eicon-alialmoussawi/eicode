using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Spectre.API.Utilities;
using Microsoft.AspNetCore.Identity;
using Spectre.Core.Models.Extenders;
using System.Collections;

namespace Spectre.API.Controllers
{
    [Route("api/SocioEconomic")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SocioEonomicsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IScocioEconomicRepository IScocioEconomicRepository;
        private readonly ICountryRepository ICountryRepository;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly ILogger ILogger;
        private readonly ILookupRepository ILookupRepository;

        public SocioEonomicsController(ILookupRepository ILookupRepository, ICountryRepository ICountryRepository, IMapper mapper, UserManager<IdentityUser> UserManager, IScocioEconomicRepository IScocioEconomicRepository, ILogger ILogger, IAwardRepository IAwardRepository)
        {
            this.IScocioEconomicRepository = IScocioEconomicRepository;
            this.mapper = mapper;
            this.ICountryRepository = ICountryRepository;
            this.UserManager = UserManager;
            this.ILookupRepository = ILookupRepository;
            this.ILogger = ILogger;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(SocioEonomic SocioEonomic)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            if (SocioEonomic.Id == 0)
            {
                SocioEonomic.CreationDate = DateTime.Now;
                SocioEonomic.CreatedBy = UserId;
                var Check = await IScocioEconomicRepository.CheckIfExists(SocioEonomic);
                if (Check.Item1)
                {
                    SocioEonomic.Id = Check.Item2;
                    var Result = await IScocioEconomicRepository.Create(SocioEonomic);
                    if (!Result.Item1)
                        return BadRequest();
                    else
                    {
                        await ILogger.LogUserAction(new UserActionLog
                        {
                            Id = 0,
                            UserId = UserId,
                            Action = Convert.ToInt32(Operations.Update),

                            Page = "SocioEonomic",
                            PageAr = "الإيداع الاقتصادي",
                            Details = ("SocioEonomic  for Year(" + SocioEonomic.Year + ") is added"),
                            DetailsAr = ("تم اضافة  الإيداع الاقتصادي (" + SocioEonomic.Year + ")"),
                            Date = DateTime.Now
                        });
                    }
                    return Ok(Result.Item1);
                }
                return Ok(true);
            }
            else
            {
                var Result = await IScocioEconomicRepository.Update(SocioEonomic);

                if (!Result.Item1)
                    return BadRequest();
                else
                {
                    await ILogger.LogUserAction(new UserActionLog
                    {
                        Id = 0,
                        UserId = UserId,
                        Action = Convert.ToInt32(Operations.Update),
                        Page = "SocioEonomic",
                        PageAr = "الإيداع الاقتصادي",
                        Details = ("SocioEonomic  for Year(" + SocioEonomic.Year + ") is Updated"),
                        DetailsAr = ("تم تعديل الإيداع الاقتصادي(" + SocioEonomic.Year + ")"),
                        Date = DateTime.Now
                    });



                }

                return Ok(Result.Item1);
            }
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(List<SocioEconomicImportView> SocioEonomics)
        {
            var TokenClaims = HttpContext.User;
            int UserId = int.Parse(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            var UserName = TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            List<FailedImportedAwards_View> result = new List<FailedImportedAwards_View>();

            foreach (SocioEconomicImportView SocioEconomicImportView in SocioEonomics)
            {
                try
                {
                    var SocioEonomic = new SocioEonomic();
                    SocioEonomic.CreationDate = DateTime.Now;
                    SocioEonomic.CreatedBy = UserId;
                    SocioEonomic.Id = 0;
                    SocioEonomic.SourceId = SocioEconomicImportView.SourceId;
                    SocioEonomic.Value = SocioEconomicImportView.Value;
                    SocioEonomic.Year = SocioEconomicImportView.Year;
                    SocioEonomic.CreatedBy = UserId;
                    SocioEonomic.IsDeleted = false;
                    SocioEonomic.CreationDate = DateTime.Now;
                    if (SocioEconomicImportView.Iso != null && SocioEconomicImportView.Iso != "")
                    {
                        var country = await ICountryRepository.getByIso3(SocioEconomicImportView.Iso);
                        if (SocioEconomicImportView.Iso == "USA")
                            SocioEonomic.CountryId = country.CountryId;
                        if (country != null)
                        {
                            SocioEonomic.CountryId = country.CountryId;
                            var Check = await IScocioEconomicRepository.CheckIfExists(SocioEonomic);
                            if (Check.Item1)
                            {
                                SocioEonomic.Id = Check.Item2;
                                var Result = await IScocioEconomicRepository.Create(SocioEonomic);
                                if (!Result.Item1)
                                    return BadRequest();
                                else
                                {
                                    await ILogger.LogUserAction(new UserActionLog
                                    {
                                        Id = 0,
                                        UserId = UserId,
                                        Action = Convert.ToInt32(Operations.Update),

                                        Page = "SocioEonomic",
                                        PageAr = "الإيداع الاقتصادي",
                                        Details = ("SocioEonomic  for Year(" + SocioEonomic.Year + ") is added"),
                                        DetailsAr = ("تم اضافة  الإيداع الاقتصادي (" + SocioEonomic.Year + ")"),
                                        Date = DateTime.Now
                                    });
                                }
                            }
                            else
                            {
                                var Result = await IScocioEconomicRepository.Create(SocioEonomic);
                                if (!Result.Item1)
                                    return BadRequest();
                                else
                                {
                                    await ILogger.LogUserAction(new UserActionLog
                                    {
                                        Id = 0,
                                        UserId = UserId,
                                        Action = Convert.ToInt32(Operations.Update),

                                        Page = "SocioEonomic",
                                        PageAr = "الإيداع الاقتصادي",
                                        Details = ("SocioEonomic  for Year(" + SocioEonomic.Year + ") is added"),
                                        DetailsAr = ("تم اضافة  الإيداع الاقتصادي (" + SocioEonomic.Year + ")"),
                                        Date = DateTime.Now
                                    });
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    FailedImportedAwards_View item = new FailedImportedAwards_View();
                    item.Country = "";
                    item.Month = "0";
                    item.Year = SocioEconomicImportView.Year;
                    item.Value = SocioEconomicImportView.Value;

                    result.Add(item);
                }

            }


            return Ok("Successfully done");
        }




        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var Result = await this.IScocioEconomicRepository.GetAll();
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllForView")]
        public async Task<IActionResult> GetAllForView()
        {

            var Result = await this.IScocioEconomicRepository.GetAllSocioEconomics();
            return Ok(Result);

        }
        [HttpGet]
        [Route("FilterSocioEconimcs")]
        public async Task<IActionResult> FilterSocioEconimcs(string CountryIds, int SourceId)
        {

            var Result = await this.IScocioEconomicRepository.FilterSocioEconimcs(CountryIds, SourceId);

            List<CustomSocioEconimcResponse> arrayList = new List<CustomSocioEconimcResponse>();
            int MinYear = DateTime.Now.Year;
            int MaxYear = DateTime.Now.Year;
            foreach (SocioEonomic_View2 item in Result)
            {
                List<SocioEonomic_View2> current = Result.Where(m => m.CountryId == item.CountryId).ToList();
                Result = Result.Where(m => m.CountryId != item.CountryId).ToList();
                SoioEconomicColumnHeader[] RowData = new SoioEconomicColumnHeader[current.Count()];
                int index = 0;
                foreach (SocioEonomic_View2 cur in current)
                {
                    SoioEconomicColumnHeader it = new SoioEconomicColumnHeader();
                    it.Value = cur.Value;
                    it.Year = cur.Year;
                    it.Id = cur.Id;
                    RowData[index] = it;
                    index++;
                    MinYear = Math.Min(MinYear, it.Year);
                    MaxYear = Math.Max(MaxYear, it.Year);
                }
                CustomSocioEconimcResponse ob = new CustomSocioEconimcResponse();
                ob.items = RowData;
                ob.SourceName = item.Source;
                ob.CountryName = item.CountryName;
                ob.CountryId = item.CountryId;
                if (CheckIfExists(arrayList, item.CountryId))
                    arrayList.Add(ob);
            }
            return Ok(new { Items = arrayList, FromYear = MinYear, ToYear = MaxYear });

        }
        [HttpGet]
        [Route("FilterSocioEconimcsByCountryId")]
        public async Task<IActionResult> FilterSocioEconimcsByCountryId(int CountryId, bool IsIMF)
        {
            int MinYear = DateTime.Now.Year;
            int MaxYear = DateTime.Now.Year;
            var LookupRes = await ILookupRepository.GetLookupsBySocialCode(IsIMF);
            var _Lookups = LookupRes.Item2;
            List<CustomSocioEconimcByCountryResponse> lst = new List<CustomSocioEconimcByCountryResponse>();
            foreach (Lookup lookup in _Lookups)
            {
                CustomSocioEconimcByCountryResponse CustomSocioEconimcByCountryResponse = new CustomSocioEconimcByCountryResponse();
                CustomSocioEconimcByCountryResponse.IndicatorName = lookup.Name;
                var CountrySourceSocio = await this.IScocioEconomicRepository.FilterSocioEconimcsByCountryAndSource(CountryId, lookup.Id);
                foreach (SoioEconomicColumnHeader cur in CountrySourceSocio)
                {

                    MinYear = Math.Min(MinYear, cur.Year);
                    MaxYear = Math.Max(MaxYear, cur.Year);
                }
                CustomSocioEconimcByCountryResponse.items = CountrySourceSocio.ToArray();
                lst.Add(CustomSocioEconimcByCountryResponse);
            }
            return Ok(new { Items = lst, FromYear = MinYear, ToYear = MaxYear });
        }


        [HttpGet]
        [Route("FilterSocioEconimcsByCountry")]
        public async Task<IActionResult> FilterSocioEconimcsByCountry(int CountryId)
        {

            var Result = await this.IScocioEconomicRepository.FilterSocioEconimcsByCountry(CountryId);

            List<CustomSocioEconimcResponse> arrayList = new List<CustomSocioEconimcResponse>();
            int MinYear = DateTime.Now.Year;
            int MaxYear = DateTime.Now.Year;
            foreach (SocioEonomic_View2 item in Result)
            {
                List<SocioEonomic_View2> current = Result.Where(m => m.CountryId == item.CountryId).ToList();
                Result = Result.Where(m => m.CountryId != item.CountryId).ToList();
                SoioEconomicColumnHeader[] RowData = new SoioEconomicColumnHeader[current.Count()];
                int index = 0;
                foreach (SocioEonomic_View2 cur in current)
                {
                    SoioEconomicColumnHeader it = new SoioEconomicColumnHeader();
                    it.Value = cur.Value;
                    it.Year = cur.Year;
                    RowData[index] = it;
                    index++;
                    MinYear = Math.Min(MinYear, it.Year);
                    MaxYear = Math.Max(MaxYear, it.Year);
                }
                CustomSocioEconimcResponse ob = new CustomSocioEconimcResponse();
                ob.items = RowData;
                ob.SourceName = item.Source;
                ob.CountryName = item.CountryName;
                ob.CountryId = item.CountryId;
                if (CheckIfExists(arrayList, item.CountryId))
                    arrayList.Add(ob);
            }
            return Ok(new { Items = arrayList, FromYear = MinYear, ToYear = MaxYear });

        }

        static bool CheckIfExists(List<CustomSocioEconimcResponse> arrayList, int CountryId)
        {
            foreach (CustomSocioEconimcResponse CustomSocioEconimcResponse in arrayList)
            {
                if (CustomSocioEconimcResponse.CountryId == CountryId)
                {
                    return false;
                }
            }
            return true;

        }
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {

            var Result = await this.IScocioEconomicRepository.GetById(Id);
            return Ok(Result);
        }

        [HttpGet]
        [Route("GetByCountryAndYear")]
        public async Task<IActionResult> GetByCountryAndYear(int CountryId, int Year, bool ISIMF)
        {

            var Result = await this.IScocioEconomicRepository.GetByCountryAndYear(CountryId, Year, ISIMF);
            return Ok(Result);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var TokenClaims = HttpContext.User;
            int UserId = Convert.ToInt32(TokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            string Lang = TokenClaims.Claims.FirstOrDefault(c => c.Type == "Lang").Value;

            var Result = await this.IScocioEconomicRepository.GetById(Id);
            Result.IsDeleted = true;
            var Res = await IScocioEconomicRepository.Update(Result);
            await ILogger.LogUserAction(new UserActionLog
            {
                Id = 0,
                UserId = UserId,
                Action = Convert.ToInt32(Operations.Delete),
                Page = "SocioEonomic",
                PageAr = "الإيداع الاقتصادي",
                Details = ("SocioEonomic  for Year(" + Result.Year + ") is Delete"),
                DetailsAr = ("تم مسح الإيداع الاقتصادي(" + Result.Year + ")"),
                Date = DateTime.Now
            });
            return Ok(Result);
        }

        [HttpPost]
        [Route("DeleteAllSocioEcnomic")]
        public async Task<IActionResult> DeleteAllSocioEcnomic()
        {

            var Result = await this.IScocioEconomicRepository.DeleteAllSocioEcnomic();
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item1);
        }


        [HttpGet]
        [Route("FilterSocioEconimcsByYear")]
        public async Task<IActionResult> FilterSocioEconimcsByYear(int Year, bool? IsIMF)
        {

            var Result = await this.IScocioEconomicRepository.FilterSocioEconimcsByYear(Year, IsIMF);
            if (!Result.Item1)
                return BadRequest();
            return Ok(Result.Item2);
        }

    }
}
