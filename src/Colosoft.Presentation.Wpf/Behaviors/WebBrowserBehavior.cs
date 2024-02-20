using System;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation.Behaviors
{
    public static class WebBrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(WebBrowserBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser browser)
        {
            if (browser is null)
            {
                throw new ArgumentNullException(nameof(browser));
            }

            return (string)browser.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser browser, string value)
        {
            if (browser is null)
            {
                throw new ArgumentNullException(nameof(browser));
            }

            browser.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var browser = dependencyObject as WebBrowser;
            if (browser != null)
            {
                browser.NavigateToString(e.NewValue as string);
            }
        }
    }
}
