using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject comingSoonPanel;
    public GameObject exitPanel;

    private bool isComingSoonActive;

    private void Start()
    {
        // Prevent a previous scene's pause state from carrying over.
        Time.timeScale = 1f;

        if (exitPanel != null)
            exitPanel.SetActive(false);

        if (comingSoonPanel != null)
            comingSoonPanel.SetActive(false);

        isComingSoonActive = false;
    }

    private void Update()
    {
        if (comingSoonPanel != null &&
            comingSoonPanel.activeSelf)
        {
            isComingSoonActive = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenExitPanel();
        }

        if (exitPanel != null && exitPanel.activeSelf)
        {
            Time.timeScale = 0f;
        }
    }

    private void OpenExitPanel()
    {
        if (exitPanel == null || exitPanel.activeSelf)
            return;

        if (comingSoonPanel != null &&
            comingSoonPanel.activeSelf)
        {
            isComingSoonActive = true;
            comingSoonPanel.SetActive(false);
        }

        exitPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnExitYes()
    {
        Time.timeScale = 1f;

        Debug.Log("Game closed.");
        Application.Quit();
    }

    public void OnExitNo()
    {
        if (exitPanel != null)
            exitPanel.SetActive(false);

        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(isComingSoonActive);
        }

        Time.timeScale = 1f;
    }
}