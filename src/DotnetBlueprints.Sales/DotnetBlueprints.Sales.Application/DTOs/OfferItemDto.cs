using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.DTOs;

public class OfferItemDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
