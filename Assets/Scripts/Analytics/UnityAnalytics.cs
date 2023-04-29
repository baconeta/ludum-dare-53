using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalytics : MonoBehaviour
    {
        private static bool optedOut = false;

        // TODO https://docs.unity.com/analytics/en/manual/EventsTracking

        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
                if (consentIdentifiers.Count == 0)
                {
                    Debug.Log("The user has provided all required consents for analytic tracking.");
                }
                else
                {
                    Debug.Log("The user has yet to provide all required consents for analytic tracking.");
                    DisplayPrivacyInformation();
                }
            }
            catch (ConsentCheckException e)
            {
                // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
                Debug.Log(e.Reason);
            }
        }

        public void OptOut()
        {
            try
            {
                if (!optedOut)
                {
                    // Show a GDPR/COPPA/other opt-out consent flow
                    // If a user opts out
                    AnalyticsService.Instance.OptOut();
                }
                // Record that we have checked a user's consent, so we don't repeat the flow unnecessarily.
                // In a real game, use PlayerPrefs or an equivalent to persist this state between sessions
                optedOut = true;
            }
            catch (ConsentCheckException e)
            {
                // Handle the exception by checking e.Reason
                Debug.Log(e.Reason);
            }
        }

        public static void DisplayPrivacyInformation()
        {
            Application.OpenURL(AnalyticsService.Instance.PrivacyUrl);
        }
    }
}