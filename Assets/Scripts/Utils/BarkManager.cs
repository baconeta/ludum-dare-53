using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarkManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The comma-separated list of damage-provoked barks to pull from.")]
    private TextAsset listOfDamageBarks;
    [SerializeField]
    [Tooltip("The comma-separated list of ambience-provoked barks to pull from.")]
    private TextAsset listOfAmbienceBarks;
    [SerializeField]
    [Tooltip("The comma-separated list of DUET damage-provoked barks to pull from. These will be played with Charon.")]
    private TextAsset listOfDuetDamageBarks;
    [SerializeField]
    [Tooltip("The character sequence to use to split barks by.")]
    private string delimiter = ",";

    private string[] damageBarks;
    private string[] ambienceBark;
    private string[] duetDamageBarks;

    private void Start()
    {
        damageBarks = listOfAmbienceBarks.text.Split(delimiter);
        ambienceBark = listOfDamageBarks.text.Split(delimiter);
        duetDamageBarks = listOfDuetDamageBarks.text.Split(delimiter);
    }

    public string GetDamageBark()
    {
        return GetBark(damageBarks, "Damage");
    }

    public string GetAmbienceBark()
    {
        return GetBark( ambienceBark, "Ambience");
    }

    public string GetDuetDamageBark()
    {
        return GetBark(duetDamageBarks, "Ambience");
    }

    private string GetBark(string[] barks, string prefix)
    {
        // Check if all barks have been used. If they have been, reset them.
        int barksUsed = PlayerPrefs.GetInt(prefix + "BarksUsedCount");
        if (barksUsed >= barks.Length)
            ResetBarks(barks.Length, prefix);
        
        int index;
        while (true)
        {
            index = Random.Range(0, barks.Length);
            // Select a random bark.
            // Check if it has been used.
            if (PlayerPrefs.GetInt(prefix + "BarkBeenUsed" + index) != 1) break;
        }
        // Increment the number of barks used.
        PlayerPrefs.SetInt(prefix + "BarksUsedCount", barksUsed + 1);
        // Mark the bark as being used.
        PlayerPrefs.SetInt(prefix + "BarkBeenUsed" + index, 1);
        return barks[index];
    }

    private void ResetBarks(int length, string prefix)
    {
        PlayerPrefs.SetInt(prefix + "BarksUsedCount", 0);
        for (int i = 0; i < length; i++)
        {
            PlayerPrefs.DeleteKey(prefix + "BarkBeenUsed" + i);
        }
    }

}
