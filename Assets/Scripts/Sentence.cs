using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;


[System.Serializable]
public class Sentence
{
    [SerializeField] public string text = "Sample Text";
    [Range(0.3f, 8f)]
    [SerializeField] public float duration = 2.5f;
    [Range(0f, 1f)]
    [SerializeField] public float characterTypingTime = 0.1f;
    [SerializeField] public bool shouldShake;
    [Range(0f, 8f)]
    [SerializeField] public float shakeOffsetDelta;
    [SerializeField] public Color textColor = Color.white;

    // Constructor
    public Sentence(string text, float duration, float characterTypingTime, bool shouldShake, Color textColor)
    {
        this.text = text;
        this.duration = duration;
        this.characterTypingTime = characterTypingTime;
        this.shouldShake = shouldShake;
        this.textColor = textColor;
    }

    public Sentence() { }

    public string GetConvertedText()
    {
        // Define the pattern for matching {placeholder}
        string pattern = @"\{clocktext\}";

        // Replace all occurrences of {placeholder} with "foo"
        return Regex.Replace(text, pattern, GameManager.Instance.GetTimeString());
    }


}
