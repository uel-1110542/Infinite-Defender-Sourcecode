using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TL_AudioManager : MonoBehaviour {

    //Mixers
    public AudioMixer MusicMixer;
    public AudioMixer SoundEffectMixer;

    //Sliders
    public Slider MusicSlider;
    public Slider SFXSlider;


    //Adjust the volume of the background music
    public void SetMusicVolume(float SliderValue)
    {
        MusicMixer.SetFloat("MusicVolume", Mathf.Log10(SliderValue) * 20f);
    }

    //Adjust the volume of the sound effects
    public void SetSoundEffectVolume(float SliderValue)
    {
        SoundEffectMixer.SetFloat("SoundEffectVolume", Mathf.Log10(SliderValue) * 20f);
    }

    void Awake()
    {
        //This gameobject does not get destroyed when it loads into a new scene
        DontDestroyOnLoad(transform.root.gameObject);
    }

    public static void PauseMusic(AudioSource Source)
    {
        //If the audio source is not null
        if (Source != null)
        {
            //Pause the audio
            Source.Pause();
        }
    }

    public static void UnpauseMusic(AudioSource Source)
    {
        //If the audio source is not null
        if (Source != null)
        {
            //Play the audio
            Source.Play();
        }
    }

    public static void PlayMusic(AudioSource Source, AudioClip Clip)
    {
        //If the audio source or the audio clip is not null
        if (Source != null || Clip != null)
        {            
            //Set the clip from the parameter
            Source.clip = Clip;

            //Play the audio
            Source.Play();
        }
    }

    public static void PlaySound(AudioSource Source, AudioClip Clip)
    {
        //If the audio source or the audio clip is not null
        if (Source != null || Clip != null)
        {
            if (!Source.isPlaying)
            {
                //Set the clip from the parameter
                Source.clip = Clip;

                //Play the audio
                Source.Play();
            }            
        }
    }

}
