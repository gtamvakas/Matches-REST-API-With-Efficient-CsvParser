using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw2backend.Controllers;
[AllowAnonymous]
public class UserController : Controller
{
    private IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Retrieve a token that will be used for requests to every "customer" route 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Token retrieved successfully</response>
    [HttpGet]
    [Route("customer/token")]
    public IActionResult GetCustomerJwtToken()
    {
        return Ok(new{token = _configuration.GetValue<string>("jwt:CustomerToken")});
    }
    /// <summary>
    /// Retrieve a token that will be used for requests to every "dev" route 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Token retrieved successfully</response>
    [HttpGet]
    [Route("dev/token")]
    public IActionResult GetDevJwtToken()
    {
        return Ok(new{ token = _configuration.GetValue<string>("jwt:DevToken")});
    }
}