using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Permission.Query;

public sealed record PermissionDto(
    Guid Id,
    string Key,
    string Description
);
