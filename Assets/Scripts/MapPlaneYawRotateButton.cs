namespace EVRC
{
    public class MapPlaneYawRotateButton : BaseButton
    {
        public MapPlaneController mapPlaneController;
        public float setAxis;
        protected override Unpress Activate()
        {
            mapPlaneController.SetYaw(setAxis);
            return () => mapPlaneController.SetYaw(0);
        }
    }
}
