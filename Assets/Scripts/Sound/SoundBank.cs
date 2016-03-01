using UnityEngine;
using UnityEngine.Assertions;

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundBank : MonoBehaviour
{

    [SerializeField]
    private string[] audioClipNames;
    [SerializeField]
    private AudioClip[] audioClips;

    public Dictionary<string, AudioClip> clipDictionary;

    private AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        clipDictionary = new Dictionary<string, AudioClip>();

        Assert.IsTrue( audioClipNames.Length == audioClips.Length );
        for(int i = 0; i < audioClipNames.Length; i++)
        {
            clipDictionary.Add(audioClipNames[i], audioClips[i]);
        }
	}

    public void PlaySound(string key)
    {
        Debug.Log("Hit");
        audioSource.PlayOneShot(clipDictionary[key]);
    }
}
