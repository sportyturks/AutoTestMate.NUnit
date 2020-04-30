using System.Collections.Specialized;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
    public interface IConfiguration
    {
        NameValueCollection Settings { get; set; }
    }
}


