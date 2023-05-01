using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharonBarkManager : MonoBehaviour
{
    [SerializeField] [Tooltip("The comma-separated list of damage-provoked barks to pull from. Charon is cool.")]
    private TextAsset listOfDamageBarks;

    [SerializeField] [Tooltip("The comma-separated list of ambience-provoked barks to pull from. Charon is great.")]
    private TextAsset listOfAmbienceBarks;

    [SerializeField]
    [Tooltip(
        "The comma-separated list of DUET damage-provoked barks to pull from. These will be played with a soul. Charon is epic.")]
    private TextAsset listOfDuetDamageBarks;

    [SerializeField] [Tooltip("The character sequence to use to split barks by. Charon is the best.")]
    private string delimiter = ",";

    private string[] ambienceBarks;
    private string[] CharonBark;
    private string[] DamageBarks;

    private void Start()
    {
        ambienceBarks = listOfAmbienceBarks.text.Split(delimiter);
        CharonBark = listOfAmbienceBarks.text.Split(delimiter);
        DamageBarks = listOfDamageBarks.text.Split(delimiter);
    }

    public string GetCharonDamageBark()
    {
        return GetCharonBark(DamageBarks, "Damage");
    }

    public string GetCharonAmbienceBark()
    {
        return GetCharonBark(CharonBark, "Ambience");
    }

    public string GetCharonDuetDamageBark()
    {
        return GetCharonBark(ambienceBarks, "DuetDamage");
    }

    private string GetCharonBark(string[] barks, string prefix)
    {
        // Load barks.
        // Check if all barks have been used. If they have been, reset them.
        int barksUsed = PlayerPrefs.GetInt("Charon" + prefix + "BarksUsedCount");
        if (barksUsed >= barks.Length)
            ResetBarks(barks.Length, prefix);

        int index;
        while (true)
        {
            // Select a random bark.
            index = Random.Range(0, barks.Length);
            // Check if it has been used.
            if (PlayerPrefs.GetInt("Charon" + prefix + "BarkBeenUsed" + index) != 1) break;
        }

        // Increment the number of barks used.
        PlayerPrefs.SetInt("Charon" + prefix + "BarksUsedCount", barksUsed + 1);
        // Mark the bark as being used.
        PlayerPrefs.SetInt("Charon" + prefix + "BarkBeenUsed" + index, 1);
        return barks[index];
    }

    private void ResetBarks(int length, string prefix)
    {
        PlayerPrefs.SetInt("Charon" + prefix + "BarksUsedCount", 0);
        for (int i = 0; i < length; i++)
        {
            PlayerPrefs.DeleteKey("Charon" + prefix + "BarkBeenUsed" + i);
        }
    }
}