using Castle.Windsor;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
	public interface ITestManager
	{
        WindsorContainer Container { get; }
        TestParameters TestParameters { get; }
		IConfigurationReader ConfigurationReader { get; }
        IConfiguration AppConfiguration { get; }
		ILoggingUtility LoggingUtility { get; }
		void OnInitialiseAssemblyDependencies(TestParameters testParameters = null);
		void OnDisposeAssemblyDependencies();
        void OnTestMethodInitialise(TestParameters testParameters = null);
		void OnTestCleanup();
		void InitialiseIoc();
		void InitialiseTestContext(TestParameters testParameters = null);
		void InitialiseTestContextDependencies();
        void Dispose();
        void DisposeInternal();
		void UpdateConfigurationReader(IConfigurationReader configurationReader);
		void SetTestParameters(TestParameters testParameters);
	}
}