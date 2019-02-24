namespace EVRC
{
    public class MapPlaneYMoveButton : BaseButton
    {
        public MapPlaneController mapPlaneController;
        public float setAxis;
        protected override Unpress Activate()
        {
            mapPlaneController.SetYAxis(setAxis);
            return () => mapPlaneController.SetYAxis(0);
        }
    }
}
