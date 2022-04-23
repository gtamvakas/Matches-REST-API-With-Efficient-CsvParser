using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using cw2backend;
using cw2backend.Repositories;
using cw2backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;
using Match = cw2backend.Models.Match;

namespace MatchesApi.Tests;

public class CustomerControllerTests
{ 
    private readonly MatchService _matchService;
    private readonly Mock<IMatchesRepository> _repo;

       
   public CustomerControllerTests()
   {
       _repo = new Mock<IMatchesRepository>();
       _matchService = new MatchService(_repo.Object);
   }
   
    [Fact]
    public void GetTeamsByDivision_returns_list_of_teams_when_division_is_specified()
    {
        var teamsList = new List<string>()
        {
            "Arsenal",
            "Bayern"
        };
        var division = "E0";
        _repo.Setup(repo => repo.GetTeamsByDivision(division, false)).Returns(teamsList);

        var result = _matchService.GetTeamsByDivision(division);
        
        Assert.Equal(teamsList, result);
    }
    
    [Fact]
    public void GetTeamsByDivision_returns_empty_list_when_division_does_not_exist()
    {
        //empty list
        var teamsList = new List<string>();
        var division = "XX";
        _repo.Setup(repo => repo.GetTeamsByDivision(division, false)).Returns(teamsList);

        var result = _matchService.GetTeamsByDivision(division);
        
        Assert.Equal(teamsList, result);
    }
 
}