using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public enum BindingsHintCategory
    {
        Default,
        Menu,
        UI,
        CockpitControls,
    }

    public interface IBindingsController
    {
        bool CanShowBindings();
        void ShowBindings(BindingsHintCategory hintCategory);
        void EditBindings();
    }
}
