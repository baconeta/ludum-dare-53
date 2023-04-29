using System.Text;
using UnityEngine;


public class NameGenerator : MonoBehaviour
{
    [Header("NOTE: This script can be added for each type of thing that needs to be named!")]
    [Header("Word Lists")]
    [SerializeField]
    [Tooltip("The comma-separated list of descriptors to generate names from.")]
    private TextAsset listOfAdjectives;
    [SerializeField]
    [Tooltip("The comma-separated list of nouns to generate names from.")]
    private TextAsset listOfNouns;

    public string GenerateName()
    {
        // Get random name
        StringBuilder generatedName = new();
        generatedName.Append(GetRandomAdjectiveFromFile());
        generatedName.Append(GetRandomNounFromFile());

        return generatedName.ToString();
    }

    private string GetRandomAdjectiveFromFile()
    {
        string[] words = listOfAdjectives.text.Split(",");
        return words[Random.Range(0, words.Length)];
    }

    private string GetRandomNounFromFile()
    {
        string[] words = listOfNouns.text.Split(",");
        return words[Random.Range(0, words.Length)];
    }
}
