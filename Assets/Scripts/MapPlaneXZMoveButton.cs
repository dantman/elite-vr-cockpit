using UnityEngine;

namespace EVRC
{
    public class MapPlaneXZMoveButton : BaseButton
    {
        public MapHorizontalPlaneController mapHorizontalPlaneController;
        public Vector2 setAxis;
        protected override Unpress Activate()
        {
            if (!mapHorizontalPlaneController.Reserve()) return () => { };

            mapHorizontalPlaneController.SetAxis(setAxis);

            return () => mapHorizontalPlaneController.Release();
        }
    }
}
