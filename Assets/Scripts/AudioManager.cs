using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sound;
    
        private void Awake()
    {
        if (instance!= null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


    void Start()
    {
        foreach(Sound s in sound)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.name = s.name;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }


        PlaySound("Theme");

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string name)
    {
        foreach (Sound s in sound)
        {
            if (name == s.name)
            {
                s.source.Play();
            }
                
        }
    }







}
