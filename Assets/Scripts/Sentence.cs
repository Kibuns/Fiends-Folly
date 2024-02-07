using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "New Sentence", menuName = "Sentence")]
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
}
