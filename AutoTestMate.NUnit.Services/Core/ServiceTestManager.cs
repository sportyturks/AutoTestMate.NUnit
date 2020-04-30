using System;
using System.Net;
using System.Net.Http;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Infrastructure.Core;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Services.Core
{
    public class ServiceTestManager : TestManager
    {
        #region Private Variables

        private static ServiceTestManager _uniqueInstance;
        private static readonly object SyncLock = new Object();

        #endregion
        
        #region Properties

        public new static ServiceTestManager Instance()
        {
            // Lock entire body of method
            lock (SyncLock)
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_uniqueInstance == null)
                {
                    _uniqueInstance = new ServiceTestManager();
                }
                return _uniqueInstance;
            }
        }

        public virtual bool UseHttpClient
        { 
            get
            {
                if (ConfigurationReader == null)

                {
                    return false;
                }

                var useHttpClientConfigValue = ConfigurationReader.GetConfigurationValue(Constants.Configuration.UseHttpClientConfig);

                return !string.IsNullOrWhiteSpace(useHttpClientConfigValue) && string.Equals(useHttpClientConfigValue.ToLower(), Generic.TrueValue);
            }
        }
        public virtual HttpClient HttpClient
        {
            get
            {
                if (UseHttpClient)
                {
                    return Container.Resolve<HttpClient>();
                }

                throw new ApplicationException(Constants.Configuration.HttpClientSettingExceptionMsg);
            }
        }

        #endregion

        #region Constructor

        protected ServiceTestManager()
        {
        }

        #endregion

        #region Public Methods

        public override void OnInitialiseAssemblyDependencies(TestParameters testParameters = null)
        {
            base.OnInitialiseAssemblyDependencies(testParameters);

            InitialiseHttpClient();
        }

        public override void OnDisposeAssemblyDependencies()
        {
            if (UseHttpClient)
            {
                HttpClient?.Dispose();
            }

            base.OnDisposeAssemblyDependencies();
        }

        public virtual void InitialiseHttpClient()
        {
            var useHttpClient = ConfigurationReader.GetConfigurationValue(Constants.Configuration.UseHttpClientConfig);

            if (string.IsNullOrWhiteSpace(useHttpClient) || !string.Equals(useHttpClient.ToLower(), Generic.TrueValue)) return;
            
            var cookieContainer = new CookieContainer {PerDomainCapacity = 5};
            var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseDefaultCredentials = true,
                AllowAutoRedirect = false
            };

            var httpClient = new HttpClient(httpClientHandler);
            Container.Register(Component.For<HttpClient>().Instance(httpClient).OverridesExistingRegistration().LifestyleSingleton())
                .Register(Component.For<HttpClientHandler>().Instance(httpClientHandler).OverridesExistingRegistration().LifestyleSingleton())
                .Register(Component.For<CookieContainer>().Instance(cookieContainer).OverridesExistingRegistration().LifestyleSingleton());
        }

        #endregion
    }
}
