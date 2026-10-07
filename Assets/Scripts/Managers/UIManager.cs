using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _optionsPanel;

    [Header("Audio")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            _musicSlider.onValueChanged.RemoveAllListeners();
            _sfxSlider.onValueChanged.RemoveAllListeners();

            _musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            _sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        }

        if (AudioManager.Instance.MusicMainMenu != null) AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicMainMenu);

        ShowMainMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowOptions()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        _optionsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    public void Leave() => Application.Quit();
}