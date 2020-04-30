namespace AutoTestMate.NUnit.Web.Constants
{
    public class Exceptions
    { 

        #region Constants
        
        public const string ExceptionMsgWebBrowserWaitInstanceNotInitialised = "The WebDriver browser wait instance was not initialized. You should first call the method Start.";

        public const string ExceptionMsgScreenshotsError = "Unable to save screenshot with error - ";
        
        public const string ExceptionMsgSetupError = "Error during setup - ";

        public const string ExceptionMsgSingletonAlreadyInitialised = "TestManager already initialised, please dispose existing driver prior to creating another one";
        
        public const string ExceptionMsgWakeUpError = "Error occured while trying to wakeup url, please ensure url is accessible and rerun test if it is";
        
        #endregion
    }
}
