using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleScript : MonoBehaviour
{
    [Header("Candle Light Settings")]
    public float maxRangeDelta = 7f;  // Maximum range of the light
    public float maxIntensityDelta;
    public float minFlickerSpeed = 1f;  // Minimum speed of flickering
    public float maxFlickerSpeed = 2f;  // Maximum speed of flickering
    public float randomEventTime;
    public float noiseFrequency = 1f;  // Frequency of the Perlin noise
    public float noiseMagnitude = 0.5f;

    [Header("Flame Settings")]
    public GameObject flame;
    public float maxFlameRangeDelta;


    private Light candleLight;
    private Light flameLight;
    private float baseFlameRange;
    private float baseCandleRange;
    private float baseCandleIntensity;
    private float flickerSpeed;
    private float timer;
    private bool isFlamePresent;

    void Start()
    {
        candleLight = GetComponent<Light>();

        baseCandleRange = candleLight.range;
        baseCandleIntensity = candleLight.intensity;
        if (flame != null)
        {
            flameLight = flame.GetComponent<Light>();
            baseFlameRange = flameLight.range;
            isFlamePresent = true;
        }

        float randomTimeOffset = Random.Range(0f, 100f);
        timer = randomTimeOffset;

        StartCoroutine(RandomEventCoroutine());


        //if (candleLight.lightmapBakeType != LightmapBakeType.Realtime) //lightmapBakeType property is editor only for some reason
        //{
        //    Debug.LogWarning("Light component is not set to Realtime.");
        //}
        if (candleLight == null)
        {
            Debug.LogError("Light component cannot be found on GameObject");
        }

        if (flameLight == null)
        {
            Debug.LogError("No Flame Found");
            isFlamePresent = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime * flickerSpeed;
        // Calculate flicker using a sine wave
        float flicker = Mathf.Sin(timer) + Mathf.PerlinNoise(Time.time * noiseFrequency, 0) * noiseMagnitude;

        // Adjust light range and intensity based on flicker
        candleLight.range = baseCandleRange + flicker * maxRangeDelta;
        candleLight.intensity = baseCandleIntensity + flicker * maxIntensityDelta;
        if(!isFlamePresent) { return; }
        flameLight.range = baseFlameRange + flicker * maxFlameRangeDelta;
    }

    private IEnumerator RandomEventCoroutine()
    {
        while (true)
        {
            RandomEvent();
            yield return new WaitForSeconds(randomEventTime);
        }
    }

    private void RandomEvent()
    {
        flickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
    }
}
