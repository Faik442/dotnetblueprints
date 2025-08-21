using System;

namespace DotnetBlueprints.SharedKernel.Audit;

/// <summary>
/// Indicates that the decorated property should be tracked in audit logs.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AuditableAttribute : Attribute
{

}
