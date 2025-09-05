using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPanelTracker : MonoBehaviour
{
    // ----------------------
    //
    // This object keeps track of whether or not all of its "Speed Panels" have been activated, for gates and other similar objects
    //
    // ----------------------

    [Space(10)]
    public List<SpeedPanel> SpeedPanels = new List<SpeedPanel>();

    [Space(10)]
    public MonoBehaviour TriggerObject;

    private void Update()
    {
        int activatedPanels = 0;
        foreach (SpeedPanel panel in SpeedPanels)
        {
            if (panel.IsActivated)
            {
                activatedPanels++;
            }
        }

        // Open gate, or trigger other miscellaneous code, if all panels have been activated
        if (activatedPanels >= SpeedPanels.Count)
        {
            foreach (SpeedPanel panel in SpeedPanels)
            {
                panel.DisablePanel();
            }

            switch (TriggerObject)
            {
                case Gate triggerGate:
                    triggerGate.Open();
                    break;
            }

            enabled = false;
        }
    }
}
