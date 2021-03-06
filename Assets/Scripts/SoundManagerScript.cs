using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip jumpSound, dashSound, walkSound, snowSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        jumpSound = Resources.Load<AudioClip> ("jump");
        dashSound = Resources.Load<AudioClip> ("dashpulse");
        walkSound = Resources.Load<AudioClip> ("Walk");
        snowSound = Resources.Load<AudioClip> ("Snow");
        audioSrc = GetComponent<AudioSource> ();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySound (string clip) {
        switch (clip) {
            case "jump":
                audioSrc.PlayOneShot(jumpSound);
                break;
            case "dashpulse":
                audioSrc.PlayOneShot(dashSound);
                break;
            case "Walk":
                audioSrc.PlayOneShot(walkSound);
                break;
            case "Snow":
                audioSrc.PlayOneShot(snowSound);
                break;
        }
    }
}
