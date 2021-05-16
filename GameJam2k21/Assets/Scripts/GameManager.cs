using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] panelPrefabs;
    public int NbOfPanelsToActivate = 3;
    public float lvlSpeedMultiplier = 1.10f;
    public float lvlTurnMultiplier = 1f;
    List<PanelBehaviour> panels = new List<PanelBehaviour>();
    int nbPanelTagged = 0;
    public int nbPanelsToTag = 3;
    float baseRoundSeconds = 120;
    public float roundSeconds;
    int roundNumber = 0;

    Text timerText;
    Text objectiveText;
    Text roundText;
    Image nextRoundPanel;
    Image endGamePanel;
    Text endGameScore;

    bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        roundSeconds = baseRoundSeconds;
        GameObject.FindObjectOfType<AudioManager>().Play("Soundtrack");
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
                Debug.Log(nextRoundPanel);
                nextRoundPanel.gameObject.SetActive(false);
            }
            if (imageName.Contains("end"))
            {
                endGamePanel = current;
                endGameScore = GameObject.FindGameObjectWithTag("EndScore").GetComponent<Text>();
                Debug.Log(endGameScore);
                endGamePanel.gameObject.SetActive(false);
            }
        }

        updatePanelsBehaviourList();
        if (panels.Count != 0)
        {
            foreach (PanelBehaviour p in panels)
            {
                p.addListener(TriggerPanel);
            }
            updatePanels();
        }
        GameObject.FindObjectOfType<AudioManager>().Play("Soundtrack");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        }

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
        endGameScore.text = string.Format("Vous avez défendu le français pendant {0} nuit{1}!", roundNumber, roundNumber > 1 ? "s" : "");
        yield return new WaitForSeconds(endGamePanel.GetComponent<Animation>().clip.length);
        yield return new WaitUntil(() => Input.anyKeyDown);
        GameObject.FindObjectOfType<AudioManager>().Stop("Soundtrack");
        AsyncOperation promise = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private IEnumerator nextRoundPanelRoutine()
    {
        nextRoundPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(nextRoundPanel.GetComponent<Animation>().clip.length);
        updatePanels();
        nextRoundPanel.gameObject.SetActive(false);
        nbPanelTagged = 0;
        roundNumber++;
        updateRoundText();
        updateObjectiveText();
        baseRoundSeconds -= 10;
        roundSeconds = baseRoundSeconds;
    }

    private void updateRoundText()
    {
        roundText.text = string.Format("Ronde {0} ", roundNumber);
    }

    private void updateObjectiveText()
    {
        objectiveText.text = string.Format("{0} sur {1}", nbPanelTagged, nbPanelsToTag);
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
        updatePanelsBehaviourList();
        resetPanels();
        List<int> panelsToActivate = new List<int>();
        for (int i = 0; i < NbOfPanelsToActivate; i++)
        {
            panelsToActivate.Add(getPanelIndex(panelsToActivate));
        }
        panelsToActivate.ForEach((index) =>
        {
            int panelType = Random.Range(0, panelPrefabs.Length);
            GameObject currentPanel = panels[index].transform.parent.gameObject;
            GameObject newPanel = Instantiate(panelPrefabs[panelType], currentPanel.transform.position, currentPanel.transform.rotation);
            newPanel.transform.parent = currentPanel.transform.parent;
            newPanel.transform.position = currentPanel.transform.position;
            newPanel.transform.rotation = currentPanel.transform.rotation;
            newPanel.transform.SetSiblingIndex(currentPanel.transform.GetSiblingIndex());
            Destroy(currentPanel);
            PanelBehaviour newPanelBehaviour = newPanel.GetComponentInChildren<PanelBehaviour>();
            newPanelBehaviour.addListener(TriggerPanel);
            newPanelBehaviour.Activate();
        });
    }

    private int getPanelIndex(List<int> usedIndexes)
    {
        int panelIndex = Random.Range(0, panels.Count);
        while (NbOfPanelsToActivate <= panels.Count && usedIndexes.Contains(panelIndex))
        {
            panelIndex = Random.Range(0, panels.Count);
        }
        return panelIndex;
    }

    private void updatePanelsBehaviourList()
    {
        panels = new List<PanelBehaviour>(FindObjectsOfType<PanelBehaviour>(true));
    }

    private void resetPanels()
    {
        panels.ForEach((p) => p.Deactivate());
    }
}
