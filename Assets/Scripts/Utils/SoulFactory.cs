using UnityEngine;

public class SoulFactory : MonoBehaviour
{
    [Tooltip("The comma-separated list of descriptors to generate names from.")]
    [SerializeField] private NameGenerator _nameGenerator;
    
    [Tooltip("The comma-separated list of descriptors to generate names from.")]
    [SerializeField] private TextAsset _specials;

    [Tooltip("The comma-separated list of descriptors to generate names from.")]
    [SerializeField] private double _specialSoulChance = 0.05;

    public Soul GenerateSoul()
    {
        System.Random random = new System.Random();
        double roll = random.NextDouble();
        if (roll < _specialSoulChance)
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
        Object info = ;
        if (name == "%player_name%")
            name = PlayerPrefs.GetString("Name");
        name = (name == "") ? "You" : name;
        return new Soul(name, false, info.);
    }

    public Soul GenerateNormalSoul()
    {
        string name = _nameGenerator.GenerateName();
        return new Soul(name, true, barks);
    }
}

public class Soul
{
    public readonly string Name;
    public readonly bool IsSpecial;
    private readonly string[] Barks;

    public Soul(string name, bool isSpecial, string[] barks)
    {
        Name = name;
        IsSpecial = isSpecial;
        Barks = barks;
    }

    public string GetBark()
    {
        System.Random random = new System.Random();
        int index = random.Next(0, Barks.Length);
        return Barks[index];
    }
}
