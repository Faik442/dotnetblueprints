using DotnetBlueprints.Auth.Application.Features.Company.Commands.CreateCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Commands.UpdateCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanies;
using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;
using DotnetBlueprints.Auth.Application.Features.Role.Commands.CreateRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBlueprints.Auth.Api.Controllers;

[Route("api/company")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //[Authorize]
    //[RequirePermission("Company.Create")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCompany([FromBody] CreateCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch]
    public async Task<ActionResult> UpdateCompany([FromBody] UpdateCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<ActionResult> DeleteCompany([FromBody] DeleteCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("company-role")]
    public async Task<ActionResult<Guid>> CreateRole([FromBody] CreateRoleCommand command)
    {
        return await _mediator.Send(command);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("company-role")]
    public async Task<ActionResult> UpdateRole([FromBody] UpdateCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("company-role")]
    public async Task<ActionResult> DeleteRole([FromBody] DeleteCompanyCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<CompanyDto>))]
    [HttpGet]
    public async Task<ActionResult<CompanyDto>> GetCompanies([FromBody] GetCompaniesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<CompanyDto>))]
    [HttpGet("id")]
    public async Task<ActionResult<CompanyDto>> GetCompanyById([FromBody] GetCompanyByIdQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
