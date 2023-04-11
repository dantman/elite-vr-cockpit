using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Controller for the FSS mode radio tuning
     */
    public class FssTuningRadioAxisController : AxisController
    {
        protected override void SetValue(float value)
        {
            vJoyInterface.instance.SetFSSTuning(value);
        }
    }
}
