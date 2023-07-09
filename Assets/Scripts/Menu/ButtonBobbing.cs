using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBobbing : MonoBehaviour
{

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) * 0.1f + 0.5f, transform.position.z);
    }
}
