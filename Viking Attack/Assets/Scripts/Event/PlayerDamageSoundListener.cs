using System;
using Event;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class PlayerDamageSoundListener : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private AudioClip[] sounds; // Contains all sounds that can be played

    [SerializeField]
    private AudioClip lastAudioClip; // last audioclip that was player, used to not play same audio twice

    [SerializeField] private AudioSource audioSource;

    [SerializeField]
    private uint netID; // the netID of the player, making sure to only play sounds when the local player is hit

    private Guid SoundEventGuid;

    private void Start()
    {
        lastAudioClip = sounds[0];
        netID = gameObject.GetComponent<NetworkIdentity>().netId; // sets the netid 
        EventSystem.Current.RegisterListener<DamageEventInfo>(OnPlayerDamage,
            ref SoundEventGuid); // registers the listener
    }

    // Will play a random track from the array above when the local player takes damage
    void OnPlayerDamage(DamageEventInfo eventInfo)
    {
        if (isServer)
        {
            if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID && !audioSource.isPlaying)
            {
                do
                {
                    audioSource.clip = sounds[Random.Range(0, sounds.Length)];
                } while (audioSource.clip == lastAudioClip || audioSource.clip == null);

                if (audioSource.gameObject.active && audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }


                lastAudioClip = audioSource.clip;
                RpcPlayTakingDamage(eventInfo);
            }
        }
    }

    [ClientRpc]
    void RpcPlayTakingDamage(DamageEventInfo damageEventInfo)
    {
        if (isServer) return;
        if (damageEventInfo.target.GetComponent<NetworkIdentity>().netId == netID)
        {
            do
            {
                audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            } while (audioSource.clip == lastAudioClip || audioSource.clip == null);

            if (audioSource.gameObject.active && audioSource.isPlaying == false)
            {
                audioSource.Play();
            }
            lastAudioClip = audioSource.clip;
        }
    }
}