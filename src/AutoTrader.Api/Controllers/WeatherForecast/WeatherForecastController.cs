using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoTrader.Application.WeatherForecast.Queries;

namespace AutoTrader.API.Controllers.WeatherForecast;

[ApiController]
[Route("weatherforecast")]
public class WeatherForecastController(IMediator _mediator) : ControllerBase
{
    private readonly IMediator mediator = _mediator;

    [HttpGet]
    public async Task<IActionResult> GetWeatherForecast()
    {
        ListWeatherForecastsQuery query = new();
        return Ok(await mediator.Send(query));
    }
}
