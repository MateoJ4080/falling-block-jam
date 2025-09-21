using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    [Header("Audio Sources")]
    [SerializeField] AudioMixer mixer;

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
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("Music", Mathf.Log10(value) * 20);
    }

    // Assigned to slider in the inspector
    public void SetSfxVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }
}