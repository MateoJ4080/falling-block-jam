using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button optionsBackButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    void Start()
    {
        if (AudioManager.Instance.musicMainMenu != null) AudioManager.Instance.PlayMusic(AudioManager.Instance.musicMainMenu);

        ShowMainMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}