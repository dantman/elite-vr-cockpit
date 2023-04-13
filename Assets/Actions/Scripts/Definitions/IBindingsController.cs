namespace EVRC.Core.Actions
{
    using static ActionsController;
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
        TrackpadInterval GetTrackpadSwipeInterval(Hand hand);
        string[] GetBindingNames(InputAction inputAction, NameType nameType);
        string GetHandedBindingName(InputAction inputAction, NameType nameType, Hand hand);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
