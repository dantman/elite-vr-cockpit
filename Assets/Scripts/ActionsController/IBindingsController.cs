namespace EVRC
{
    using Hand = ActionsController.Hand;
    using TrackpadInterval = ActionsController.TrackpadInterval;

    public enum BindingsHintCategory
    {
        Default,
        Menu,
        UI,
        CockpitControls,
    }

    public interface IBindingsController
    {
        TrackpadInterval GetTrackpadSwipeInterval(ActionsController.Hand hand);
        string[] GetBindingNames(ActionsController.InputAction inputAction, InputBindingNameInfoManager.NameType nameType);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
        IHaptics GetHapticsForHand(Hand hand);

    }
}
