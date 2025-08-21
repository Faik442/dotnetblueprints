using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Domain.Enums;

public record PermissionKey(string Value)
{
    public static readonly PermissionKey OffersReadSelf = new("Offers.Read.Self");
    public static readonly PermissionKey OffersReadCompany = new("Offers.Read.Company");
    public static readonly PermissionKey OffersReadAll = new("Offers.Read.All");

    public static readonly PermissionKey OffersWriteSelf = new("Offers.Write.Self");
    public static readonly PermissionKey OffersWriteCompany = new("Offers.Write.Company");
    public static readonly PermissionKey OffersWriteAll = new("Offers.Write.All");
}

