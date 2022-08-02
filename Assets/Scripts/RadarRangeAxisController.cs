using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Controller for the Radar Range (sensor zoom) axis
     */
    public class RadarRangeAxisController : AxisController
    {
        protected override void SetValue(float value)
        {
            vJoyInterface.instance.SetRadarRange(value);
        }
    }
}
