using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ItemNamespace;
using Mirror;
using UnityEngine;

namespace Event
{
    public class EnemyDeathSoundListener : NetworkBehaviour
    {
        [SerializeField] private AudioClip[] deathSounds;
        [SerializeField] private GameObject musicPlayerPrefab;
        private int indexToPlay;
        private Guid enemyDeathGuid;


        private void Start()
        {
            EventSystem.Current.RegisterListener<UnitDeathEventInfo>(OnEnemyDeath,
                ref enemyDeathGuid); // registers the listener
        }

        private void OnEnemyDeath(UnitDeathEventInfo unitDeathEventInfo)
        {
            if (isServer)
            {
                // Finds which sound to play depending on the CharacterBase
                switch (unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetCharacterBase().GetEnemyType())
                {
                    case CharacterBase.EnemyType.Bear:
                        indexToPlay = 0;
                        break;
                    case CharacterBase.EnemyType.Skeleton:
                        indexToPlay = 1;
                        break;
                    default:
                        break;
                }

                var position = unitDeathEventInfo.EventUnitGo.transform.position;
                var musicPlayer = Instantiate(musicPlayerPrefab, new Vector3(
                    position.x,
                    position.y + 1,
                    position.z), new Quaternion(0, 0, 0, 0));
                NetworkServer.Spawn(musicPlayer);
                musicPlayer.GetComponent<AudioSource>().PlayOneShot(deathSounds[indexToPlay]);
            
                // Plays sound on client
                RpcPlaySound(musicPlayer, indexToPlay);
            
                // Removes the gameobject
                StartCoroutine(RemoveAfterPlaying(musicPlayer));
            }

        }

        [ClientRpc]
        private void RpcPlaySound(GameObject musicPlayer, int index)
        {
            if (isServer)
            {
                return;
            }
            musicPlayer.GetComponent<AudioSource>().PlayOneShot(deathSounds[index]);
        }

        IEnumerator RemoveAfterPlaying(GameObject musicPlayer)
        {
            yield return new WaitForSeconds(3);
            NetworkServer.Destroy(musicPlayer);
        }
    }
}