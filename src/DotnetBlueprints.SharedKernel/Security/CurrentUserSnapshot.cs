using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Security;

public sealed record CurrentUserSnapshot(
    Guid UserId,
    string? DisplayName,
    string? Email,
    Guid CompanyId,
    IReadOnlyCollection<Guid> RoleIds);
