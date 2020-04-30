using System.Collections.Specialized;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
    public class AppConfiguration : IConfiguration
    {
        public AppConfiguration()
        {
            //Settings = ConfigurationManager.AppSettings;
            Settings = new NameValueCollection();
        }
        public NameValueCollection Settings { get; set; }
    }
}
