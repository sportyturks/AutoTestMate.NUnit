using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace AutoTestMate.NUnit.Web.Extensions
{
    public static class FindElementExtensions
    {
        /// <summary>
        /// Find an element, waiting until a timeout is reached if necessary.
        /// </summary>
        /// <param name="context">The search context.</param>
        /// <param name="by">Method to find elements.</param>
        /// <param name="timeout">How many seconds to wait.</param>
        /// <param name="polling">How many seconds to research.</param>
        /// <param name="displayed">Require the element to be displayed?</param>
        /// <returns>The found element.</returns>
        public static IWebElement FindElement(this ISearchContext context, By by, uint timeout = 60, uint polling = 250, bool displayed = true)
        {
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.DefaultWait<ISearchContext>(context)
                {
                    Timeout = TimeSpan.FromSeconds(timeout),
                    PollingInterval = TimeSpan.FromMilliseconds(polling)
                };

                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverException), typeof(StaleElementReferenceException));

                return wait.Until(ctx =>
                {
                    var elem = ctx.FindElement(by);

                    if (elem == null) return null;

                    if (displayed)
                    {
                        if (elem.Displayed && elem.Enabled)
                        {
                            return elem;
                        }
                    }
                    else
                    {
                        if (!elem.Displayed || !elem.Enabled)
                        {
                            return elem;
                        }

                        return elem;
                    }

                    return null;
                });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Find an element, waiting until a timeout is reached if necessary.
        /// </summary>
        /// <param name="context">The search context.</param>
        /// <param name="by">Method to find elements.</param>
        /// <param name="timeout">How many seconds to wait.</param>
        /// <param name="polling">How many seconds to research.</param>
        /// <param name="displayed">Require the element to be displayed?</param>
        /// <returns>The found element.</returns>
        public static ReadOnlyCollection<IWebElement> FindElements(this ISearchContext context, By by, uint timeout = 60, uint polling = 250)
        {
            var wait = new OpenQA.Selenium.Support.UI.DefaultWait<ISearchContext>(context);
            wait.Timeout = TimeSpan.FromSeconds(timeout);
            wait.PollingInterval = TimeSpan.FromMilliseconds(polling);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverException), typeof(StaleElementReferenceException));
            return wait.Until(ctx => {
                var elem = ctx.FindElements(by);
                if (elem.Count == 0)
                    return null;
                return elem;
            });
        }
    }
}
