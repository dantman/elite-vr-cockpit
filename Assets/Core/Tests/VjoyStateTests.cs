using UnityEngine;
using UnityEngine.UIElements;
using NUnit.Framework;
using System.Reflection;
using UnityEngine.TestTools;
using System.Collections;
using EVRC.Core;

namespace CoreTests
{
    public class VjoyStateTests
    {
        private VJoyState vJoyState;

        [SetUp]
        public void Setup()
        {
            vJoyState = ScriptableObject.CreateInstance<VJoyState>();
        }


        [Test]
        public void GetStatusText_ForEachVJoyStatus_IsNotUnknown()
        {
            foreach (VJoyStatus status in System.Enum.GetValues(typeof(VJoyStatus)))
            {
                vJoyState.vJoyStatus = status;
                string returnedStatus = vJoyState.GetStatusText();
                if (status != VJoyStatus.Unknown)
                {
                    Assert.AreNotEqual("Unknown", returnedStatus);
                }
            }
        }

      
    }
}
