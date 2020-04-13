using Valve.VR;

namespace EVRC
{
    using Hand = ActionsController.Hand;

    public class SteamVRHaptics : IHaptics
    {
        SteamVR_Action_Vibration hapticAction;
        SteamVR_Input_Sources inputSource;

        public SteamVRHaptics(SteamVR_Action_Vibration hapticAction, Hand hand)
        {
            this.hapticAction = hapticAction;
            inputSource = ActionsController_SteamVRInputBindings.GetInputSourceForHand(hand);
        }

        public void ThrottleDetent()
        {
            hapticAction.Execute(0, 0.15f, 10, 0.15f, inputSource);
        }
    }
}
