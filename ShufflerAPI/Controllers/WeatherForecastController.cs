using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities;
using SharedLibrary;
using SharedLibrary.Models;
using ShufflerAPI.DataAccess;
using System.Net.Http.Headers;

namespace ShufflerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IVoteData _voteData;
    private readonly ConfigHelper _config;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IVoteData voteData, ConfigHelper config)
    {
        _logger = logger;
        _voteData = voteData;
        _config = config;
    }

    [HttpGet(Name = "SubmitVotes")]
    public async Task<IActionResult> Get(int pollId)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(AddressService.CounterAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        var votes = await _voteData.GetVotesByPoll(pollId);

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync($"/vote/?usageKey={_config.GetAPIUsageKey()}", votes);
        }
        catch (Exception)
        {
            return BadRequest();
        }

        return Ok();
        
    }

    [HttpPost(Name = "ValidateVotes")]
    public async Task<IActionResult> Post(int pollId)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(AddressService.CounterAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


        var validations = await _voteData.GetValidationsByPoll(pollId);

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync($"/validate/?usageKey={_config.GetAPIUsageKey()}", validations);
        }
        catch (Exception)
        {
            return BadRequest();
        }

        return Ok();

    }
}
