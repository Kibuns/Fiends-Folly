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

    [Header("Pulse Settings")]
    public bool pulseButton;
    public float pulseDuration;
    public float maxPulseIntensityMultiplier;
    public float maxPulseRangeMultiplier;
    public float maxIntensityPauseDuration;


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
        if (pulseButton)
        {
            pulseButton = false;
            Pulse();
        }
        timer += Time.deltaTime * flickerSpeed;
        // Calculate flicker using a sine wave
        float flicker = Mathf.Sin(timer) + Mathf.PerlinNoise(Time.time * noiseFrequency, 0) * noiseMagnitude;

        // Adjust light range and intensity based on flicker
        candleLight.range = baseCandleRange + flicker * maxRangeDelta;
        candleLight.intensity = baseCandleIntensity + flicker * maxIntensityDelta;
        if(!isFlamePresent) { return; }
        flameLight.range = baseFlameRange + flicker * maxFlameRangeDelta;
    }

    public void Pulse()
    {
        StartCoroutine(PulseSequence(pulseDuration));
    }

    private IEnumerator PulseSequence(float seconds)
    {
        // Store the original values
        float originalIntensity = baseCandleIntensity;
        float originalRange = baseCandleRange;

        // Calculate the target values (2x larger)
        float targetIntensity = originalIntensity * maxPulseIntensityMultiplier;
        float targetRange = originalRange * maxPulseRangeMultiplier;

        // Gradually increase the baseCandleIntensity and baseCandleRange
        float elapsedTime = 0f;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / seconds);

            // Interpolate between original and target values
            baseCandleIntensity = Mathf.Lerp(originalIntensity, targetIntensity, t);
            baseCandleRange = Mathf.Lerp(originalRange, targetRange, t);

            // Wait for the next frame
            yield return null;
        }

        // Ensure final values are exactly the target values
        baseCandleIntensity = targetIntensity;
        baseCandleRange = targetRange;

        // Wait for some time with increased intensity and range (optional)
        yield return new WaitForSeconds(maxIntensityPauseDuration);

        // Gradually go back to the original values
        elapsedTime = 0f;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / seconds);

            // Interpolate between target and original values
            baseCandleIntensity = Mathf.Lerp(targetIntensity, originalIntensity, t);
            baseCandleRange = Mathf.Lerp(targetRange, originalRange, t);

            // Wait for the next frame
            yield return null;
        }

        // Ensure final values are exactly the original values
        baseCandleIntensity = originalIntensity;
        baseCandleRange = originalRange;
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
