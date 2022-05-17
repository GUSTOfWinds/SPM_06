using System.Collections.Generic;


public enum StateType
{
    GUARD,
    CHASE,
    ATTACK,
    RECALL
}
public class Enemy_FSM //State Countrroller
{
    private StateBase currentState;
    public StateType stateType;
    private Dictionary<StateType, StateBase> allState;
    public Enemy_FSM()
    {
        allState = new Dictionary<StateType, StateBase>(); //contructor 
    }

    public void AddState(StateType stateType, StateBase nextState)
    {
        //each state only add once if the state is already in dictionary, dont need to add again
        if (allState.ContainsKey(stateType))
        {
            
            return;
        }
        allState.Add(stateType, nextState);
    }

    public void SetState(StateType stateType)
    {
        //pass in the new state, which state should AI be next
        if (currentState == allState[stateType]) return; //if we are in the same state, dont change 
        currentState?.OnExit(); //if the current State is not null, leave the current state can go next
        currentState = allState[stateType];
        currentState?.OnEnter(); //excute the new state's on enter
    }
    public void OnTick() //On uppdate
    {
        currentState?.OnUppdate(); //if the current state is not null, excute it's Onuppdate function
    }


}
