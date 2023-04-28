using System;
using UnityEngine;

namespace Achievements
{
    [CreateAssetMenu]
    [Serializable]
    public class AchievementData : ScriptableObject
    {
        public string achievementName;

        [Tooltip("Name used to reference the prefs values")]
        public string achievementUserPrefsCodeName;

        public string subMessage = "???";
        public AchievementSettingsBase achievementSettings;
    }
}