using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IQuantityService
    {
        public IEnumerable<QuantityUnit> GetQuantities();
    }
}
