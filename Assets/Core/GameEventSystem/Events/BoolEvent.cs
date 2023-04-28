using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/Bool Event"), System.Serializable]
    public sealed class BoolEvent : GameEvent<bool>
    {
        
    }
}