using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using cw2backend.Models;
using cw2backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw2backend.Controllers;
[ApiController]
[Route("customer")]
[Authorize(Roles = "Developer,Customer")]
public class CustomerController : Controller
{
    private readonly IMatchService _matchService;


    public CustomerController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    /// <summary>
    /// Retrieves the teams for a specific division
    /// </summary>  
    /// <param name="division"></param>
    /// <param name="useModifiers">Adding the value "true"(without quotes) to the optional query parameter useModifiers, will result
    /// in showing only the data that have the current active modifiers as Division</param>
    /// <returns>200 Ok response</returns>
    [HttpGet]
    [Route("teams/{division}")]
    [Produces("application/json")]
    
    public ActionResult<IEnumerable<string>> GetTeamsByDivision(string division, [FromQuery] string? useModifiers)
    {
        if (!string.IsNullOrEmpty(useModifiers) && useModifiers.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new { data = _matchService.GetTeamsByDivision(division, true).Distinct()});
        }
            
        
        return Ok(new {data = _matchService.GetTeamsByDivision(division).Distinct()});
    }

    ///  <summary>
    ///  Retrieves the matches for the specific date
    ///  </summary>
    ///  <param name="date">Date MUST be in format M-d-yy or MM-DD-yyyy or M-d-yyyy</param>
    ///  <param name="useModifiers">Adding the value "true"(without quotes) to the optional query parameter useModifiers, will result
    ///  in showing only the data that have the current active modifiers as Division</param>
    ///  <returns>200 Ok response</returns>
    [HttpGet]
    [Route("matches/{date:datetime}")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Match>> GetMatchesByDate(DateTime date, [FromQuery] string? useModifiers)
    {
        if (!string.IsNullOrEmpty(useModifiers) && useModifiers.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new {data = _matchService.GetMatchesByDate(date, true)});
        }
        return Ok(new { data = _matchService.GetMatchesByDate(date)});
    }

    /// <summary>
    /// Retrieves the matches played by the specific team.
    /// 
    /// </summary>
    /// <param name="teamName">Case Sensitive</param>
    /// <param name="useModifiers">Adding the value "true"(without quotes) to the optional query parameter useModifiers, will result
    /// in showing only the data that have the current active modifiers as Division.
    /// </param>
    /// <returns>200 Ok response</returns>
    [HttpGet]
    [Route("{teamName}/matches")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Match>> GetMatchesByTeam(string teamName, [FromQuery] string? useModifiers)
    {
        if (!string.IsNullOrEmpty(useModifiers) && useModifiers.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new{data = _matchService.GetMatchesByTeam(teamName, true)});
        }
        return Ok(new {data = _matchService.GetMatchesByTeam(teamName)});
    }

    /// <summary>
    /// Retrieves matches that were played in the specific time period.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="useModifiers">Adding the value "true"(without quotes) to the optional query parameter useModifiers, will result
    /// in showing only the data that have the current active modifiers as Division.
    /// </param>
    /// <returns>200 Ok response</returns>
    [HttpGet]
    [Route("matches")]
    [Produces("application/json")]
    public ActionResult<IEnumerable<Match>> GetMatchesByTimePeriod([FromQuery][Required] DateTime from, [FromQuery][Required] DateTime to, [FromQuery] string? useModifiers)
    {
        if (!string.IsNullOrEmpty(useModifiers) && useModifiers.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new {data = _matchService.GetMatchesByTimePeriod(from, to, true)});
        }
        return Ok(new { data = _matchService.GetMatchesByTimePeriod(from, to)});
    }


}