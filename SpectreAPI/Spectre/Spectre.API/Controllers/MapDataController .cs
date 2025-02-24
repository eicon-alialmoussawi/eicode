using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapDataController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMapDataRepository _mapDataRepository;
        private readonly ILogger<MapDataController> _logger;

        public MapDataController(IMapper mapper, ILogger<MapDataController> logger, IMapDataRepository mapDataRepository)
        {
            _mapDataRepository = mapDataRepository ?? throw new ArgumentNullException(nameof(mapDataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // Get MapData by name
        [HttpGet("{name}")]
        public async Task<IActionResult> GetJsonByName(string name)
        {
            try
            {
                var jsonData = await _mapDataRepository.GetJsonByName(name);

                if (jsonData == null)
                {
                    return NotFound(new { message = "Data not found" });
                }

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving MapData with name: {name}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
