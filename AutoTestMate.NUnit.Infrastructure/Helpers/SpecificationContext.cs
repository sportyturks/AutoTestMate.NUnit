using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.Helpers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public abstract class SpecificationContext
    {
        [SetUp]
        public virtual void Init()
        {
            Given();
            When();
        }
        public abstract void Given();
        public abstract void When();
    }
}
