using Apsitvarkom.Models.Mapping;
using AutoMapper;

namespace Apsitvarkom.UnitTests.Models.Mapping;

public class TidyingEventMappingTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TidyingEventProfile>();
        });

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
    }

    #region Request mappings

    #endregion

    #region Response mappings

    #endregion
}