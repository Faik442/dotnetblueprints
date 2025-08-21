using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Domain.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedOn { get; set; }

    public OutboxMessage(string type, string content)
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        Type = type;
        Content = content;
        Processed = false;
    }
}
