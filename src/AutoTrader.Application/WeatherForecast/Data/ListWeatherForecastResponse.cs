using System;

namespace AutoTrader.Application.WeatherForecast.Data;

public class ListWeatherForecastsResponse
{
    public required string Day{get; set;}
    public int Temperature{get; set;}
    public required string Description{get; set;}
}