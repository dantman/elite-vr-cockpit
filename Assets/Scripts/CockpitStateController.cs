using UnityEngine;

namespace EVRC
{
    public class CockpitStateController : MonoBehaviour
    {
        [SerializeField]
        private bool _editLocked = true;
        public bool editLocked
        {
            get
            {
                return _editLocked;
            }
        }

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

        /**
         * Toggle the editLocked state
         */
        public bool ToggleEditLocked()
        {
            SetEditLocked(!_editLocked);
            return _editLocked;
        }

        /**
         * Set the editLocked state
         */
        public void SetEditLocked(bool editLocked)
        {
            if (_editLocked != editLocked)
            {
                _editLocked = editLocked;
            }
        }
    }
}
