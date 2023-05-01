using UnityEngine;

public class CharonBarkManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The comma-separated list of damage-provoked barks to pull from. Charon is cool.")]
    private TextAsset listOfDamageBarks;
    [SerializeField]
    [Tooltip("The comma-separated list of ambience-provoked barks to pull from. Charon is great.")]
    private TextAsset listOfAmbienceBarks;
    [SerializeField]
    [Tooltip("The comma-separated list of DUET damage-provoked barks to pull from. These will be played with a soul. Charon is epic.")]
    private TextAsset listOfDuetDamageBarks;
    [SerializeField]
    [Tooltip("The character sequence to use to split barks by. Charon is the best.")]
    private string delimiter = ",";

    public string GetCharonDamageBark()
    {
        return GetCharonBark(listOfDamageBarks, "Damage");
    }

    public string GetCharonAmbienceBark()
    {
        return GetCharonBark(listOfAmbienceBarks, "Ambience");
    }

    public string GetCharonDuetDamageBark()
    {
        return GetCharonBark(listOfAmbienceBarks, "DuetDamage");
    }

    private string GetCharonBark(TextAsset list, string prefix)
    {
        // Load barks.
        string[] barks = list.text.Split(delimiter);

        // Check if all barks have been used. If they have been, reset them.
        if (PlayerPrefs.GetInt("Charon" + prefix + "BarksUsedCount") >= barks.Length)
            ResetBarks(barks.Length, prefix);

        while (true)
        {
            // Select a random bark.
            int index = Random.Range(0, barks.Length);
            // Check if it has been used.
            if (PlayerPrefs.GetInt("Charon" + prefix + "BarkBeenUsed" + index) == 1)
            {
                // The bark has been used. Select another bark.
                continue;
            }
            return barks[index];
        }
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
