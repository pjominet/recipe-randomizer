using System;
using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Models.Shared
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public int NumberOfPeople { get; set; }
        public Cost Cost { get; set; }
        public Difficulty Difficulty { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public string Preparation { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public IList<Ingredient> Ingredients { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}
