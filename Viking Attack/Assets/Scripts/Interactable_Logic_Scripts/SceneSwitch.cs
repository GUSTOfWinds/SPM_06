using System;
using Character_scripts.Enemy;
using Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable_Logic_Scripts
{
    /**
     * @author Martin Kings
     */
    public class SceneSwitch : MonoBehaviour
    {
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
}