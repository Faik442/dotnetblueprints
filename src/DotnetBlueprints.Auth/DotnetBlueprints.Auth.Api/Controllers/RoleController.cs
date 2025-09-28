using DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Commands.UpdateCompany;
using DotnetBlueprints.Auth.Application.Features.Role.Commands.CreateRole;
using DotnetBlueprints.Auth.Application.Features.Role.Commands.SetRolePermission;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBlueprints.Auth.Api.Controllers;

[Route("api/role")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [RequirePermission("Role.Create")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateRole([FromBody] CreateRoleCommand command)
    {
        return await _mediator.Send(command);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch]
    public async Task<ActionResult> UpdateRole([FromBody] UpdateCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<ActionResult> DeleteRole([FromBody] DeleteCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("set-permission")]
    public async Task<ActionResult> SetPermission([FromBody] SetRolePermissionsCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
