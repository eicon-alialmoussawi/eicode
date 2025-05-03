using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazBand")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazBandController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazBandRepository _bazBandRepository; // Interface for BazBand Repository
    private readonly ILogger<BazBandController> _logger;

    public BazBandController(IMapper mapper, ILogger<BazBandController> logger, IBazBandRepository bazBandRepository)
    {
        _bazBandRepository = bazBandRepository ?? throw new ArgumentNullException(nameof(bazBandRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazBands
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _bazBandRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazBands.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazBand by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bazBandRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazBand not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazBand with ID {BazBandId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
