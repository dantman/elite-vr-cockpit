using UnityEngine;

namespace EVRC
{
    public class EscButton : BaseButton
    {
        public override void Activate()
        {
            KeyboardInterface.SendEscape();
        }
    }
}
