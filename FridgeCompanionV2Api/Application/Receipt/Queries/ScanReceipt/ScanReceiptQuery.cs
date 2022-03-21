using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanReceipt
{
    public class ScanReceiptQuery : IRequest<ScanReceiptDto>
    {
        public string UserId { get; set; }
        public IFormFile Image { get; set; }
    }
}
