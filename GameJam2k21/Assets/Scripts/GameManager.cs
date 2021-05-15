using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<PanelBehaviour> panels = new List<PanelBehaviour>();
    int nbPanelTagged = 0;
    // Start is called before the first frame update
    void Start()
    {
        panels = new List<PanelBehaviour>(FindObjectsOfType<PanelBehaviour>());
        if (panels.Count != 0)
        {
            foreach (PanelBehaviour p in panels)
            {
                p.addListener(TriggerPanel);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (panels.Count == 0)
            panels = new List<PanelBehaviour>(FindObjectsOfType<PanelBehaviour>());
    }

    void TriggerPanel(string panelName)
    {
        print(panelName);
        nbPanelTagged++;
        print(nbPanelTagged);
    }
}
