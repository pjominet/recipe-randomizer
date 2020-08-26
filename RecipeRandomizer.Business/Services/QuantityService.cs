using System.Collections.Generic;
using AutoMapper;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Nomenclature;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Business.Services
{
    public class QuantityService : IQuantityService
    {

        private readonly QuantityRepository _quantityRepository;
        private readonly IMapper _mapper;

        public QuantityService(RRContext context, IMapper mapper)
        {
            _quantityRepository = new QuantityRepository(context);
            _mapper = mapper;
        }

        public IEnumerable<QuantityUnit> GetQuantities()
        {
            return _mapper.Map<IEnumerable<QuantityUnit>>(_quantityRepository.GetAll<Entities.QuantityUnit>());
        }
    }
}
