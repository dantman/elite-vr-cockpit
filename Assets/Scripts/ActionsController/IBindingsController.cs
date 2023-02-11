namespace EVRC
{
    using static EVRC.ActionsController;
    using static EVRC.InputBindingNameInfoManager;
    using Valve.VR;
    using System.Collections.Generic;
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
        List<Hand> GetBindingHands(InputAction inputAction);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
