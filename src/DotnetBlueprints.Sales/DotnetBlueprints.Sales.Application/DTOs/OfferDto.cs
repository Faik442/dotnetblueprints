using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.DTOs;

public class OfferDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime ValidUntil { get; set; }
    public required string Status { get; set; }
    public List<OfferItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
