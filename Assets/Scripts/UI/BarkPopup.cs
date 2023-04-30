using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarkPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text barkText;
    [SerializeField] private Image barkPortrait;

    public void SetSpeaker(string speaker)
    {
        speakerNameText.SetText(speaker);
    }

    public void SetText(string text)
    {
        barkText.SetText(text);
    }
}