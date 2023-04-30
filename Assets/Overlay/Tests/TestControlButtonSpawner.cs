using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class TestControlButtonSpawner : MonoBehaviour
    {
        public GameObject testObject;

        public void TestSpawn()
        {
            var to = Instantiate(testObject);
            to.transform.localPosition = OverlayUtils.GetSpawnLocation(Vector3.zero, to);
        }
    }
}
