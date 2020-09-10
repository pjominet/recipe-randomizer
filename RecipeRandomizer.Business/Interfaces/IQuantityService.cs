using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IQuantityService
    {
        public Task<IEnumerable<QuantityUnit>> GetQuantityUnitsAsync();
    }
}
