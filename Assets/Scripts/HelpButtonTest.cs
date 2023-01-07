using EVRC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButtonTest : MonoBehaviour
{
    public void OnEnable()
    {
        CockpitSettingsState.instance.ChangeSettings(settings =>
        {
            settings.buttonLabelsEnabled = !settings.buttonLabelsEnabled;
        });
    }

}
