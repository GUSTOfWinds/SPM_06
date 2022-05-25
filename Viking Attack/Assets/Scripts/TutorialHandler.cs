using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    public bool wasdFinished;
    public bool sprintFinished;
    public bool dashFinished;
    public bool attackFinished;
    public bool itemSwapFinished;
    public void FinishWasd()
    {
        wasdFinished = true;
        // TODO ADD ANIMATION
    }
    
    public void FinishSprint()
    {
        if (!wasdFinished)
        {
            return;
        }

        sprintFinished = true;
        // TODO ADD ANIMATION
    }

    public void FinishDash()
    {
        if (!sprintFinished)
        {
            return;
        }

        dashFinished = true;
        // TODO ADD ANIMATION
    }

    public void FinishAttack()
    {
        if (!dashFinished)
        {
            return;
        }

        attackFinished = true;
        // TODO ADD ANIMATION
    }

    public void FinishItemSwap()
    {
        if (!attackFinished)
        {
            return;
        }

        itemSwapFinished = true;
        // TODO ADD ANIMATION
    }
}