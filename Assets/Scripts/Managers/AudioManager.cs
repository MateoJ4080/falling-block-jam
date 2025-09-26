using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip musicMainMenu;
    public AudioClip musicGameplay;

    [Header("Sound Effects")]
    public AudioClip sfxRotate;
    public AudioClip sfxClearLine;
    public AudioClip sfxMove;
    public AudioClip sfxGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Assigned to slider in the inspector
    public void SetMusicVolume(float value)
    {
        musicSource.volume = value * 0.05f;

    }

    // Assigned to slider in the inspector
    public void SetSfxVolume(float value)
    {
        sfxSource.volume = value * 0.05f;
    }
}