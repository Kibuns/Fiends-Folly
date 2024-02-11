using UnityEngine;

public class ClockScript : MonoBehaviour
{
    [SerializeField] private Transform MinuteHand;
    [SerializeField] private Transform HourHand;

    private float secondsInHalfDay;


    private void Start()
    {
        secondsInHalfDay = GameManager.instance.secondsInHalfDay;
    }
    private void Update()
    {
        //timer += Time.deltaTime;
        float timer = GameManager.instance.timer;
        // Calculate rotation angles for hour and minute hands
        float hoursPassed = (timer / secondsInHalfDay) * 12f;
        float minutesPassed = (timer / secondsInHalfDay) * 720f; // 12 hours * 60 minutes

        // Calculate rotation angles
        float hourRotation = hoursPassed * 30f; // Each hour is 30 degrees (360 degrees / 12 hours)
        float minuteRotation = hourRotation * 12f;

        HourHand.localEulerAngles = new Vector3(0f, 0f, hourRotation); // Invert rotation for correct direction
        MinuteHand.localEulerAngles = new Vector3(0f, 0f, minuteRotation); // Invert rotation for correct direction
    }

   
}
