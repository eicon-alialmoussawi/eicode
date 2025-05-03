using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazMarket")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazMarketController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazMarketRepository _bazMarketRepository; // Interface for BazMarket Repository
    private readonly ILogger<BazMarketController> _logger;

    public BazMarketController(IMapper mapper, ILogger<BazMarketController> logger, IBazMarketRepository bazMarketRepository)
    {
        _bazMarketRepository = bazMarketRepository ?? throw new ArgumentNullException(nameof(bazMarketRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazMarkets
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _bazMarketRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazMarkets.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazMarket by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bazMarketRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazMarket not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazMarket with ID {BazMarketId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
