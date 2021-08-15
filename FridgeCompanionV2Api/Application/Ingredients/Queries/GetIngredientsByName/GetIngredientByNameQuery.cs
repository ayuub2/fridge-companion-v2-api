using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetIngredientsByName
{
    public class GetIngredientByNameQuery : IRequest<List<IngredientDto>>
    {
        public string UserId { get; set; }
        public List<string> IngredientNames { get; set; }
    }
}
