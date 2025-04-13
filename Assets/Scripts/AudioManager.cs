using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource themeSource, sfxSource;
    public AudioClip sfx1, sfx2, sfx3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void PlayMainTheme(AudioClip audioClip)
    {
        themeSource.clip = audioClip;    
    }

    public void PlaceItem()
    {
        sfxSource.clip = sfx1;
        sfxSource.Play();  
    }

    public void MenuButton()
    {
        sfxSource.clip = sfx2;
        sfxSource.Play();
    }


}
