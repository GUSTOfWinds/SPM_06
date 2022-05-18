using UnityEngine;

namespace Character_scripts.Player
{
    public class GameController
    {
        void PauseGame ()
        {
            Time.timeScale = 0;
        }
        void ResumeGame ()
        {
            Time.timeScale = 1;
        }
    }
}