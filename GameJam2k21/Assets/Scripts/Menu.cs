using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{

    Image bg;
    Image tuto;

    // Start is called before the first frame update
    void Start()
    {
        Image[] images = FindObjectsOfType<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            Image current = images[i];
            Debug.Log(current);
            if (current.name.Contains("Bg"))
            {
                bg = current;
            }
            if (current.name.Contains("Tuto"))
            {
                tuto = current;
            }
        }


        GameObject.FindObjectOfType<AudioManager>().Play("Menu");
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
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        tuto.gameObject.SetActive(true);
        bg.gameObject.SetActive(false);
        yield return new WaitUntil(() => Input.anyKeyDown);
        GameObject.FindObjectOfType<AudioManager>().Stop("Menu");
        SceneManager.LoadScene("Main_Level", LoadSceneMode.Single);
    }

    void ExitGame()
    {
        Application.Quit();
    }
}