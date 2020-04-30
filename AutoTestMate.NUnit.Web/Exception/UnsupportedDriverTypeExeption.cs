using System;
using System.Diagnostics.CodeAnalysis;

namespace AutoTestMate.NUnit.Web.Exception
{
	[ExcludeFromCodeCoverage]
	public class UnsupportedDriverTypeException : System.Exception
	{
		/// <summary>
		/// </summary>
		public UnsupportedDriverTypeException(Type driverType, string message) : base(($"{driverType} is not a supported driver type. - {message}"))
		{
		}
	}

	[ExcludeFromCodeCoverage]
	public class InvalidPageCastException : System.Exception
	{
		public InvalidPageCastException(Type type, string message) : base(message)
		{
			Type = type;
		}

		public Type Type { get; set; }
	}

	[ExcludeFromCodeCoverage]
	public class WakeupApplicationException : ApplicationException
	{
		public WakeupApplicationException() : base(Constants.Exceptions.ExceptionMsgWakeUpError)
		{
		}

		public WakeupApplicationException(System.Exception exp) : base(Constants.Exceptions.ExceptionMsgWakeUpError, exp)
		{
		}
	}
}
