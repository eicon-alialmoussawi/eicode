using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazLicense")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazLicenseController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazLicenseRepository _BazLicenseRepository; // Interface for BazLicense Repository
    private readonly ILogger<BazLicenseController> _logger;

    public BazLicenseController(IMapper mapper, ILogger<BazLicenseController> logger, IBazLicenseRepository BazLicenseRepository)
    {
        _BazLicenseRepository = BazLicenseRepository ?? throw new ArgumentNullException(nameof(BazLicenseRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazLicenses
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _BazLicenseRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazLicenses.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazLicense by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _BazLicenseRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazLicense not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazLicense with ID {BazLicenseId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
