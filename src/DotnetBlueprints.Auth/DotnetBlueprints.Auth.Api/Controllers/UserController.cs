using DotnetBlueprints.Auth.Application.Features.Company.Commands.CreateCompany;
using DotnetBlueprints.Auth.Application.Features.User.Commands.AddUserToCompany;
using DotnetBlueprints.Auth.Application.Features.User.Commands.AssignRoleToUserInCompany;
using DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveRoleFromUserInCompany;
using DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveUserFromCompany;
using DotnetBlueprints.Auth.Application.Features.User.Commands.UpdateUserProfile;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBlueprints.Auth.Api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddUserToCompany([FromBody] AddUserToCompanyCommand command)
    {
        return await _mediator.Send(command);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch]
    public async Task<ActionResult> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<ActionResult> RemoveUserFromCompany([FromBody] RemoveUserFromCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("role")]
    public async Task<ActionResult> AssignRoleToUserInCompany([FromBody] AssignRoleToUserInCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("role")]
    public async Task<ActionResult> RemoveRoleFromUserInCompany([FromBody] RemoveRoleFromUserInCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
