using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float spinSpeed;

    void Update()
    {
        transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y + spinSpeed * Time.deltaTime, transform.localEulerAngles.z);
    }
}
