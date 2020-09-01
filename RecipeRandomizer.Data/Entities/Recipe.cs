using System;
using System.Collections;
using System.Collections.Generic;
using RecipeRandomizer.Data.Entities.Identity;
using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Entities
{
    public class Recipe
    {
        public Recipe()
        {
            Ingredients = new HashSet<Ingredient>();
            RecipeTagAssociations = new HashSet<RecipeTagAssociation>();
            RecipeLikes = new HashSet<RecipeLike>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public int NumberOfPeople { get; set; }
        public int CostId { get; set; }
        public int DifficultyId { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public string Preparation { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted => DeletedOn != null;

        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<RecipeTagAssociation> RecipeTagAssociations { get; set; }
        public virtual ICollection<RecipeLike> RecipeLikes { get; set; }
        public virtual Cost Cost { get; set; }
        public virtual Difficulty Difficulty { get; set; }
        public virtual User User { get; set; }
    }
}
