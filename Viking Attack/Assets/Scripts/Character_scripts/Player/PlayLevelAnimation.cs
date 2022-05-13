using System;
using Event;
using UnityEngine;

namespace Character_scripts.Player
{
    /**
     * @author Martin Kings
     */
    public class PlayLevelAnimation : MonoBehaviour
    {
        [SerializeField] private Animator parentAnimator;
        [SerializeField] private Animator otherAnimator;
        private Guid levelUpGuid;

        void Start()
        {
            EventSystem.Current.RegisterListener<PlayerLevelUpEventInfo>(OnPlayerLevelUp, ref levelUpGuid);
        }

        public void OnPlayerLevelUp(PlayerLevelUpEventInfo playerLevelUpEventInfo)
        {
            parentAnimator.SetTrigger("incLVL");
            otherAnimator.SetBool("levelNOTIF", true);
        }
    }
}