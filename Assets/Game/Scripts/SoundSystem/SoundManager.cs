using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField, Foldout("References")] private AudioSource audioSource;
    [SerializeField, Foldout("References")] private AudioMixer audioMixer;
    [SerializeField, Foldout("References")] private AudioClip noteSound;
    
    private int perfectComboCount = 0;
    private float basePitch = 1f; 
    private float pitchIncrement = 0.05f;
    private int maxCombo = 15;
    
    private void Start()
    {
        audioSource.clip = noteSound;
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Notes")[0];
        audioSource.playOnAwake = false;
    }

    [Button()]
    public void AudioTest()
    {
        PlayNoteSound(true);
    }
    public void PlayNoteSound(bool isPerfectCut)
    {
        if (isPerfectCut)
        {
            perfectComboCount++;
            audioSource.pitch = Mathf.Min(basePitch + (perfectComboCount * pitchIncrement), basePitch + (maxCombo * pitchIncrement));
        }
        else
        {
            perfectComboCount = 0;
            audioSource.pitch = basePitch;
        }

        audioSource.Play();
    }
    
}
