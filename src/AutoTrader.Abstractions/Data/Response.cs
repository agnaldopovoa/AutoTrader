using System;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace AutoTrader.Abstractions;

public class Response
{

    public ResponseStatus Status { get; set; } = ResponseStatus.Ok;
    public string Message { get; set; } = "Operação realizada com sucesso.";
    public Error[] Errors { get; set; } = Array.Empty<Error>();

    [JsonIgnore]
    public object Result { get; set; }  = new object();

    [JsonIgnore]
    public bool IsSuccess => (Errors == null || Errors.Length == 0) &&
                             Status == ResponseStatus.Ok;
    public string TraceId { get; set; }

    public Response()
    {
        TraceId = "Elastic.Apm.Agent.Tracer?.CurrentTransaction?.TraceId";
    }
    public Response(object result)
        : this()
    {
        Result = result;
    }
    public Response(Exception exception)
        : this()
    {
        Status = ResponseStatus.InternalServerError;
        Message = exception.Message;
    }
    public Response(ValidationException validationException)
        : this()
    {
        Status = ResponseStatus.BadRequest;
        Message = validationException.Message;
        Errors = [ new Error(validationException) ];
    }
    public Response(IEnumerable<ValidationFailure> validationFailures)
        : this()
    {
        Status = ResponseStatus.BadRequest;
        Message = "Erro na validação da requisição.";
        Errors = validationFailures.Select(validationFailure => new Error(validationFailure)).ToArray();
    }
    public Response(ResponseStatus responseStatus, string message)
        : this()
    {
        Status = responseStatus;
        Message = message;
    }
}

public class Response<TResult> : Response where TResult : notnull
{
    public Response() { }
    public Response(TResult result) : base(result) { }
    public Response(ValidationException validationException) : base(validationException) { }
    public Response(IEnumerable<ValidationFailure> validationFailures) : base(validationFailures) { }
    public Response(Exception exception) : base(exception) { }
    public Response(ResponseStatus responseStatus, string message) : base(responseStatus, message) { }
    public new TResult Result
    {
        get => base.Result != null ? (TResult)base.Result : default!;
        set => base.Result = value;
    }
}
