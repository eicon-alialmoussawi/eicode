using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Repositories
{
    public class RegionRepository
   : Repository<Region>, IRegionRepository
    {
        private readonly ILogger Logger;
        private readonly IConfiguration configuration;
        public RegionRepository(IConfiguration configuration, SpectreDBContext context, ILogger Logger)
            : base(context)
        {
            this.Logger = Logger;
            this.configuration = configuration;

        }
        private SpectreDBContext MyDbContext
        {
            get { return Context as SpectreDBContext; }
        }

        public async Task<Tuple<bool, RegionDetails>> Create(RegionDetails Region)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    string countries_xml = "";
                    int RegionId = Region.RegionId != 0 ?  Region.RegionId : 0 ;
                    Connection.Open();
                    SqlCommand Command = new SqlCommand("CreateRegion", Connection);
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@RegionId", RegionId);
                    Command.Parameters.AddWithValue("@NameAr", Region.NameAr);
                    Command.Parameters.AddWithValue("@NameFr", Region.NameFr);
                    Command.Parameters.AddWithValue("@NameEn", Region.NameEn);
                    Command.Parameters.AddWithValue("@Code", Region.Code);

                    if (!string.IsNullOrEmpty(Region.Countries))
                    {
                        List<Country> Countries = JsonConvert.DeserializeObject<List<Country>>(Region.Countries);
                        countries_xml = "<Countries>";

                        foreach (Country country in Countries)
                        {
                            countries_xml += "<Country>";
                            countries_xml += "<Id>";
                            countries_xml += country.CountryId;
                            countries_xml += "</Id>";
                            countries_xml += "<Iso3>";
                            countries_xml += country.Iso3;
                            countries_xml += "</Iso3>";
                            countries_xml += "</Country>";
                        }

                        countries_xml += "</Countries>";

                    }

                    Command.Parameters.AddWithValue("@Countries_XML", countries_xml);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
                return Tuple.Create<bool, RegionDetails>(true, Region);
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return Tuple.Create<bool, RegionDetails>(false, null);
            }
        }

        public async Task<Tuple<bool, RegionDetails>> GetById(int Id)
        {
            try
            {
                List<RegionDetails> regions = new List<RegionDetails>();
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    DataSet dbResult = new DataSet();
                    SqlCommand command = Connection.CreateCommand();
                    command.CommandText = "GetRegionById";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", Id);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dbResult);

                    DataTable dtCountries = new DataTable();
                    DataTable dtRegions = new DataTable();

                    RegionDetails region = new RegionDetails();

                    if (dbResult.Tables.Count > 0)
                    {
                        if (dbResult.Tables[0] != null && dbResult.Tables[0].Rows.Count > 0)
                        {
                            dtRegions = dbResult.Tables[0];
                        }
                        if (dbResult.Tables[1] != null && dbResult.Tables[1].Rows.Count > 0)
                        {
                            dtCountries = dbResult.Tables[1];
                        }
                        DataRow row = dtRegions.Rows[0];

                           
                            region.RegionId = row.IsNull("RegionId") ? 0 : row.Field<int>("RegionId");
                            region.Code = row.IsNull("Code") ? "" : row.Field<string>("Code");
                            region.NameAr = row.IsNull("NameAr") ? "" : row.Field<string>("NameAr");
                            region.NameEn = row.IsNull("NameEn") ? "" : row.Field<string>("NameEn");
                            region.NameFr = row.IsNull("NameFr") ? "" : row.Field<string>("NameFr");

                            if (dtCountries != null && dtCountries.Rows.Count > 0)
                            {
                                List<Country_View> regionCountries = new List<Country_View>();
                                foreach (DataRow rowCountry in dtCountries.Rows)
                                {
                                    Country_View country = new Country_View();
                                    country.CountryId = rowCountry.IsNull("CountryId") ? 0 : rowCountry.Field<int>("CountryId");
                                    country.RegionId = rowCountry.IsNull("RegionId") ? 0 : rowCountry.Field<int>("RegionId");
                                    country.Iso3 = rowCountry.IsNull("Iso3") ? "" : rowCountry.Field<string>("Iso3");
                                    country.NameAr = rowCountry.IsNull("NameAr") ? "" : rowCountry.Field<string>("NameAr");
                                    country.NameEn = rowCountry.IsNull("NameEn") ? "" : rowCountry.Field<string>("NameEn");
                                    regionCountries.Add(country);
                                }

                                var jsonCountries = JsonConvert.SerializeObject(regionCountries);
                                region.Countries = jsonCountries;
                            }

                            
                        


                    }

                    Connection.Close();
                    return new Tuple<bool, RegionDetails>(true, region);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, RegionDetails>(false, null);
            }
        }

        public async Task<Tuple<bool, List<RegionDetails>>> GetRegions()
        {
            try
            {
                List<RegionDetails> regions = new List<RegionDetails>();
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    DataSet dbResult = new DataSet();
                    SqlCommand command = Connection.CreateCommand();
                    command.CommandText = "GetRegionDetails";
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@Lang", Lang);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dbResult);

                    DataTable dtCountries = new DataTable();
                    DataTable dtRegions = new DataTable();

                    if (dbResult.Tables.Count > 0 )
                    {
                        if(dbResult.Tables[0] != null && dbResult.Tables[0].Rows.Count > 0)
                        {
                            dtRegions = dbResult.Tables[0];
                        }
                        if (dbResult.Tables[1] != null && dbResult.Tables[1].Rows.Count > 0)
                        {
                            dtCountries = dbResult.Tables[1];
                        }

                        foreach (DataRow row in dtRegions.Rows)
                        {
                            RegionDetails region = new RegionDetails();
                            region.RegionId = row.IsNull("RegionId") ? 0 : row.Field<int>("RegionId");
                            region.Code = row.IsNull("Code") ? "" : row.Field<string>("Code");
                            region.NameAr = row.IsNull("NameAr") ? "" : row.Field<string>("NameAr");
                            region.NameEn = row.IsNull("NameEn") ? "" : row.Field<string>("NameEn");
                            region.NameFr = row.IsNull("NameFr") ? "" : row.Field<string>("NameFr");

                            var Countries = (from DataRow rw in dtCountries.Rows
                                             where rw.Field<int>("RegionId") == region.RegionId
                                             select rw).Distinct().ToList();

                            if (Countries != null && Countries.Count > 0)
                            {
                                List<Country_View> regionCountries = new List<Country_View>();
                                foreach(DataRow rowCountry in Countries)
                                {
                                    Country_View country = new Country_View();
                                    country.CountryId = rowCountry.IsNull("CountryId") ? 0 : rowCountry.Field<int>("CountryId");
                                    country.RegionId = rowCountry.IsNull("RegionId") ? 0 : rowCountry.Field<int>("RegionId");
                                    country.Iso3 = rowCountry.IsNull("Iso3") ? "" : rowCountry.Field<string>("Iso3");
                                    country.NameAr = rowCountry.IsNull("NameAr") ? "" : rowCountry.Field<string>("NameAr");
                                    country.NameEn = rowCountry.IsNull("NameEn") ? "" : rowCountry.Field<string>("NameEn");
                                    regionCountries.Add(country);
                                }

                                var jsonCountries = JsonConvert.SerializeObject(regionCountries);
                                region.Countries = jsonCountries;
                            }

                            regions.Add(region);
                        }


                    }

                    Connection.Close();
                    return new Tuple<bool, List<RegionDetails>>(true, regions);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool, List<RegionDetails>>(false, null);
            }
        }

        public async Task<Tuple<bool>> Delete(int Id)
        {

            try
            {
                using (SqlConnection Connection = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                {
                    Connection.Open();
                    var Params = new DynamicParameters();
                    Params.Add("@RegionId", Id);


                    var Results = await Connection.ExecuteAsync("DeleteRegion", Params, commandType: CommandType.StoredProcedure);
                    Connection.Close();
                    return new Tuple<bool>(true);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                return new Tuple<bool>(false);
            }
        }


    }
}
