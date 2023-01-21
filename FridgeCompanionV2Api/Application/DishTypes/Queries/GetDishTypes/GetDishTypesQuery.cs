﻿using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.CuisineTypes.Queries.GetCuisineTypes
{
    public class GetDishTypesQuery : IRequest<List<DishTypeDto>>
    {
        public string UserId { get; set; }
    }
}
