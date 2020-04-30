using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using AutoTestMate.NUnit.Infrastructure.Core;
using Newtonsoft.Json;
using RestSharp;

namespace AutoTestMate.NUnit.Services.Client
{
    public abstract class CustomRestSharpClient
    {
        #region Constructor

        protected CustomRestSharpClient(IRestClient client, IConfigurationReader configurationReader, IAuthorisationToken authorisationToken)
        {
            Client = client;
            AuthorisationToken = authorisationToken;
            ConfigurationReader = configurationReader;
        }

        #endregion


        #region Private Method/s

        private bool ConvertSingleSimpleType<T>(IRestResponse response, Type genericType, out T deserializeObject)

        {
            object data = null;

            var converted = false;


            if (genericType == typeof(DateTime))

            {
                var contentValue = Convert.ToDateTime(response.Content.Replace("\"", ""));

                data = new WrappedHttpResponseBody<DateTime>((int) response.StatusCode, contentValue);

                converted = true;
            }

            else if (genericType == typeof(int))

            {
                var contentValue = Convert.ToInt32(response.Content.Replace("\"", ""));

                data = new WrappedHttpResponseBody<int>((int) response.StatusCode, contentValue);

                converted = true;
            }

            else if (genericType == typeof(long))

            {
                var contentValue = Convert.ToInt64(response.Content.Replace("\"", ""));

                data = new WrappedHttpResponseBody<long>((int) response.StatusCode, contentValue);

                converted = true;
            }

            else if (genericType == typeof(decimal))

            {
                var contentValue = Convert.ToDecimal(response.Content.Replace("\"", ""));

                data = new WrappedHttpResponseBody<decimal>((int) response.StatusCode, contentValue);

                converted = true;
            }

            else if (genericType == typeof(string))

            {
                var contentValue = Convert.ToString(response.Content.Replace("\"", ""));

                data = new WrappedHttpResponseBody<string>((int) response.StatusCode, contentValue);

                converted = true;
            }

            else if (genericType == typeof(DateTime?))

            {
                DateTime? contentValue;

                var stringValue = response.Content.Replace("\"", "");

                converted = true;

                if (string.IsNullOrWhiteSpace(stringValue))
                    contentValue = null;

                else
                    contentValue = Convert.ToDateTime(stringValue);

                data = new WrappedHttpResponseBody<DateTime?>((int) response.StatusCode, contentValue);
            }

            else if (genericType == typeof(int?))

            {
                int? contentValue;

                var stringValue = response.Content.Replace("\"", "");

                converted = true;

                if (string.IsNullOrWhiteSpace(stringValue))
                    contentValue = null;

                else
                    contentValue = Convert.ToInt32(stringValue);

                data = new WrappedHttpResponseBody<int?>((int) response.StatusCode, contentValue);
            }

            else if (genericType == typeof(long?))

            {
                long? contentValue;

                var stringValue = response.Content.Replace("\"", "");

                converted = true;

                if (string.IsNullOrWhiteSpace(stringValue))
                    contentValue = null;

                else
                    contentValue = Convert.ToInt64(stringValue);

                data = new WrappedHttpResponseBody<long?>((int) response.StatusCode, contentValue);
            }

            else if (genericType == typeof(decimal?))

            {
                decimal? contentValue;

                var stringValue = response.Content.Replace("\"", "");

                converted = true;

                if (string.IsNullOrWhiteSpace(stringValue))
                    contentValue = null;

                else
                    contentValue = Convert.ToDecimal(stringValue);

                data = new WrappedHttpResponseBody<decimal?>((int) response.StatusCode, contentValue);
            }


            var jsonObj = JsonConvert.SerializeObject(data);

            {
                deserializeObject = JsonConvert.DeserializeObject<T>(jsonObj);

                return converted;
            }
        }

        #endregion

        #region Constants

        public const string ApiEnvironmentUrlConfig = "ApiEnvironmentUrl";

        public const string TokenTypeBearer = "Bearer";

        #endregion


        #region Propeties

        protected string ApiArea { get; set; }

        protected string ApiEnvironmentBaseUrl =>
            ConfigurationReader.GetConfigurationValue(ApiEnvironmentUrlConfig, true);

        protected string ApiBaseUrl => ApiEnvironmentBaseUrl + ApiArea;

        protected IConfigurationReader ConfigurationReader { get; }

        protected IRestClient Client { get; }

        protected IAuthorisationToken AuthorisationToken { get; }

        #endregion


        #region Methods

        protected string CombineUrl(string baseUrl, params string[] steps)

        {
            var url = new StringBuilder(baseUrl.TrimEnd('/'));

            return steps.Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(url, (u, s) => u.Append($"{(s.StartsWith("?") ? "" : "/")}{s.Trim('/')}")).ToString();
        }


        /// <summary>
        ///     Combines parameters into a single string that can be appended to an API URL.
        /// </summary>
        /// <param name="parameters">A dictionary containing one or more parameters to append</param>
        /// <returns>A string containing all parameters in the appropriate format.</returns>
        protected string CombineParameters(IDictionary<string, string> parameters)

        {
            var sb = new StringBuilder();

            parameters.Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key)).Aggregate(sb,
                (b, kvp) => sb.Append($"{(sb.Length == 0 ? "?" : "&")}{kvp.Key}={kvp.Value}"));

            return sb.ToString();
        }


        protected virtual T Execute<T>(RestRequest request) where T : new()

        {
            var response = Client.Execute<T>(request);


            if (response.ErrorException != null)

            {
                const string message = "Error retrieving response.  Check inner details for more info.";

                var apiException = new ApplicationException(message, response.ErrorException);

                throw apiException;
            }


            return response.Data;
        }

        protected virtual IRestRequest SetClientDefaults(string url, Method method, object requestObject,
            bool forceResetToken = false)

        {
            SetDefaultHeaders(url);

            return SetDefaultRestRequest(url, method, requestObject, forceResetToken);
        }


        protected virtual void SetDefaultHeaders(string url)

        {
            Client.BaseUrl = new Uri(url);

            Client.AddHandler("application/json", () => NewtonsoftJsonSerializer.Default);
        }


        protected virtual IRestRequest SetDefaultRestRequest(string url, Method method, object requestObject,
            bool forceResetToken)

        {
            IRestRequest restRequest = new RestRequest(url, method)

            {
                RequestFormat = DataFormat.Json,

                JsonSerializer = NewtonsoftJsonSerializer.Default
            };


            restRequest.AddHeader("employment.gov.au-UniqueRequestMessageId", Guid.NewGuid().ToString());

            restRequest.AddHeader(nameof(HttpRequestHeaders.Authorization),
                $"{TokenTypeBearer} {AuthorisationToken.GetToken(forceResetToken)}");

            restRequest.AddJsonBody(requestObject);


            return restRequest;
        }


        protected virtual T Deserialize<T>(IRestResponse response)

        {
            try

            {
                if (response.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();


                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.Content);


                return JsonConvert.DeserializeObject<T>(response.Content);
            }

            catch

            {
                //Try converting Simple types

                var typeParameterType = typeof(T);

                var genericType = typeParameterType.GenericTypeArguments[0];


                if (ConvertSingleSimpleType(response, genericType, out T deserializeObject)) return deserializeObject;


                throw;
            }
        }


        protected string ConvertToString(object value, CultureInfo cultureInfo)

        {
            if (value is Enum)

            {
                var name = Enum.GetName(value.GetType(), value);

                if (name != null)

                {
                    var field = value.GetType().GetTypeInfo().GetDeclaredField(name);

                    if (field != null)

                    {
                        var attribute =
                            field.GetCustomAttribute(typeof(EnumMemberAttribute)) as
                                EnumMemberAttribute;


                        if (attribute != null) return attribute.Value != null ? attribute.Value : name;
                    }
                }
            }

            else if (value is bool)

            {
                return Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }

            else if (value is byte[])

            {
                return Convert.ToBase64String((byte[]) value);
            }

            else if (value != null && value.GetType().IsArray)

            {
                var array = ((Array) value).OfType<object>();

                return string.Join(",", array.Select(o => ConvertToString(o, cultureInfo)));
            }


            return Convert.ToString(value, cultureInfo);
        }

        #endregion
    }

    public  class WrappedHttpResponseBody<T>
    {
        public WrappedHttpResponseBody(int responseStatusCode, object contentValue)
        {
            throw new NotImplementedException();
        }
    }

    public interface IAuthorisationToken
    {
        string GetToken(bool forceReset);
    }
}