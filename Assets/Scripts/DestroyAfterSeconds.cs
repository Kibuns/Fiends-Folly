using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyCoroutine(seconds));
    }

    private IEnumerator DestroyCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds - 0.05f);
        GetComponent<AudioSource>().Stop();
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }

}
