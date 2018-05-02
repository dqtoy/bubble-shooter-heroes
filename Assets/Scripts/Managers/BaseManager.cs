using UnityEngine;
using System.Collections;

// Is the parent Base manager all other Managers will use AudioClips or other data from
public class BaseManager : ScriptableObject
{

    // instance of class (Singleton)
    public static BaseManager instance = null;

    // audio clips to play looped and fade in/out 
    public AudioClip menuMusic;
    public static GameObject globalMenuMusic;
    public AudioClip gameMusic;
    public static GameObject globalGameMusic;

    // our fade animation clip
    public AnimationClip fadeClip;

    // audio clips to play one time
    public AudioClip sfxButton;
    public AudioClip sfxBomb;
    public AudioClip sfxSwap;
    public AudioClip sfxTouched;
    public AudioClip sfxDrop;
    public AudioClip sfxReadyGo;
    public AudioClip sfxThrow;
    public AudioClip sfxVictory;
    public AudioClip sfxFailed;

    // declare instance
    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public static BaseManager GetInstance()
    {
        return instance;
    }
}