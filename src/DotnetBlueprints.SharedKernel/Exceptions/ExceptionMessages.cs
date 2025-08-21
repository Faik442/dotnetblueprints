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
    public static string EntityNotFound(string entity, object value) =>
        $"Entity \"{entity}\" ({value}) was not found.";

    public static string FieldRequired(string fieldName) =>
        $"The field \"{fieldName}\" is required.";

    public static string InvalidEnumValue(string fieldName) =>
        $"The value of \"{fieldName}\" is not a valid option.";

    public static string ValueAlreadySet(string entityName, object value) =>
        $"The {entityName} already has the '{value}'. No update is necessary.";
}
