using UnityEngine;

namespace EVRC
{
    public class EscButton : BaseButton
    {
        public override void Activate()
        {
            if (!KeyboardInterface.Send("Key_Escape"))
            {
                Debug.LogWarning("Could not send keypress Key_Escape, did not understand the key");
            }
        }
    }
}
