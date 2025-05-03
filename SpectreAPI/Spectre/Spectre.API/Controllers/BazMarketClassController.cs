using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazMarketClass")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazMarketClassController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazMarketClassRepository _bazMarketClassRepository; // Interface for BazMarketClass Repository
    private readonly ILogger<BazMarketClassController> _logger;

    public BazMarketClassController(IMapper mapper, ILogger<BazMarketClassController> logger, IBazMarketClassRepository bazMarketClassRepository)
    {
        _bazMarketClassRepository = bazMarketClassRepository ?? throw new ArgumentNullException(nameof(bazMarketClassRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazMarketClasss
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _bazMarketClassRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazMarketClasss.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazMarketClass by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bazMarketClassRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazMarketClass not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazMarketClass with ID {BazMarketClassId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
