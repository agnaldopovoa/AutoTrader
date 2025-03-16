using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;

namespace AutoTrader.Abstractions;

public class Error
{
    public string? Property { get; set; }
    public string? Message { get; set; }

    public Error()
    {
    }

    public Error(ValidationException validationException)
    {
        Property = validationException.ValidationResult.MemberNames?.FirstOrDefault();
        Message = validationException.ValidationResult.ErrorMessage;
    }

    public Error(ValidationFailure validationFailure)
    {
        Property = validationFailure.PropertyName;
        Message = validationFailure.ErrorMessage;
    }

    public Error(string property, string message)
    {
        Property = property;
        Message = message;
    }
}