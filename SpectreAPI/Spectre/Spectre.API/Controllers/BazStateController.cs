using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazState")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazStateController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazStateRepository _BazStateRepository; // Interface for BazState Repository
    private readonly ILogger<BazStateController> _logger;

    public BazStateController(IMapper mapper, ILogger<BazStateController> logger, IBazStateRepository BazStateRepository)
    {
        _BazStateRepository = BazStateRepository ?? throw new ArgumentNullException(nameof(BazStateRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazStates
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _BazStateRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazStates.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazState by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _BazStateRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazState not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazState with ID {BazStateId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
