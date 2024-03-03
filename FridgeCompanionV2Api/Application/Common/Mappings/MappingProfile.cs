using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FridgeCompanionV2Api.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
            CreateMap<ShoppingList, ShoppingListDto>();
            CreateMap<ShoppingListItem, ShoppingItemDto>()
                .ForMember(dest => dest.Measurement, opt => opt.MapFrom(src => src.Measurement.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Ingredient.Name));
            CreateMap<ShoppingListRecipeItem, ShoppingListRecipeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Recipe.Name));

            CreateMap<MeasurementType, MeasurementTypeDto>();
            CreateMap<IngredientMeasurement, IngredientMeasurementTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Measurement.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Measurement.Name));
            CreateMap<IngredientLocation, IngredientLocationDto>();
            CreateMap<IngredientGroupType, IngredientGroupTypeDto>();
            CreateMap<Ingredient, IngredientDto>();
            CreateMap<CuisineType, CuisineTypeDto>();
            CreateMap<RecipeIngredient, RecipeIngredientDto>();
            CreateMap<RecipeStep, RecipeStepDto>();
            CreateMap<RecipeCuisine, RecipeCuisineDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Cuisine.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Cuisine.Name));
            CreateMap<RecipeDish, RecipeDishDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Dish.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dish.Name));
            CreateMap<IngredientDiet, IngredientDietDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Diet.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Diet.Name));
            CreateMap<IngredientType, IngredientTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IngredientGroupType.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.IngredientGroupType.Name));
            CreateMap<DishType, DishTypeDto>();
            CreateMap<DietType, DietTypeDto>();
            CreateMap<Recipe, RecipeDto>();
            CreateMap<FridgeItem, FridgeItemDto>();
            CreateMap<UserDiets, UserDietsDto>();
            CreateMap<UserFavouriteRecipes, UserFavouriteRecipesDto>();
            CreateMap<UserMadeRecipes, UserMadeRecipesDto>();
            CreateMap<Domain.Entities.User, UserDto>();

        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") 
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
                
                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}