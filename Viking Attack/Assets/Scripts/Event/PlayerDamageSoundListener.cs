using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class PlayerDamageSoundListener : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds; // Contains all sounds that can be played
    [SerializeField] private AudioClip lastAudioClip; // last audioclip that was player, used to not play same audio twice
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private uint netID; // the netID of the player, making sure to only play sounds when the local player is hit
    private Guid SoundEventGuid;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); // fetches the audio source
        lastAudioClip = sounds[0];
        netID = gameObject.GetComponentInParent<NetworkIdentity>().netId; // sets the netid 
        EventSystem.Current.RegisterListener<DamageEventInfo>(OnPlayerDamage,
            ref SoundEventGuid); // registers the listener
        
    }

    // Will play a random track from the array above when the local player takes damage
    void OnPlayerDamage(DamageEventInfo eventInfo)
    {
        if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID && !audioSource.isPlaying)
        {
            while (audioSource.clip == lastAudioClip || audioSource.clip == null)
            {
                audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            }

            audioSource.Play();
            lastAudioClip = audioSource.clip;
        }
    }
}