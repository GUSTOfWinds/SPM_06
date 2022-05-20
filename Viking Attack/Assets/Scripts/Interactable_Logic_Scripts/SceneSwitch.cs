using System;
using Character_scripts.Enemy;
using Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class SceneSwitch : NetworkBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] public bool bossIsDead;
    private Guid portalEventGuid;


    private void Start()
    {
        
        EventSystem.Current.RegisterListener<UnitDeathEventInfo>(SetBossLifeStatus, ref portalEventGuid);
    }

    void OnTriggerEnter(Collider other)
    {
        if (bossIsDead)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                DontDestroyOnLoad(player);
            }

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