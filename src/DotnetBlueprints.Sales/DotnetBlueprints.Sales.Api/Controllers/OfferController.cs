using DotnetBlueprints.Sales.Application.Features.Offers.Commands.Create;
using DotnetBlueprints.Sales.Application.Features.Offers.Commands.Update;
using DotnetBlueprints.Sales.Application.Features.Offers.Commands.UpdateStatus;
using DotnetBlueprints.Sales.Application.Offers.Commands.Delete;
using DotnetBlueprints.Sales.Application.Offers.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBlueprints.Sales.Api.Controllers;

/// <summary>
/// Controller for managing offers.
/// </summary>
[ApiController]
[Route("api/offer")]
public class OfferController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfferController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new offer.
    /// </summary>
    /// <param name="command">Offer creation request.</param>
    /// <returns>The unique identifier (Guid) of the created offer.</returns>
    /// <response code="200">The offer has been created successfully.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("create")]
    public async Task<ActionResult<Guid>> CreateOffer([FromBody] CreateOfferCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing offer with the specified information.
    /// </summary>
    /// <param name="id">The unique identifier of the offer to update.</param>
    /// <param name="command">The update offer command containing updated values.</param>
    /// <returns>No content if the update is successful; 404 if the offer does not exist.</returns>
    /// <response code="204">Offer updated successfully.</response>
    /// <response code="404">Offer not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOffer([FromRoute] Guid id, [FromBody] UpdateOfferCommand command)
    {
        if (id != command.OfferId)
            return BadRequest("ID in URL does not match ID in body.");

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Updates the status of an existing offer.
    /// </summary>
    /// <param name="command">The update status command containing offer ID, new status, and changed by information.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Status updated successfully.</response>
    /// <response code="404">Offer not found.</response>
    /// <response code="400">Invalid status transition.</response>
    /// <response code="500">Unexpected server error.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("status")]
    public async Task<IActionResult> UpdateOfferStatus([FromBody] UpdateOfferStatusCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Soft deletes an offer by its ID.
    /// </summary>
    /// <param name="id">The ID of the offer to delete.</param>
    /// <returns>NoContent if successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteOfferCommand { OfferId = id });
        return NoContent();
    }

    /// <summary>
    /// Retrieves an offer by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the offer.</param>
    /// <returns>The offer details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetOfferByIdQuery(id));
        return Ok(result);
    }

}
