using NetFusion.Base.Validation;
using NetFusion.Bootstrap.Validation;

namespace Boondocks.Auth.Tests.Mocks
{
    /// <summary>
    /// Mock validation service based on MS Validation Attributes.
    /// </summary>
    public class MockValidationService : IValidationService
    {
        public ValidationResultSet Validate(object obj)
        {
            var validator = new MSObjectValidator(obj);
            return validator.Validate();
        }
    }
}
