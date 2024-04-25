using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] bookFallClips;
    [SerializeField] private float maxVolume;
    [SerializeField] private float playClipCooldown;
    private AudioSource source;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(timer < playClipCooldown) { return; }
        timer = 0f;
        float collisionMag = collision.relativeVelocity.magnitude;
        Debug.Log(collisionMag);
        AudioClip clip = bookFallClips[Random.Range(0, bookFallClips.Length - 1)];
        float volume = (collisionMag / 4) * maxVolume;
        source.PlayOneShot(clip, volume);
    }
}
