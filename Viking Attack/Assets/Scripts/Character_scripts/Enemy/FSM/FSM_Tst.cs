using UnityEngine;

namespace Character_scripts.Enemy.FSM
{
    public class FSM_Tst : MonoBehaviour
    {
        private Enemy_FSM Enemy_FSM;
        private Animator animator;

        private void Awake()
        {
            Enemy_FSM = new Enemy_FSM();
            Enemy_FSM.AddState(StateType.GUARD, new State_Guard(animator,this.gameObject));
            Enemy_FSM.SetState(StateType.GUARD);
        }

        private void FixedUpdate()
        {
            Enemy_FSM.OnTick();
        }

    }
}
