using UnityEngine;


    public class ToggleControlsScreen : MonoBehaviour
    {
        [SerializeField] public Animator animator;

        public void ToggleControls()
        {
            animator.SetBool("controls", !animator.GetBool("controls"));
        }
    }
