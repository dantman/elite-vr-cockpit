using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    public class CockpitModeManager : MonoBehaviour
    {
        private List<CockpitModeAnchor> modeAnchors;

        internal void OnEnable()
        {
            modeAnchors = FindObjectsOfType<CockpitModeAnchor>(true).ToList();
        }


    }
}
