using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanBarcode
{
    public class ScanBarcodeQueryValidator : AbstractValidator<ScanBarcodeQuery>
    {
        public ScanBarcodeQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.EAN)
                .NotNull().WithMessage("Please supply a EAN Barcode.");
        }
    }
}
