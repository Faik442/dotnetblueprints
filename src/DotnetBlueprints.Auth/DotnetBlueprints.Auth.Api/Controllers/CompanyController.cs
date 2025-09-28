using DotnetBlueprints.Auth.Application.Features.Company.Commands.CreateCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Commands.UpdateCompany;
using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanies;
using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;
using DotnetBlueprints.Auth.Application.Features.Role.Commands.CreateRole;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [RequirePermission("Company.Create")]
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

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<Domain.Entities.Company>))]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Domain.Entities.Company>>> GetCompanies([FromBody] GetCompaniesQuery query)
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
