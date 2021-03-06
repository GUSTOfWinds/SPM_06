using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Event
{
    public class PlayerEatingSoundListener : NetworkBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField] private AudioClip[] sounds; // Contains all sounds that can be played

        [SerializeField]
        private AudioClip lastAudioClip; // last audioclip that was player, used to not play same audio twice

        [SerializeField] private AudioSource audioSource;

        [SerializeField]
        private uint netID;

        private Guid SoundEventGuid;

        private void Start()
        {
            lastAudioClip = sounds[0];
            netID = gameObject.GetComponent<NetworkIdentity>().netId; // sets the netid 
            EventSystem.Current.RegisterListener<PlayerEatingEventInfo>(OnPlayerEating,
                ref SoundEventGuid); // registers the listener
        }

        // Will play a random track from the array above when the local player takes damage
        void OnPlayerEating(PlayerEatingEventInfo eventInfo)
        {
            if (isLocalPlayer)
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
}