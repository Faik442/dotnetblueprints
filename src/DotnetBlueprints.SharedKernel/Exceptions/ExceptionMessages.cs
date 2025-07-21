using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Exceptions;

/// <summary>
/// Centralized error messages for common exceptions.
/// </summary>
public static class ExceptionMessages
{
    public const string EntityNotFound = "Entity \"{0}\" ({1}) was not found.";

    public static string FieldRequired(string fieldName) =>
        $"The field \"{fieldName}\" is required.";

    public static string InvalidFormat(string fieldName) =>
        $"The value for \"{fieldName}\" is not in a valid format.";

    public static string ValueOutOfRange(string fieldName, object min, object max) =>
        $"The value for \"{fieldName}\" must be between {min} and {max}.";

    public static string InvalidEnumValue(string fieldName) =>
        $"The value of \"{fieldName}\" is not a valid option.";
}
