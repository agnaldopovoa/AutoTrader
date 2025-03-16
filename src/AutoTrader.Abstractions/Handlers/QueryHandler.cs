using MediatR;

namespace AutoTrader.Abstractions.Handlers;

public abstract class QueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Response<TResult>>
    where TQuery : IQuery<TResult>
    where TResult : notnull
{
    protected abstract Task<TResult> Execute(TQuery request, CancellationToken cancellationToken);

    public async Task<Response<TResult>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var result = await Execute(request, cancellationToken);
        var response = result == null ?
                      new Response<TResult>(ResponseStatus.NoContent, "Registro não encontrado.") :
                      new Response<TResult> { Status = ResponseStatus.Ok, Result = result };
        return response;
    }
}
