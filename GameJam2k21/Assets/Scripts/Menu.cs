
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        AudioSource bgMusic = FindObjectOfType<AudioSource>();
        DontDestroyOnLoad(bgMusic.gameObject);
        foreach (var btn in FindObjectOfType<Canvas>().GetComponentsInChildren<Button>())
        {
            if (btn.name.Contains("Play"))
            {
                btn.onClick.AddListener(() => Play());
            }
            else if (btn.name.Contains("Quit"))
            {
                btn.onClick.AddListener(() => ExitGame());
            }
        }
    }

    void Play()
    {
        SceneManager.LoadScene("Main_Level", LoadSceneMode.Single);
    }

    void ExitGame()
    {
        Application.Quit();
    }
}