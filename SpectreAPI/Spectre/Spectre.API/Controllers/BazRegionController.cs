using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazRegion")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazRegionController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazRegionRepository _BazRegionRepository; // Interface for BazRegion Repository
    private readonly ILogger<BazRegionController> _logger;

    public BazRegionController(IMapper mapper, ILogger<BazRegionController> logger, IBazRegionRepository BazRegionRepository)
    {
        _BazRegionRepository = BazRegionRepository ?? throw new ArgumentNullException(nameof(BazRegionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazRegions
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _BazRegionRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazRegions.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazRegion by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _BazRegionRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazRegion not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazRegion with ID {BazRegionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
