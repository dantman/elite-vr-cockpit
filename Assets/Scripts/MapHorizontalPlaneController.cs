using System;
using UnityEngine;

namespace EVRC
{
    /**
     * MapPlaneController component that controls the XZ translation
     */
    public class MapHorizontalPlaneController : MonoBehaviour
    {
        public MapPlaneController planeController;
        public bool Active { get; set; } = false;

        public bool Reserve()
        {
            if (Active) return false;
            Active = true;
            return true;
        }

        public void Release()
        {
            if (!Active) throw new Exception("MapHorizontalPlaneController already released");
            SetAxis(Vector2.zero);
            Active = false;
        }

        public void SetAxis(Vector2 axis)
        {
            planeController.SetXZAxis(axis);
        }
    }
}
