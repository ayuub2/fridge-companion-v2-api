using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Textract;
using Amazon.Textract.Model;
using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Common.Options;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzySharp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Receipt.Queries.ScanReceipt
{
    public class ScanReceiptQueryHandler : IRequestHandler<ScanReceiptQuery, ScanReceiptDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IOptions<AwsOptions> _awsOptions;

        public ScanReceiptQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<ScanReceiptQueryHandler> logger, IOptions<AwsOptions> awsOptions)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _awsOptions = awsOptions ?? throw new ArgumentNullException(nameof(awsOptions));
        }

        public async Task<ScanReceiptDto> Handle(ScanReceiptQuery request, CancellationToken cancellationToken)
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
                .Where(x => !x.IsDeleted).AsNoTracking().ToList();

            var credentials = new BasicAWSCredentials(_awsOptions.Value.AccessKey, _awsOptions.Value.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.EUWest1
            };
            using var client = new AmazonS3Client(credentials, config);
            await using var newMemoryStream = new MemoryStream();
            request.Image.CopyTo(newMemoryStream);

            var fileName = Guid.NewGuid().ToString();
            var bucketName = _awsOptions.Value.BucketName;
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = fileName,
                BucketName = bucketName
            };

            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            using var textExtractClient = new AmazonTextractClient(credentials, RegionEndpoint.EUWest1);
            var textRequest = new DetectDocumentTextRequest()
            {
                Document = new Document() 
                {
                    S3Object = new S3Object()
                    {
                        Bucket = bucketName,
                        Name = fileName
                    }
                },
            };
            var response = await textExtractClient.DetectDocumentTextAsync(textRequest);
            var ingredientList = new List<Ingredient>();
            foreach (var block in response.Blocks)
            {
                if(block.BlockType == BlockType.LINE) 
                {
                    var results = Process.ExtractOne(block.Text, ingredients
                        .Select(x => x.Name).ToArray(), cutoff: 85);
                    if(results != null) 
                    {
                        ingredientList.Add(ingredients.ElementAt(results.Index));
                    }
                }
            }
            return new ScanReceiptDto() { Ingredients = _mapper.Map<List<IngredientDto>>(ingredientList) };

        }
    }
}
