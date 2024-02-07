using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] public AudioClip typingSound;
    [SerializeField] public List<Sentence> sentences = new List<Sentence>();
}