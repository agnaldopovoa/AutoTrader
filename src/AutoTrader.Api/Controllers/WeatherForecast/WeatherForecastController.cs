using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoTrader.Application.WeatherForecast.Queries;

namespace AutoTrader.API.Controllers.WeatherForecast;

[ApiController]
[Route("weatherforecast")]
public class WeatherForecastController(IMediator _mediator, ILogger<WeatherForecastController> _logger) : ControllerBase
{
    private readonly IMediator mediator = _mediator;
    private readonly ILogger<WeatherForecastController> logger = _logger;

    [HttpGet]
    public async Task<IActionResult> GetWeatherForecast()
    {
        logger.LogInformation("GetWeatherForecast called");
        ListWeatherForecastsQuery query = new();
        return Ok(await mediator.Send(query));
    }
}
