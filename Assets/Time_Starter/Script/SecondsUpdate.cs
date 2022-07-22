using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondsUpdate : MonoBehaviour
{
    float timeOffset;
    bool started;

    void Update()
    {
        if (!started)
        {
            timeOffset = Time.realtimeSinceStartup;
            started = true;
        }

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Time.realtimeSinceStartup - timeOffset);
    }
}
