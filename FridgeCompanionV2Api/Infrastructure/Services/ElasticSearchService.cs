using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Common.Options;
using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Infrastructure.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticClient _elasticClient;
        private const string RECIPE_INDEX = "recipes";
        private const string INGREDIENT_INDEX = "ingredients";

        public ElasticSearchService(IOptions<ElasticOptions> elastic)
        {
            var uri = new Uri(elastic.Value.Url);
            var settings = new ConnectionSettings(uri).EnableApiVersioningHeader();
            _elasticClient = new ElasticClient(settings);
            //if (_elasticClient.Indices.Exists(RECIPE_INDEX).Exists)
            //{
                _elasticClient.Indices.Create(RECIPE_INDEX, x => x.Map<ElasticModelRequest>(m => m
                    .AutoMap()
                    .Properties(ps => ps
                        .Completion(c => c
                            .Name(p => p.Suggest)))));
            //}

            //if (_elasticClient.Indices.Exists(INGREDIENT_INDEX).Exists)
            //{
            var createIndexResponse = _elasticClient.Indices.Create(INGREDIENT_INDEX, c => c
    .Map<ElasticModelRequest>(m => m
        .AutoMap()
        .Properties(ps => ps
            .Text(t => t
                .Name(p => p.Name)
                .Analyzer("autocomplete")
            )
        )
    )
    .Settings(s => s
        .Analysis(a => a
            .TokenFilters(tf => tf.EdgeNGram("autocomplete", e => e
                .MinGram(1)
                .MaxGram(20)
            ))
            .Analyzers(an => an.Custom("autocomplete", ca => ca
                .Tokenizer("standard")
                .Filters("lowercase", "autocomplete")
            ))
        )
    )
);
            //}
        }

        public async Task BulkIndexRecipesAsync(List<Recipe> recipes)
        {
            var bulkIndexResponse = await _elasticClient.IndexManyAsync(recipes.Select(x => new ElasticModelRequest { Id = x.Id, Name = x.Name }), RECIPE_INDEX);

            if (!bulkIndexResponse.IsValid)
            {
                // Handle error, log, etc.
                throw new Exception($"Failed to bulk index recipes: {bulkIndexResponse.DebugInformation}");
            }
        }

        public async Task BulkIndexIngredientsAsync(List<Ingredient> ingredients)
        {
            var bulkIndexResponse = await _elasticClient.IndexManyAsync(ingredients.Select(x => new ElasticModelRequest { Id = x.Id, Name = x.Name }), INGREDIENT_INDEX);

            if (!bulkIndexResponse.IsValid)
            {
                // Handle error, log, etc.
                throw new Exception($"Failed to bulk index ingredients: {bulkIndexResponse.DebugInformation}");
            }
        }


        public async Task<List<ElasticModelRequest>> SearchByRecipeNameAsync(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticModelRequest>(s => s
                .Index(RECIPE_INDEX)
                .Suggest(su => su
                    .Completion("suggestions", c => c
                        .Field(f => f.Suggest)
                        .Prefix(query)
                        .Fuzzy(f => f
                            .Fuzziness(Fuzziness.Auto)
                        )
                        .Size(5))
                        ));


            return searchResponse.Documents.ToList();
        }

        public async Task<List<ElasticModelRequest>> SearchByIngredientNameAsync(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<ElasticModelRequest>(s => s
                .Index(INGREDIENT_INDEX)
                .Query(q => q
        .Prefix(p => p
            .Field(f => f.Name)
            .Value(query.ToLowerInvariant()) // Ensure consistent case for prefix matching
        )
    )
                .Size(5)
            );



            if (!searchResponse.IsValid)
            {
                // Handle error, log, etc.
                throw new Exception($"Failed to bulk index recipes: {searchResponse.DebugInformation}");
            }

            return searchResponse.Documents.ToList();
        }
    }
}
