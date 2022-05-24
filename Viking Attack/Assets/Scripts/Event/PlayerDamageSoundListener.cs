using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Event
{
    public class PlayerDamageSoundListener : NetworkBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField] private AudioClip[] damageSounds; // Contains all sounds that can be played

        [SerializeField] private AudioClip deathSound;

        [SerializeField]
        private AudioClip lastAudioClip; // last audioclip that was player, used to not play same audio twice

        [SerializeField] private AudioSource audioSource;

        [SerializeField]
        private uint netID; // the netID of the player, making sure to only play sounds when the local player is hit

        private Guid SoundEventGuid;

        private void Start()
        {
            lastAudioClip = damageSounds[0];
            netID = gameObject.GetComponent<NetworkIdentity>().netId; // sets the netid 
            EventSystem.Current.RegisterListener<PlayerDamageEventInfo>(OnPlayerDamage,
                ref SoundEventGuid); // registers the listener
        }

        // Will play a random track from the array above when the local player takes damage
        void OnPlayerDamage(PlayerDamageEventInfo eventInfo)
        {
            if (isServer)
            {
                if (eventInfo.target.GetComponent<NetworkIdentity>().netId == netID && !audioSource.isPlaying)
                {
                    do
                    {
                        if (eventInfo.target.GetComponent<GlobalPlayerInfo>().GetHealth() <= 0)
                        {
                            audioSource.clip = deathSound;
                            audioSource.Play();
                            break;
                        }
                    
                        audioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
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
        void RpcPlayTakingDamage(PlayerDamageEventInfo playerDamageEventInfo)
        {
            if (isServer) return;
            if (playerDamageEventInfo.target.GetComponent<NetworkIdentity>().netId == netID)
            {
                do
                {
                    if (playerDamageEventInfo.target.GetComponent<GlobalPlayerInfo>().GetHealth() <= 0)
                    {
                        audioSource.clip = deathSound;
                        audioSource.Play();
                        break;
                    }

                    audioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
                } while (audioSource.clip == lastAudioClip || audioSource.clip == null);

                if (audioSource.gameObject.active && audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }

                lastAudioClip = audioSource.clip;
            }
        }
    }
}