using UnityEngine;

public class BarkManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The comma-separated list of damage-provoked barks to pull from.")]
    private TextAsset listOfDamageBarks;
    [SerializeField]
    [Tooltip("The comma-separated list of ambience-provoked barks to pull from.")]
    private TextAsset listOfAmbienceBarks;
    [SerializeField]
    [Tooltip("The character sequence to use to split barks by.")]
    private string delimiter = ",";

    public string GetDamageBark()
    {
        return GetBark(listOfDamageBarks, "Damage");
    }

    public string GetAmbienceBark()
    {
        return GetBark(listOfAmbienceBarks, "Ambience");
    }

    private string GetBark(TextAsset list, string prefix)
    {
        string[] barks = list.text.Split(delimiter);

        // Check if all barks have been used. If they have been, reset them.
        if (PlayerPrefs.GetInt(prefix + "BarksUsedCount") >= barks.Length)
            ResetBarks(barks.Length, prefix);

        while (true)
        {
            // Select a random bark.
            int index = Random.Range(0, barks.Length);
            // Check if it has been used.
            if (PlayerPrefs.GetInt(prefix + "BarkBeenUsed" + index) == 1)
            {
                // The bark has been used. Select another bark.
                continue;
            }
            return barks[index];
        }
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
