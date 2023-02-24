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
        string GetHandedBindingName(ActionsController.InputAction inputAction, InputBindingNameInfoManager.NameType nameType, ActionsController.Hand hand);
        List<Hand> GetBindingHands(InputAction inputAction);
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
