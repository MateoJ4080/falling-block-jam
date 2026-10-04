using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _optionsBackButton;

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
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
    }

    public void Leave() => Application.Quit();
}