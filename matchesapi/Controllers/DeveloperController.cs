using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using cw2backend.Models;
using cw2backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw2backend.Controllers;

[ApiController]
[Route("dev")]
[Authorize(Roles = "Developer")]
public class DeveloperController : Controller
{
    private readonly IMatchService _matchService;
    private readonly IReaderService _readerService;

    public DeveloperController(IMatchService matchService, IReaderService readerService)
    {
        _matchService = matchService;
        _readerService = readerService;
    }
    
    /// <summary>
    /// Reads all the CSV files in storage and saves all the data in the database.
    /// </summary>
    /// <returns>200 response if the data were parsed successfully or a 409 conflict if the data have already been parsed.</returns>
    /// <response code="200">Data parse successfully</response>
    /// <response code="409">If data have already been parsed</response>
    [HttpGet]
    [Route("parse")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Parse()
    {
        
        return !_readerService.ParseFiles() ? Conflict("Data already parsed!") : Ok();
    }
    
    /// <summary>
    /// Inserts the specific match in the database.
    /// </summary>
    /// <param name="match"></param>
    /// <returns>400 Bad Request if the match provided is missing the required fields or a 201 Created along with the created object if successful.</returns>
    /// <remarks>
    ///     {
    ///          POST example with required only fields
    /// 
    ///         "id":1,
    ///         "division": "E0",
    ///         "homeTeam" : "Arsenal",
    ///         "awayTeam" : "Manchester",
    ///         "fullTimeResult" : "W"
    ///     }
    /// </remarks>
    /// <response code="201">Returns the newly created object</response>
    /// <response code="400">If required fields are missing</response> 
    [HttpPost]
    [Route("matches")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult InsertMatch(Match match)
    {
        if (!match.Valid())
        {
            return BadRequest("Fields: division, home_team, away_team, full_time_result are required!");
        }

        _matchService.InsertMatch(match);
        return Created(nameof(match), match);
    }

    /// <summary>
    /// Updates a specific match.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="match"></param>
    /// <returns>400 Bad Request if the match provided is missing the required fields or a 404 Not Found if the id provided does not match a record or a 200 Ok if successful.</returns>
    ///  <remarks>
    ///     {
    ///          PUT example with required only fields
    ///
    ///         "division": "E0",
    ///         "homeTeam" : "Arsenal",
    ///         "awayTeam" : "Manchester",
    ///         "fullTimeResult" : "W"
    ///     }
    /// </remarks>
    /// <response code="200">Match updated successfully</response>
    /// <response code="404">If required fields are missing</response> 
    /// <response code="400">If required fields are missing</response> 
    [HttpPut]
    [Route("matches/{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult EditMatch(int id, Match match)
    {
        if (!match.Valid())
        {
            return BadRequest("Fields: division, home_team, away_team, full_time_result are required!");
        }

        return _matchService.EditMatch(id, match) == 0 && _matchService.EditStatistic(id, match) == 0 ? NotFound() : Ok();
    }

    /// <summary>
    /// Deletes a specific match.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>404 Not Found if the id provided does not match a record or a 204 No Content if successful.</returns>
    /// <response code="204">Match deleted successfully</response>
    /// <response code="404">If the id provided does not match a record</response> 
    [HttpDelete]
    [Route("matches/{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(int id)
    {
        return _matchService.DeleteMatch(id) == 0 ? NotFound() : NoContent();
    }

    /// <summary>
    /// Deletes all the data in the database.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Data deleted successfully</response> 
    [HttpGet] //Post is probably better for something like this.
    [Route("matches/delete")]
    public IActionResult DeleteAll()
    {
        return Ok(_matchService.DeleteAllData());
    }

    /// <summary>
    /// Retrieves all the currently active modifiers.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Data retrieved successfully</response> 
    [HttpGet] 
    [Route("modifiers")]
    public IActionResult GetModifiers()
    {
        return Ok(new{modifiers = _matchService.GetModifiers()});
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