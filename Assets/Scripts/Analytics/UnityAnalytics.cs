using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalytics : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (ConsentCheckException e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}