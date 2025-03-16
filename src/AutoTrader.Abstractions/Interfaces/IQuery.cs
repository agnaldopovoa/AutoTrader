using System;
using MediatR;

namespace AutoTrader.Abstractions;

public interface IQuery<TResult> : IRequest<Response<TResult>> where TResult : notnull {}
