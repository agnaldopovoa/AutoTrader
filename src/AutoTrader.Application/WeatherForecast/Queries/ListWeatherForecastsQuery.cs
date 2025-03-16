using System;
using MediatR;
using AutoTrader.Application.WeatherForecast.Data;
using AutoTrader.Abstractions;

namespace AutoTrader.Application.WeatherForecast.Queries;

public class ListWeatherForecastsQuery: IQuery<IEnumerable<ListWeatherForecastsResponse>>{}
