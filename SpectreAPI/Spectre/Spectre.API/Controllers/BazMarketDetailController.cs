using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazMarketDetail")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazMarketDetailController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazMarketDetailRepository _bazMarketDetailRepository; // Interface for BazMarketDetail Repository
    private readonly ILogger<BazMarketDetailController> _logger;

    public BazMarketDetailController(IMapper mapper, ILogger<BazMarketDetailController> logger, IBazMarketDetailRepository bazMarketDetailRepository)
    {
        _bazMarketDetailRepository = bazMarketDetailRepository ?? throw new ArgumentNullException(nameof(bazMarketDetailRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazMarketDetails
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _bazMarketDetailRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazMarketDetails.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazMarketDetail by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bazMarketDetailRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazMarketDetail not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazMarketDetail with ID {BazMarketDetailId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
