using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.CuisineTypes.Queries.GetCuisineTypes
{
    public class GetDietTypesQuery : IRequest<List<DietTypeDto>>
    {
        public string UserId { get; set; }
    }
}
