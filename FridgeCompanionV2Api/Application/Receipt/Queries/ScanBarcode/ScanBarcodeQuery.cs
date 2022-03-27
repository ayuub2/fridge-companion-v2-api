using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanBarcode
{
    public class ScanBarcodeQuery : IRequest<ScanBarcodeDto>
    {
        public string UserId { get; set; }
        public string EAN { get; set; }
    }
}
