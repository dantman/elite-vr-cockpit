using EVRC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WristTurnActivate : MonoBehaviour
{
    private float zLow = 100.0f;
    private float zHigh = 170.0f;
    private float yLow = 235.0f;
    private float yHigh = 295.0f;
    public GameObject target;

    void Update()
    {
        var euAngle = this.gameObject.transform.localRotation.eulerAngles;
        if (euAngle.z > zLow && euAngle.z <= zHigh && euAngle.y > yLow && euAngle.y < yHigh)
        {
            if (!target.activeInHierarchy)
            {
                target.SetActive(true);
            }
            return;
        }

        if (target.activeInHierarchy)
        {
            target.SetActive(false);
            return;
        }
    }
}
