using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    [Header("Audio Sources")]
    public AudioSource MusicSource;
    public AudioSource SfxSource;

    [Header("Music")]
    public AudioClip MusicMainMenu;
    public AudioClip MusicGameplay;

    [Header("Sound Effects")]
    public AudioClip SfxRotate;
    public AudioClip SfxClearLine;
    public AudioClip SfxMove;
    public AudioClip SfxGameOver;
    public AudioClip SfxButtonClick;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetMusicVolume(0.5f);
        SetSfxVolume(0.5f);
    }

    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SfxSource.PlayOneShot(clip);
    }

    public void PlayButtonSFX()
    {
        SfxSource.PlayOneShot(SfxButtonClick);
    }

    // Assigned to slider in the inspector
    public void SetMusicVolume(float value)
    {
        MusicSource.volume = value * 0.5f;
    }

    // Assigned to slider in the inspector
    public void SetSfxVolume(float value)
    {
        SfxSource.volume = value * 0.5f;
    }
}