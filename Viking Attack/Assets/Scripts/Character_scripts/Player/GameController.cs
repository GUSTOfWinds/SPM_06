using UnityEngine;

namespace ItemNamespace
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
