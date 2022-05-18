using UnityEngine;

//Base class to all states
namespace Player_movement_camera_scripts
{
    public abstract class State : ScriptableObject
    {

        public object owner;
        public StateMachine stateMachine;
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
