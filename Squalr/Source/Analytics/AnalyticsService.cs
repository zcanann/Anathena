namespace Squalr.Source.Analytics
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// A service that handles analytics reporting.
    /// </summary>
    public class AnalyticsService
    {
        /// <summary>
        /// The url for the Google Analytics associated site.
        /// </summary>
        private const String AnalyticsUrl = "https://www.squalr.com";

        /// <summary>
        /// The Google Analytics code associated with the site.
        /// </summary>
        private const String GoogleAnalyticsCode = "UA-86974038-1";

        /// <summary>
        /// The category for analytics events.
        /// </summary>
        private const String AnalyticsServiceCategory = "DesktopClient";

        /// <summary>
        /// Singleton instance of the <see cref="AnalyticsService" /> class.
        /// </summary>
        private static Lazy<AnalyticsService> analyticsServiceInstance = new Lazy<AnalyticsService>(
                () => { return new AnalyticsService(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="AnalyticsService" /> class from being created.
        /// </summary>
        private AnalyticsService()
        {
        }

        /// <summary>
        /// An enumeration of analytics actions.
        /// </summary>
        public enum AnalyticsAction
        {
            /// <summary>
            /// A general action.
            /// </summary>
            [Description("General")]
            General,

            /// <summary>
            /// An action from the stream client.
            /// </summary>
            [Description("Stream Client")]
            StreamClient,
        }

        /// <summary>
        /// Gets or sets the analytics session.
        /// </summary>
        // private AnalyticsSession Session { get; set; }

        /// <summary>
        /// Gets a singleton instance of the <see cref="AnalyticsService"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static AnalyticsService GetInstance()
        {
            return AnalyticsService.analyticsServiceInstance.Value;
        }

        /// <summary>
        /// Starts the analytics service.
        /// </summary>
        public void Start()
        {
            /*
            this.Session = new AnalyticsSession(AnalyticsService.AnalyticsUrl, AnalyticsService.GoogleAnalyticsCode);
            */
        }

        /// <summary>
        /// Sends an analytics event with a runtime exception.
        /// </summary>
        /// <param name="action">The analytics action.</param>
        /// <param name="ex">The exception.</param>
        public void SendEvent(AnalyticsAction action, Exception ex)
        {
            this.SendEvent(action, "Error", ex?.ToString());
        }

        /// <summary>
        /// Sends an analytics event with an empty value.
        /// </summary>
        /// <param name="action">The analytics action.</param>
        /// <param name="label">>The analytics label.</param>
        public void SendEvent(AnalyticsAction action, String label)
        {
            this.SendEvent(action, label, "empty");
        }

        /// <summary>
        /// Sends an analytics event.
        /// </summary>
        /// <param name="action">The analytics action.</param>
        /// <param name="label">>The analytics label.</param>
        /// <param name="value">>The analytics value.</param>
        public void SendEvent(AnalyticsAction action, String label, String value)
        {
            /*
            IAnalyticsPageViewRequest page = this.Session.CreatePageViewRequest("/", "Home page");

            page.SendEvent(AnalyticsService.AnalyticsServiceCategory, action.ToString(), label, value);
            */
        }
    }
    //// End class
}
//// End namespace