using DotnetBlueprints.Sales.Application.Features.OfferItem.Create;
using DotnetBlueprints.Sales.Application.Features.OfferItem.Delete;
using DotnetBlueprints.Sales.Application.Features.OfferItem.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBlueprints.Sales.Api.Controllers;

/// <summary>
/// Controller for managing offer items.
/// </summary>
[ApiController]
[Route("api/offer-items")]
public class OfferItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfferItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new offer item and adds it to the specified offer.
    /// </summary>
    /// <param name="command">The data for the offer item to be created.</param>
    /// <returns>The unique identifier of the created offer item.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Guid> Create([FromBody] CreateOfferItemCommand command)
    {
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Updates an existing offer item.
    /// </summary>
    /// <param name="command">The update offer item command.</param>
    /// <returns>No content if update is successful.</returns>
    [HttpPut("{offerItemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferItemCommand command)
    {
        if (id != command.OfferItemId)
            return BadRequest("Route id and payload id do not match.");

        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Soft-deletes an offer item by marking it as deleted and recording deletion details.
    /// </summary>
    /// <param name="id">The unique identifier of the offer item to be deleted.</param>
    /// <param name="deletedBy">The user or process who performed the deletion.</param>
    /// <returns>No content if the deletion was successful; 404 if not found.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string deletedBy)
    {
        await _mediator.Send(new DeleteOfferItemCommand
        {
            OfferItemId = id,
            DeletedBy = deletedBy
        });
        return NoContent();
    }
}
