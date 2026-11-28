using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject aboutPanel;
    public GameObject loadingPanel;

    private bool isLoading;

    private void Start()
    {
        Time.timeScale = 1f;

        if (menuPanel != null)
            menuPanel.SetActive(true);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (aboutPanel != null)
            aboutPanel.SetActive(false);

        isLoading = false;
    }

    public void OnStartGame()
    {
        if (isLoading)
            return;

        isLoading = true;

        Debug.Log("Starting Game");

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (aboutPanel != null)
            aboutPanel.SetActive(false);

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        // Allow the loading panel to render for at least one frame.
        yield return null;

        SceneManager.LoadScene("GameScene");
    }

    public void OnBack()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    public void OnAbout()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (aboutPanel != null)
            aboutPanel.SetActive(true);

        Debug.Log("About AI opened.");
    }

    public void OnExit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}