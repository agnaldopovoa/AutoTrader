using System;
using AutoTrader.Abstractions.Handlers;
using AutoTrader.Application.WeatherForecast.Queries;
using AutoTrader.Application.WeatherForecast.Data;

namespace AutoTrader.Infrastructure.WeatherForecast;

public class ListWeatherForecastsRepository
    : QueryHandler<ListWeatherForecastsQuery, IEnumerable<ListWeatherForecastsResponse>>
{
    private static readonly string[] summaries =
    [
        "Freezing", "Cool", "Mild", "Warm", "Scorching"
    ];

    protected async override Task<IEnumerable<ListWeatherForecastsResponse>> Execute(
        ListWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        var forecasts = await Task.Run(
            () => Enumerable.Range(1,Random.Shared.Next(3, 7)).Select(
                    index => {
                        int temp = Random.Shared.Next(19, 38);
                        return new ListWeatherForecastsResponse()
                        {
                            Day = DateTime.Now.Date.ToString(),
                            Temperature = temp,
                            Description = summaries[(int)((temp-19)/4)]
                        };
                    }
                  ).ToArray()
        );

        return forecasts;
    }
}
