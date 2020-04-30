using System.Diagnostics.CodeAnalysis;

namespace AutoTestMate.NUnit.Infrastructure.Constants
{
    [ExcludeFromCodeCoverage]
    public class Generic
    {
        public const string FalseValue = "false";
        public const string TrueValue = "true";
        public const string Empty = "";
	    public const string TestSuccessMessage = "Test has completed successfully.";
	    public const string TestErrorMessage = "Test has not completed successfully. Please refer to logs for further details.";
	}
}
