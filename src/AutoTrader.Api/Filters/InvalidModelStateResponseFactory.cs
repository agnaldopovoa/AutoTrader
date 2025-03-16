using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Mime;
using AutoTrader.Abstractions;

namespace AutoTrader.Api.Filters;

public static class InvalidModelStateResponseFactory
{
    /// <summary>
    /// Tratar todas as requisições que retornam InvalidModelStateResponse 
    /// e transforma em uma resposta padrão com a classe Response.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IActionResult Handle(ActionContext context)
    {
        var response = new Response(ResponseStatus.BadRequest, "A requisição tem alguns dados inválidos.")
        {
            Errors = GetErrorsFromModelState(context.ModelState)
        };
        var result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        return result;
    }

    /// <summary>
    /// Transforma os erros do modelState em um array de Error.
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    private static Error[] GetErrorsFromModelState(ModelStateDictionary modelState)
    {
        return [.. modelState
            .SelectMany(modelState => GetErrorsFromModelStateEntry(modelState))];
    }

    /// <summary>
    /// Transforma os erros do modelState em um array de Error.
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    private static IEnumerable<Error> GetErrorsFromModelStateEntry(KeyValuePair<string, ModelStateEntry?> modelState)
    {
        var property = string.Concat(modelState.Key[..1].ToLowerInvariant(), modelState.Key.AsSpan(1));
        foreach (var error in modelState.Value!.Errors)
        {
            yield return new Error(property, error.ErrorMessage);
        }
    }
}
