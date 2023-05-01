using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SoulsOnBoatManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> allSouls;
        [SerializeField] private int numberOfSoulsPerVisual = 4;

        private void OnEnable()
        {
            //Event registration
            BoatCapacity.OnSoulsChanged += UpdateSoulVisuals;
        }

        private void UpdateSoulVisuals(SoulAmounts soulAmounts)
        {
            var soulsCarried = soulAmounts.CurrentLoad;
            var soulsToShow = (int) Mathf.Ceil(Mathf.Clamp(soulsCarried / numberOfSoulsPerVisual, 0, allSouls.Count));

            if (soulsToShow == 0)
            {
                HideAllSouls();
                return;
            }

            // show n souls
            List<GameObject> gameObjectsToShow = ChooseRandomElements(allSouls, soulsToShow);
            foreach (var soul in gameObjectsToShow)
            {
                soul.SetActive(true);
            }
        }

        private void OnDisable()
        {
            // Event de-registration
            BoatCapacity.OnSoulsChanged -= UpdateSoulVisuals;
        }

        private void Start()
        {
            // Hide all souls
            HideAllSouls();
        }

        private void HideAllSouls()
        {
            foreach (GameObject soul in allSouls)
            {
                soul.SetActive(false);
            }
        }

        public static List<T> ChooseRandomElements<T>(List<T> list, int x)
        {
            if (x >= list.Count)
            {
                return list;
            }

            List<T> randomElements = new List<T>();

            while (randomElements.Count < x)
            {
                T element = list[UnityEngine.Random.Range(0, list.Count)];

                if (!randomElements.Contains(element))
                {
                    randomElements.Add(element);
                }
            }

            return randomElements;
        }
    }
}