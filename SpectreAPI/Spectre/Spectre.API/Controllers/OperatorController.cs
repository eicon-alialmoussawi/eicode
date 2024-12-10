using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.RepositoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.API.Controllers
{
    [Route("api/Operator")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ensure the user is authenticated

    public class OperatorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOperatorRepository _operatorRepository; // Interface for Operator Repository
        private readonly ILogger _logger;

        public OperatorController(IMapper mapper, ILogger logger, IOperatorRepository operatorRepository)
        {
            _operatorRepository = operatorRepository ?? throw new ArgumentNullException(nameof(operatorRepository));
            _mapper = mapper;
            _logger = logger;
        }

        // Get all operators
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _operatorRepository.GetAll();
            return Ok(result);
        }

        // Get an _operator by ID
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _operatorRepository.GetById(id);
            if (result == null)
            {
                return NotFound("Operator not found");
            }
            return Ok(result);
        }

        // Import operators in bulk (using a list of operators)
        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(List<Operator> operators)
        {
            var tokenClaims = HttpContext.User;
            int userId = int.Parse(tokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            var userName = tokenClaims.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;

            foreach (var _operator in operators)
            {
                var res = await _operatorRepository.Create(_operator);
                // You can log the successful creation or handle errors here if needed
            }
            return Ok("Success");
        }

        // Get operators related to a specific user, along with additional filters
        [HttpGet]
        [Route("GetUserOperators")]
        public async Task<IActionResult> GetUserOperators(string pageUrl, string source, string lang)
        {
            var tokenClaims = HttpContext.User;
            int userId = int.Parse(tokenClaims.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            var result = await _operatorRepository.GetUserOperators(userId, pageUrl, source, lang);
            if (!result.Item1)
                return BadRequest("Error retrieving operators");
            return Ok(result.Item2);
        }
    }
}
