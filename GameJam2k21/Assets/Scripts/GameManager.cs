using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<PanelBehaviour> panels = new List<PanelBehaviour>();
    int nbPanelTagged = 0;
    float roundSeconds = 5;
    int roundNumber = 0;
    int nbPanelsToTag = 3;

    Text[] textPanels;
    Text timerText;
    Text objectiveText;
    Text roundText;

    // Start is called before the first frame update
    void Start()
    {
        textPanels = FindObjectsOfType<Text>();
        for (int i = 0; i < textPanels.Length; i++)
        {
            Text current = textPanels[i];
            string textName = current.name.ToLower();
            if (textName.Contains("timer"))
            {
                timerText = current;
            }
            if (textName.Contains("objective"))
            {
                objectiveText = current;
                updateObjectiveText();
            }
            if (textName.Contains("round"))
            {
                roundText = current;
                updateRoundText();
            }
        }
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

        if (roundInProgress())
        {
            updateTimerText();
        }
        else
        {
            nextRound();
        }
    }

    void TriggerPanel(string panelName)
    {
        print(panelName);
        nbPanelTagged++;
        print(nbPanelTagged);
    }

    private bool roundInProgress()
    {
        return roundSeconds > 0;
    }

    private void nextRound()
    {
        roundNumber++;
        roundSeconds += 5;
        updateRoundText();
        updateObjectiveText();
        updatePanels();
    }

    private void updateRoundText()
    {
        roundText.text = string.Format("Ronde {0} ", roundNumber);
    }

    private void updateObjectiveText()
    {
        objectiveText.text = string.Format("{0}/{1}", nbPanelTagged, nbPanelsToTag);
    }

    private void updateTimerText()
    {
        roundSeconds -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(roundSeconds / 60);
        if (roundSeconds >= 0)
        {
            timerText.text = string.Format("{0}:{1}", minutes, Mathf.RoundToInt(roundSeconds - (minutes * 60)));
        }
    }

    private void updatePanels()
    {
        // do stuff
    }
}
