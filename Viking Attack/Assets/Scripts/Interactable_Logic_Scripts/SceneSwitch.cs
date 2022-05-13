using System;
using Event;
using ItemNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitch : MonoBehaviour
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
            SceneManager.LoadScene(1);
        }
    }

    public void SetBossLifeStatus(UnitDeathEventInfo unitDeathEventInfo)
    {
        if (unitDeathEventInfo.EventUnitGo.GetComponent<EnemyInfo>().GetName() == "Boss")
        {
            bossIsDead = true;
        }
    }
}