using AutoMapper;
using NUnit.Framework;

namespace RecipeRandomizer.Tests
{
    [TestFixture]
    public class AutoMapperTests
    {
        private MapperConfiguration _mapperConf;

        [SetUp]
        public void Setup()
        {
            _mapperConf = new MapperConfiguration(c => c.AddMaps($"{nameof(RecipeRandomizer)}.{nameof(Business)}"));
        }

        [Test]
        public void TestMappingProfiles()
        {
            _mapperConf.AssertConfigurationIsValid();
        }
    }
}
