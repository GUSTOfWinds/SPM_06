using System;
using Event;
using ItemNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class SceneSwitch : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private bool bossIsDead;
    private Guid portalEventGuid;

    private void Start()
    {
        EventSystem.Current.RegisterListener<UnitDeathEventInfo>(SetBossLifeStatus, ref portalEventGuid);
    }

    void OnTriggerEnter(Collider other)
    {
        if (bossIsDead)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                DontDestroyOnLoad(player);
            }
            
            
          // NetworkManager.singleton.OnServerSceneChanged
            
            NetworkManager.singleton.ServerChangeScene("TerrainIsland2");
           
        }
    }

    public void SetBossLifeStatus(UnitDeathEventInfo unitDeathEventInfo)
    {
        if (unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>() != null)
        {
            if (unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetName() == "Boss")
            {
                bossIsDead = true;
            }
        }
    }
}