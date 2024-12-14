using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Added this for ILogger
using Spectre.Core.Interfaces;
using System;
using System.Threading.Tasks;

[Route("api/BazAuction")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated
public class BazAuctionController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBazAuctionRepository _bazAuctionRepository; // Interface for BazAuction Repository
    private readonly ILogger<BazAuctionController> _logger;

    public BazAuctionController(IMapper mapper, ILogger<BazAuctionController> logger, IBazAuctionRepository bazAuctionRepository)
    {
        _bazAuctionRepository = bazAuctionRepository ?? throw new ArgumentNullException(nameof(bazAuctionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Get all BazAuctions
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _bazAuctionRepository.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all BazAuctions.");
            return StatusCode(500, "Internal server error");
        }
    }

    // Get a BazAuction by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bazAuctionRepository.GetById(id);
            if (result == null)
            {
                return NotFound("BazAuction not found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving BazAuction with ID {BazAuctionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
