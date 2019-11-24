namespace EVRC
{
    public enum BindingsHintCategory
    {
        Default,
        Menu,
        UI,
        CockpitControls,
    }

    public interface IBindingsController
    {
        float GetTrackpadSwipeInterval(ActionsController.Hand hand);
        string[] GetBindingNames(ActionsController.InputAction inputAction, InputBindingNameInfoManager.NameType nameType);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
