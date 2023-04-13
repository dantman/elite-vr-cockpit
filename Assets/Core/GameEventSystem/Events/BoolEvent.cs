using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/bool"), System.Serializable]
    public sealed class BoolEvent : GameEvent<bool>
    {
    }
}