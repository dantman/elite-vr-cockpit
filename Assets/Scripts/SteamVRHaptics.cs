using Valve.VR;

namespace EVRC
{
    using Hand = ActionsController.Hand;

    public class SteamVRHaptics : IHaptics
    {
        SteamVR_Action_Vibration hapticAction = new SteamVR_Action_Vibration();
        SteamVR_Input_Sources inputSource;

        public SteamVRHaptics(Hand hand)
        {
            inputSource = ActionsController_SteamVRInputBindings.GetInputSourceForHand(hand);
        }

        public void ThrottleDetent()
        {
            hapticAction.Execute(0, 0.15f, 20, 0.1f, inputSource);
        }
    }
}
