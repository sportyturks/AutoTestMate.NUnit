using System;
using System.ComponentModel;
using AutoTestMate.NUnit.Infrastructure.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace AutoTestMate.NUnit.Web.Extensions
{
	public static class WebElementExtensions
	{
		public static IWebElement GetParent(this IWebElement element)
		{
			return element.FindElement(By.XPath(".."));
		}

		public static void Highlight(this IWebElement context, IWebDriver driver)
		{
			string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 2px; border-style: solid; border-color: red"";";
			driver.ExecuteJavaScript(highlightJavascript, context);
		}

		public static void ClearHighlight(this IWebElement context, IWebDriver driver)
		{
			string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 0px; border-style: solid; border-color: red""; ";
			driver.ExecuteJavaScript(highlightJavascript, context);
		}

		public static IWebDriver GetWebDriverFromElement(IWebElement element)
		{
			var realElement = element.GetType() != typeof(RemoteWebElement)
				? element
				: ((IWrapsElement) element).WrappedElement;

			return ((IWrapsDriver) realElement).WrappedDriver;
		}

		public static IWebElement ClearAndSendKeys(this IWebElement element, string keys)
		{
			element.Clear();
			element.SendKeys(keys);
			return element;
		}

		public const int ActionTimeout = 2000;

		public static void MoveToClick(this IWebElement element, IWebDriver driver)
		{
			try
			{
				driver.JavaScript().ExecuteScript("arguments[0].scrollIntoView();", element);
				element.Click();
			}
			catch (WebDriverException)
			{
				driver.JavaScript().ExecuteScript("window.scrollTo(0, 0);");
				driver.JavaScript().ExecuteScript("arguments[0].scrollIntoView();", element);
				element.Click();
			}
		}

        public static void DoubleClick(this IWebElement element)
		{
			element.Click();
			element.Click();
		}

		public static void WaitClick(this IWebElement element, IWebDriver driver)
		{
			driver.WaitForPageToLoad();
			element.Click();
		}

		public static void WaitClickMove(this IWebElement element, IWebDriver driver)
		{
			driver.WaitForPageToLoad();
			var actions = new Actions(driver);
			actions.MoveToElement(element).Click().Build().Perform();
		}

		public static void WaitClickJs(this IWebElement element, IWebDriver driver)
		{
			driver.WaitForPageToLoad();
			driver.JavaScript().ExecuteScript("window.scrollTo(0, 0);");
			driver.JavaScript().ExecuteScript("arguments[0].click();", element);
		}


		public static void WaitClick(this IWebElement element, IWebDriver driver, ref int iteration)
		{
			driver.WaitForPageToLoad();

			iteration = iteration > 3 ? 1 : iteration;

			switch (iteration)
			{
				case 1:
					WaitClick(element, driver);
					break;
				case 2:
					WaitClickMove(element, driver);
					break;
				case 3:
					WaitClickJs(element, driver);
					break;
				default:
					WaitClick(element, driver);
					break;
			}
		}


		public static bool IsAlertPresent(IWebDriver driver)
		{
			try
			{
				driver.SwitchTo().Alert();
				return true;
			} // try 
			catch (NoAlertPresentException)
			{
				return false;
			} // catch 
		}

		/// <summary>
		/// This function is very simple, all it does is make our code a little from concise. 
		/// In the original Selenium RC, SendKeys would clear a text field and then send the required text. 
		/// In Selenium 2, text fields are not cleared first unless explicitly executed.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="value"></param>
		/// <param name="clearFirst"></param>
		public static void SendKeys(this IWebElement element, string value, bool clearFirst)
		{
			if (clearFirst) element.Clear();
			element.SendKeys(value);
		}

		/// <summary>
		/// This one is extremely useful and is even recommended in the Selenium FAQ. 
		/// The function returns the text in the entire page without any HTML code. 
		/// Note that this function can be a little quirky if the page hasn't finished loading.
		/// </summary>
		/// <param name="driver"></param>
		/// <returns></returns>
		public static string GetText(this IWebDriver driver)
		{
			return driver.FindElement(By.TagName("body")).Text;
		}

		/// <summary>
		/// Selenium's default behavior for FindElement is to throw an exception if an element is not found. HasElement catches
		/// this exception to allow us to test for elements without stopping execution of the entire program.
		/// Similary you can write an extension function for IWebElement to test of elements nested in another element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="by"></param>
		/// <returns></returns>
		public static bool HasElement(this IWebElement element, By by)
		{
			try
			{
				element.FindElement(by);
			}
			catch (NoSuchElementException)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// IWebElement objects have a handy GetAuthenticationAttribute function.This is useful, for example, for retrieving an input's value
		/// (which is not the same as an input's text). So why is there no SetAttribute function? The reason is simply that the creators
		/// of Selenium have strived to create a tool that simulates a user interacting with the web.A human user would not normally
		/// be able to modify the underlying attributes of an object.
		/// 
		/// Regardless, setting an element's attributes can be essential for working around some Selenium quirks. For example, masked input
		/// fields don't play well with SendKeys.In this case, we have to directly set the value of the field.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="attributeName"></param>
		/// <param name="value"></param>
		public static void SetAttribute(this IWebElement element, string attributeName, string value)
		{
			IWrapsDriver wrappedElement = element as IWrapsDriver;
			if (wrappedElement == null)
				throw new ArgumentException("element", "Element must wrap a web driver");

			IWebDriver driver = wrappedElement.WrappedDriver;
			IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
			if (javascript == null)
				throw new ArgumentException("element", "Element must wrap a web driver that supports javascript execution");

			javascript.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
		}

		/// <summary>
		/// The GetAuthenticationAttribute function and the Text property both return strings. Many times these strings need to be parsed into other types. 
		/// Thus we can write a function to wrap that functionality for us. I borrowed the generic TryParse code from StackOverflow to give 
		/// the function maximum flexibility.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="element"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		public static T GetAttributeAsType<T>(this IWebElement element, string attributeName)
		{
			string value = element.GetAttribute(attributeName) ?? string.Empty;
			return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
		}

		/// <summary>
		/// The GetAuthenticationAttribute function and the Text property both return strings. Many times these strings need to be parsed into other types. 
		/// Thus we can write a function to wrap that functionality for us. I borrowed the generic TryParse code from StackOverflow to give 
		/// the function maximum flexibility.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="element"></param>
		/// <returns></returns>
		public static T TextAsType<T>(this IWebElement element)
		{
			string value = element.Text;
			return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
		}

		public static bool VisibleWait(this IWebElement elm, uint timeOut = 10, uint polling = 550)
		{
			try
			{
				var wait = new DefaultWait<IWebElement>(elm)
				{
					Timeout = TimeSpan.FromSeconds(timeOut),
					PollingInterval = TimeSpan.FromMilliseconds(polling)
				};

				wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverException), typeof(StaleElementReferenceException));

				return wait.Until(element => element.Displayed && element.Enabled);
			}
			catch (System.Exception exp)
			{
				var loggingUtility = TestManager.Instance().LoggingUtility;
				loggingUtility.Warning(exp.Message + exp.StackTrace);
				return false;
			}
		}
	}
}
