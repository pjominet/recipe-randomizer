using System;
using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public int NumberOfPeople { get; set; }
        public Cost Cost { get; set; }
        public Difficulty Difficulty { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public string Preparation { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }

        public IList<Ingredient> Ingredients { get; set; }
        public IList<Tag> Tags { get; set; }
        public string CreatedBy { get; set; }
        public int Likes { get; set; }
    }
}
