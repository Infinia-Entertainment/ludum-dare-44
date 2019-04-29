using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Stores all sounds that will be used in game
    public Sound[] sounds;


    private void Awake()
    {

        //For each sound in sound array
        foreach(Sound s in sounds)
        {
            //Create an AudioSource component for the sound
            s.source = gameObject.AddComponent<AudioSource>();

            //And assign stored values to the AudioSource
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //Play the sound
    public void Play(string name)
    {
        //Find sound with given name in the array of sounds
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //Play the sound through its source
        s.source.Play();

        Debug.Log(name + " is playing");
    }
    public Sound GetSound(string name)
    {
        //Find sound with given name in the array of sounds
        Sound s = Array.Find(sounds, sound => sound.name == name);

        return s;
    }
}
