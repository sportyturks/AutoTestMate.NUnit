namespace AutoTestMate.NUnit.Infrastructure.Core
{
    /// <summary>
    ///     Interface for the Framework's Logging Utility
    /// </summary>
    public interface ILoggingUtility
    {
        void Info(string message);
        void Error(string message);
        void Warning(string message);
        void Debug(string message);
    }
}
