using UnityEngine;

namespace EVRC
{
    public class CockpitStateController : MonoBehaviour
    {
        public bool editLocked = true;

        public static CockpitStateController _instance;
        public static CockpitStateController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CockpitStateController>();

                    if (_instance == null)
                    {
                        var hmd = new GameObject("[CocpitStateController]");
                        _instance = hmd.AddComponent<CockpitStateController>();
                    }
                }

                return _instance;
            }
        }

    }
}
