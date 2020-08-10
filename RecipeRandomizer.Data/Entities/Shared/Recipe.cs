using System;
using System.Collections.Generic;
using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Entities.Shared
{
    public class Recipe
    {
        public Recipe()
        {
            Ingredients = new HashSet<Ingredient>();
            RecipeTagAssociations = new HashSet<RecipeTagAssociation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public int NumberOfPeople { get; set; }
        public int CostId { get; set; }
        public int DifficultyId { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public string Preparation { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<RecipeTagAssociation> RecipeTagAssociations { get; set; }
        public virtual Cost Cost { get; set; }
        public virtual Difficulty Difficulty { get; set; }
    }
}
