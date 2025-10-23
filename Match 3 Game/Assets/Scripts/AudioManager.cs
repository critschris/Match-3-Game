using UnityEngine;
using System;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name)
    {
        Sound s =Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            Debug.Log("Not Found");
            return;
        }
        if (!s.source.isPlaying && s.name=="BGM")
        {
            s.source.Play();
        }
        
        
    }

}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public bool loop;

    [Range(0,1f)]
    public float volume;

    [Range(0,3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
