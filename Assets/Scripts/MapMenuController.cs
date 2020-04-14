namespace EVRC
{
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;

    /**
     * Controller for outputting UI navigation keypresses from trackpad/joystick input for the map views
     */
    public class MapMenuController : UIMenuController
    {
        protected override void Back()
        {
            var bindings = EDStateManager.instance.controlBindings;
            if (bindings != null && bindings.HasKeyboardKeybinding(EDControlButton.GalaxyMapOpen))
            {
                // On the Galaxy map this will exit
                // On the System map/orrery this will go to the galaxy map, from where you can exit
                EDControlBindings.GetControlButton(EDControlButton.GalaxyMapOpen)?.Send();
            }
            else
            {
                SendEscape();
            }
        }
    }
}
