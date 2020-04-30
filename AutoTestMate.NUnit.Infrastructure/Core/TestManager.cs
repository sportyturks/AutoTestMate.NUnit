using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
	public class TestManager : ITestManager, IDisposable
	{
		#region Private Variables

		private static TestManager _uniqueInstance;
		private static readonly object SyncLock = new Object();

		#endregion

		#region Constructor

		protected TestManager() { }

		#endregion

		#region Properties
		public static TestManager Instance()
		{
			// Lock entire body of method
			lock (SyncLock)
			{
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_uniqueInstance == null)
				{
					_uniqueInstance = new TestManager();
				}

				return _uniqueInstance;
			}
		}
		public bool IsInitialised { get; set; }
        public WindsorContainer Container { get; set; }
		public ILoggingUtility LoggingUtility => Container.Resolve<ILoggingUtility>();
		public IConfigurationReader ConfigurationReader => Container.Resolve<IConfigurationReader>();
        public IConfiguration AppConfiguration => Container.Resolve<IConfiguration>();
        public TestParameters TestParameters => Container.Resolve<TestParameters>();

		#endregion

		#region Public Methods
        
        public virtual void OnInitialiseAssemblyDependencies(TestParameters testParameters = null)
        {
            InitialiseIoc();
            InitialiseTestContext(testParameters);
            InitialiseTestContextDependencies();
        }
        public virtual void OnDisposeAssemblyDependencies()
        {
            Dispose();
            Container.Dispose();
        }

        public virtual void OnTestMethodInitialise(TestParameters testParameters = null)
        {
            if (IsInitialised) throw new ApplicationException(Exceptions.Exception.ExceptionMsgSingletonAlreadyInitialised);

            try
            {
                InitialiseTestContext(testParameters);
                IsInitialised = true;
            }
            catch (Exception exp)
            {
                LoggingUtility.Error(exp.Message);
                Dispose();
                throw;
            }
        }

        public virtual void OnTestCleanup()
        {
            Dispose();
        }

        public virtual void InitialiseTestContext(TestParameters testParameters = null)
        {
            if (testParameters == null || testParameters.Count == 0) return;

            Container.Register(Component.For<TestParameters>().Instance(testParameters).OverridesExistingRegistration());
			Container.Register(Component.For<IConfigurationReader>().ImplementedBy<ConfigurationReader>().OverridesExistingRegistration());
		}

		public virtual void InitialiseIoc()
        {
            var container = new WindsorContainer();

            container.Register(Component.For<ILoggingUtility>().ImplementedBy<TestLogger>().LifestyleSingleton())
                .Register(Component.For<IConfigurationReader>().ImplementedBy<ConfigurationReader>())
                .Register(Component.For<IConfiguration>().ImplementedBy<AppConfiguration>().LifestyleSingleton())
                .Register(Component.For<IMemoryCache>().ImplementedBy<MemoryCache>().LifestyleSingleton())
                .Register(Component.For<ITestManager>().Instance(this).OverridesExistingRegistration().LifeStyle.Singleton);

            Container = container;
        }
		public virtual void InitialiseTestContextDependencies()
        {

        }
	    public virtual void UpdateConfigurationReader(IConfigurationReader configurationReader)
		{
			if (configurationReader != null)
			{
				Container.Register(Component.For<IConfigurationReader>().Instance(configurationReader).OverridesExistingRegistration());
			}
			else //Ensure ConfigurationReader is resolved from existing container dependencies 
			{
				Container.Register(Component.For<IConfigurationReader>().ImplementedBy<ConfigurationReader>().OverridesExistingRegistration());
			}
		}
		public virtual void Dispose()
        {
            DisposeInternal();
        }
        public virtual void DisposeInternal()
		{
			IsInitialised = false;
		}

        public virtual void SetTestParameters(TestParameters testParameters)
        {
	        Container.Register(Component.For<TestParameters>().Instance(testParameters).OverridesExistingRegistration());
        }

        #endregion
	}
}
