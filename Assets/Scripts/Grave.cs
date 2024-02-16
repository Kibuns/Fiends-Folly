using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    private float timer;

    private float interval = 1f;

    public int coinsPerSecond;



    private void Awake()
    {
        timer = 0f;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the timer by the time elapsed since the last frame
        timer += Time.deltaTime;

        // Check if one second has passed
        if (timer >= interval)
        {
            GameManager.Instance.AddCoins(coinsPerSecond);
            timer = 0f;
        }
    }
}
