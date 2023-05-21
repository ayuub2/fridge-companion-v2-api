using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.HttpClients;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Receipt.Queries.ScanReceipt;
using FuzzySharp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanBarcode
{
    public class ScanBarcodeQueryHandler : IRequestHandler<ScanBarcodeQuery, ScanBarcodeDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBarcodeClient _barcodeClient;

        public ScanBarcodeQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<ScanReceiptQueryHandler> logger, IBarcodeClient barcodeClient)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _barcodeClient = barcodeClient ?? throw new ArgumentNullException(nameof(barcodeClient));
        }

        public async Task<ScanBarcodeDto> Handle(ScanBarcodeQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var ingredients = _applicationDbContext.Ingredients
                .Include(x => x.DietTypes)
                    .ThenInclude(idt => idt.Diet)
                .Include(x => x.Location)
                .Include(x => x.GroupTypes)
                    .ThenInclude(idt => idt.IngredientGroupType)
                .Include(x => x.MeasurementTypes)
                    .ThenInclude(idt => idt.Measurement)
                .Where(x => !x.IsDeleted).AsNoTracking().ToList();

            var productName = await _barcodeClient.GetItemName(request.EAN);

            var results = Process.ExtractOne(productName, ingredients
                        .Select(x => x.Name).ToArray(), cutoff: 85);
            if (results != null)
            {
                var ingredient = ingredients.ElementAt(results.Index);
                return new ScanBarcodeDto() { Ingredient = _mapper.Map<IngredientDto>(ingredient) };
            }

            throw new NotFoundException();
        }
    }
}
