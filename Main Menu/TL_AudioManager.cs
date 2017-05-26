using UnityEngine;
using UnityEngine.UI;

public class TL_AudioManager : MonoBehaviour {

    //Static variables for controlling music and sound
    public static float MusicVolume;
    public static float SFXVolume;

    //Sliders
    public Slider MusicSlider;
    public Slider SFXSlider;
    
    

    void Awake()
    {
        //This gameobject does not get destroyed when it loads into a new scene
        DontDestroyOnLoad(transform.root.gameObject);

        //Set the default value for the music and sound volume
        MusicVolume = 0.5f;
        SFXVolume = 0.5f;
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

    public void ControlMusicVolume()
    {
        //Set the volume of the music based on the value of the slider
        MusicVolume = MusicSlider.value;
    }

    public void ControlSFXVolume()
    {
        //Set the volume of the SFX based on the value of the slider
        SFXVolume = SFXSlider.value;
    }

    public static void PlayMusic(AudioSource Source, AudioClip Clip)
    {
        //If the audio source or the audio clip is not null
        if (Source != null || Clip != null)
        {            
            //Set the clip from the parameter and set the volume as the music volume variable
            Source.clip = Clip;
            Source.volume = MusicVolume;

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
                //Set the clip from the parameter and set the volume as the sfx volume variable
                Source.clip = Clip;
                Source.volume = SFXVolume;

                //Play the audio
                Source.Play();
            }            
        }
    }

}
