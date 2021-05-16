using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int NbOfPanelsToActivate = 3;
    public float lvlSpeedMultiplier = 1.10f;
    public float lvlTurnMultiplier = 1f;
    List<PanelBehaviour> panels = new List<PanelBehaviour>();
    int nbPanelTagged = 0;
    public int nbPanelsToTag = 3;
    public float roundSeconds = 120;
    int roundNumber = 0;

    Text timerText;
    Text objectiveText;
    Text roundText;
    Image nextRoundPanel;
    Image endGamePanel;

    bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get text objects of UI
        Text[] textPanels = FindObjectsOfType<Text>();
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
        // Get image objects of UI
        Image[] images = FindObjectsOfType<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            Image current = images[i];
            string imageName = current.name.ToLower();
            if (imageName.Contains("next"))
            {
                nextRoundPanel = current;
                nextRoundPanel.gameObject.SetActive(false);
            }
            if (imageName.Contains("end"))
            {
                endGamePanel = current;
                endGamePanel.gameObject.SetActive(false);
            }
        }

        panels = new List<PanelBehaviour>(FindObjectsOfType<PanelBehaviour>());
        if (panels.Count != 0)
        {
            foreach (PanelBehaviour p in panels)
            {
                p.addListener(TriggerPanel);
                p.Deactivate();
            }
            updatePanels();
        }
        GameObject.FindObjectOfType<AudioManager>().Play("Soundtrack");
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
        else if (!gameEnded)
        {
            endGame();
        }
    }

    void TriggerPanel(string panelName)
    {
        print(panelName);
        nbPanelTagged++;
        updateObjectiveText();
        if (nbPanelTagged == nbPanelsToTag)
        {
            nextRound();
        }
    }

    private bool roundInProgress()
    {
        return roundSeconds > 0;
    }

    private void nextRound()
    {
        nbPanelTagged = 0;
        roundNumber++;
        roundSeconds += 5;
        updateRoundText();
        updateObjectiveText();
        updatePanels();
        StartCoroutine(nextRoundPanelRoutine());
        GameObject.FindObjectOfType<PlayerMovementRB>().lvlSpeedMultiplier *= lvlSpeedMultiplier;
        // GameObject.FindObjectOfType<PlayerMovementRB>().lvlTurnMultiplier *= lvlTurnMultiplier;
    }

    private void endGame()
    {
        gameEnded = true;
        StartCoroutine(endGameRoutine());
    }

    private IEnumerator endGameRoutine()
    {
        endGamePanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(endGamePanel.GetComponent<Animation>().clip.length);
        yield return new WaitUntil(() => Input.anyKeyDown);
        AsyncOperation promise = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private IEnumerator nextRoundPanelRoutine()
    {
        nextRoundPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(nextRoundPanel.GetComponent<Animation>().clip.length);
        nextRoundPanel.gameObject.SetActive(false);
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
        resetPanels();
        List<int> panelsToActivate = new List<int>();
        for (int i = 0; i < NbOfPanelsToActivate; i++)
        {
            panelsToActivate.Add(getPanelIndex(panelsToActivate));
        }
        panelsToActivate.ForEach((index) => panels[index].Activate());
    }

    private int getPanelIndex(List<int> usedIndexes)
    {
        int panelIndex = Random.Range(0, panels.Count);
        while (NbOfPanelsToActivate < panels.Count && usedIndexes.Contains(panelIndex))
        {
            panelIndex = Random.Range(0, panels.Count);
        }
        return panelIndex;
    }

    private void resetPanels()
    {
        panels.ForEach((p) => p.Deactivate());
    }
}
