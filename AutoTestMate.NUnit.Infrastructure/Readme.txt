
Below is a list of the main funcitonality that the infrastructure project offers:

*	ICustomAttributes to solve the issue of 
	-	Before Test attributes
	-	After Test attributes

*	ExcelDynamicData Attribute that enables a tester to use a master spreadsheet 
	and access data non-sequentially, instead using a row key as well as using data driven

*	IAuthentication Attribute that enables a test to authenticate and authorise 
	before test execution

*	IConfigurationReader that allows a test to easily access app.config, testSetting.runsettings settings

*	ILoggingUtility gives the users access to log details of a test to a log file

*	Thread Safe Singleton TestManager that contains reference to Test dependencies, including the Ioc container

*	TestBase that gives enables support of custom attributes and access to TestManager properties
