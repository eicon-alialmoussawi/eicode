using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazCounty")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazCountyController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazCountyRepository _BazCountyRepository; // Interface for BazCounty Repository
    private readonly ILogger<BazCountyController> _logger;

    public BazCountyController(IMapper mapper, ILogger<BazCountyController> logger, IBazCountyRepository BazCountyRepository)
    {
        _BazCountyRepository = BazCountyRepository ?? throw new ArgumentNullException(nameof(BazCountyRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazCountys
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _BazCountyRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazCountys.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazCounty by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _BazCountyRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazCounty not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazCounty with ID {BazCountyId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
