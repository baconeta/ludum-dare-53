using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Achievements
{
    [Serializable]
    public enum AchievementType
    {
        Bool,
        Int,
        Float
    }

    [Serializable]
    public class AchievementSettingsBase
    {
        public AchievementType achievementType;
        public string triggerValue;
    }

    [Serializable]
    public class Achievement
    {
        public string achievementName;

        [Tooltip("Name used to reference the prefs values")]
        public string achievementUserPrefsCodeName;

        public string subMessage = "???";

        [HideInInspector] public bool completed;

        public AchievementSettingsBase achievementSettings;

        private float _internalRequiredValue;

        public void SetInternalTriggerValue()
        {
            switch (achievementSettings.achievementType)
            {
                case AchievementType.Bool:
                    _internalRequiredValue = 1.0f;
                    break;
                case AchievementType.Int:
                    _internalRequiredValue = int.Parse(achievementSettings.triggerValue);
                    break;
                case AchievementType.Float:
                    _internalRequiredValue = float.Parse(achievementSettings.triggerValue);
                    break;
                default:
                    Debug.Log(
                        $"Achievement {achievementName} trigger value set incorrectly for type {achievementSettings.achievementType}");
                    break;
            }
        }

        public bool CheckCompletion()
        {
            return achievementSettings.achievementType switch
            {
                AchievementType.Bool => AchievementController.Instance.GetBool(achievementUserPrefsCodeName),
                AchievementType.Int => AchievementController.Instance.GetInt(achievementUserPrefsCodeName) >=
                                       (int) _internalRequiredValue,
                AchievementType.Float => AchievementController.Instance.GetFloat(achievementUserPrefsCodeName) >=
                                         _internalRequiredValue,
                _ => false
            };
        }
    }

    public class AchievementController : EverlastingSingleton<AchievementController>
    {
        [SerializeField] private List<AchievementData> allAchievements;

        private List<Achievement> _achievements = new();

        public List<Achievement> Achievements => _achievements;

        public void Start()
        {
            // Marks off any tasks that should be already be marked off as being completed prior to this session.
            CheckCompletedAchievements();

#if !UNITY_EDITOR
         SetupAndVerifyAchievements();
#endif
        }

        public void SetInt(string keyName, int value)
        {
            PlayerPrefs.SetInt(keyName, value);
        }

        public int GetInt(string keyName)
        {
            return PlayerPrefs.GetInt(keyName, 0);
        }

        public void AddInt(string keyName, int value)
        {
            PlayerPrefs.SetInt(keyName, GetInt(keyName) + value);
        }

        public float GetFloat(string keyName)
        {
            return PlayerPrefs.GetFloat(keyName, 0.0f);
        }

        public void AddFloat(string keyName, int value)
        {
            PlayerPrefs.SetFloat(keyName, GetFloat(keyName) + value);
        }

        public void SetBool(string boolName, bool value)
        {
            PlayerPrefs.SetInt(boolName, value ? 1 : 0);
        }

        public bool GetBool(string boolName)
        {
            return PlayerPrefs.GetInt(boolName) == 1;
        }

        public List<Achievement> GetCompletedAchievements()
        {
            return Achievements.Where(achievement => achievement.completed).ToList();
        }

        public List<Achievement> CheckCompletedAchievements()
        {
            var newlyCompleted = new List<Achievement>();

            for (var i = 0; i < Achievements.Count; i++)
            {
                Achievement ach = Achievements[i];
                if (ach.completed) continue;

                if (ach.CheckCompletion())
                {
                    newlyCompleted.Add(ach);
                    ach.completed = true;
                    Achievements[i] = ach;
                }
            }

            return newlyCompleted;
        }

        public void ResetAllAchievements()
        {
            PlayerPrefs.DeleteAll();
            for (var i = 0; i < Achievements.Count; i++)
            {
                Achievement ach = Achievements[i];
                ach.completed = false;
                Achievements[i] = ach;
            }

            CheckCompletedAchievements();
        }

        private void SetupAndVerifyAchievements()
        {
            foreach (AchievementData ach in allAchievements)
            {
                if (ach == null) continue;

                Achievement a = new()
                {
                    achievementSettings = ach.achievementSettings, achievementName = ach.achievementName,
                    achievementUserPrefsCodeName = ach.achievementUserPrefsCodeName, subMessage = ach.subMessage
                };
                a.SetInternalTriggerValue();
                _achievements.Add(a);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetupAndVerifyAchievements();
        }
#endif
    }
}