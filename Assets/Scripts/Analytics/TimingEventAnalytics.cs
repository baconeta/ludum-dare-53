using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

namespace Analytics
{
    /// <summary>
    /// Script used to time a specific event.
    /// Trigger StartEvent to start the timing, and EndEvent to end timing.
    /// Use a new instance of this script for each event to be timed.
    /// </summary>
    public class TimingEventAnalytics : MonoBehaviour
    {
        [SerializeField] private string analyticStartedName;
        [SerializeField] private string analyticEndedName;
        [SerializeField] private string analyticEndTimeName;
        [SerializeField] private bool sendStartEvent = true;

        private float _cachedTimeStart;
        private bool _started;

        public void StartTimingEvent()
        {
            _started = true;
            _cachedTimeStart = Time.time;

            if (!sendStartEvent) return;

            // The ‘analyticStartedName’ event will get cached locally 
            //and sent during the next scheduled upload, within 1 minute
            AnalyticsService.Instance.CustomData(analyticStartedName);

            // You can call Events.Flush() to send the event immediately
            AnalyticsService.Instance.Flush();
        }

        public void EndTimingEvent()
        {
            if (!_started)
            {
                Debug.Log("No event sent, as EndTimingEvent was called before StartTimingEvent.");
                return;
            }

            var currentTime = Time.time;
            var timeSinceStart = currentTime - _cachedTimeStart;
            _started = false;

            // Define custom parameters
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {analyticEndTimeName, timeSinceStart}
            };

            AnalyticsService.Instance.CustomData(analyticEndedName, parameters);
            AnalyticsService.Instance.Flush();
        }
    }
}