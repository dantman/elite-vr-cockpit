namespace EVRC.Core.Overlay
{
    public class EscButton : BaseButton
    {
        protected override Unpress Activate()
        {
            var unpress = KeyboardInterface.CallbackPress(KeyboardInterface.Escape());
            return () => unpress();
        }
    }
}
