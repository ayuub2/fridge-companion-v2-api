﻿using FluentValidation;
using System;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanReceipt
{
    public class ScanReceiptQueryValidator : AbstractValidator<ScanReceiptQuery>
    {
        public ScanReceiptQueryValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("Please upload an image.");
            RuleFor(x => x.Image)
                .NotNull()
                .Must(x => string.Equals(x?.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(x?.ContentType, "image/png", StringComparison.OrdinalIgnoreCase)).WithMessage("Please upload an image.");
        }
    }
}
