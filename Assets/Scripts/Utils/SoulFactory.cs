using UnityEngine;

public class SoulFactory : MonoBehaviour
{
    [Header("Managers")]
    [Tooltip("The name generator that is to be used to generate names for normal souls.")]
    [SerializeField]
    private NameGenerator _soulNameGenerator;

    [Tooltip("The bark manager that is to be used to generate barks.")] [SerializeField]
    private BarkManager _barkManager;

    [Header("Delimiters")] [Tooltip("The pair-pair delimiter.")] [SerializeField]
    private string delimiter1 = ",";

    [Tooltip("The name-bark delimiter.")] [SerializeField]
    private string delimiter2 = ",";

    [Header("Other")]
    [Tooltip("The 2D delimited list of name-bark pairs to generate special souls from.")]
    [SerializeField]
    private TextAsset _specials;

    [Tooltip("The comma-separated list of descriptors to generate names from.")] [SerializeField]
    private double _specialSoulChance = 0.05;

    private string[,] specialSoulData;

    public void Start()
    {
        // Split the 2D delimited list once.
        string[] pairs = _specials.text.Split(delimiter1);
        string[,] soulData = new string[pairs.Length, 2];
        // Split each pair again.
        for (int i = 0; i < pairs.Length; i++)
        {
            string[] items = pairs[i].Split(delimiter2);
            if (items.Length != 2)
                continue;
            soulData[i, 0] = items[0].Trim();
            soulData[i, 1] = items[1].Trim();
        }

        // Save the result.
        specialSoulData = soulData;
    }

    public Soul GenerateRandomSoul()
    {
        System.Random random = new System.Random();
        if (random.NextDouble() < _specialSoulChance)
        {
            return GenerateSpecialSoul();
        }
        else
        {
            return GenerateNormalSoul();
        }
    }

    public Soul GenerateSpecialSoul()
    {
        // Check if all special souls have been used. If they have been, reset them.
        if (PlayerPrefs.GetInt("SpecialSoulUsedCount") >= specialSoulData.GetLength(0))
            ResetSouls(specialSoulData.GetLength(0));

        int index;
        while (true)
        {
            // Select a random special soul pair.
            index = Random.Range(0, specialSoulData.GetLength(0));
            // Check if it has been used.
            if (PlayerPrefs.GetInt("SpecialSoulBeenUsed" + index) != 1)
            {
                break;
            }
            // Otherwise, the special soul has been used. Select another one.
        }

        // Extract data for special soul.
        string name = specialSoulData[index, 0];
        string ambienceBark = specialSoulData[index, 1];

        // Replace special name with player name.
        if (name == "%player_name%")
            name = PlayerPrefs.GetString("Name");
        name = (name == "") ? "You" : name;
        
        PlayerPrefs.SetInt("SpecialSoulUsedCount", PlayerPrefs.GetInt("SpecialSoulUsedCount", 0) + 1);
        return new Soul(name, true, _barkManager, ambienceBark);
    }

    public Soul GenerateNormalSoul()
    {
        return new Soul(_soulNameGenerator.GenerateName(), false, _barkManager);
    }

    private void ResetSouls(int length)
    {
        PlayerPrefs.SetInt("SpecialSoulUsedCount", 0);
        for (int i = 0; i < length; i++)
        {
            PlayerPrefs.DeleteKey("SpecialSoulBeenUsed" + i);
        }
    }
}

public class Soul
{
    public readonly string Name;
    public readonly bool IsSpecial;
    private readonly BarkManager BarkManager;
    private readonly string AmbienceBarkOverride;

    public Soul(string name, bool isSpecial, BarkManager barkManager, string ambienceBarkOverride = "")
    {
        Name = name;
        IsSpecial = isSpecial;
        BarkManager = barkManager;
        AmbienceBarkOverride = ambienceBarkOverride;
    }

    public string GetDamageBark()
    {
        return BarkManager.GetDamageBark();
    }

    public string GetAmbienceBark()
    {
        if (IsSpecial)
        {
            return AmbienceBarkOverride;
        }

        return BarkManager.GetAmbienceBark();
    }

    public string GetDuetDamageBark()
    {
        return BarkManager.GetDuetDamageBark();
    }
}