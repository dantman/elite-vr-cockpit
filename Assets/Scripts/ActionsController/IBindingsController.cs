namespace EVRC
{
    using static EVRC.ActionsController;
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
        string[] GetBindingNames(ActionsController.InputAction inputAction, NameType nameType);
        string GetHandedBindingName(ActionsController.InputAction inputAction, NameType nameType, ActionsController.Hand hand);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
