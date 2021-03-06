using UnityEngine;


    public class ToggleCharacterScreen : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField] private GameObject characterScreen;

        public Animator animator;
        public Animator otherAnimator;
        public Animator thirdAnimator;
        public bool locked;
    

        public void ToggleScreen()
        {
            if (!locked)
            {
                if (animator.GetBool("CSOpen"))
                {
                    animator.SetBool("CSOpen", false);
                    if (gameObject.GetComponent<CameraMovement3D>().shouldBeLocked == false)
                    {
                        gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = true;
                    }

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    animator.SetBool("CSOpen", true);
                    if (gameObject.GetComponent<GlobalPlayerInfo>().GetStatPoints() == 0)
                    {
                        otherAnimator.SetBool("levelNOTIF", false);
                        thirdAnimator.SetBool("pointsavailable", false);
                    }
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    if (gameObject.GetComponent<CameraMovement3D>().shouldBeLocked == true)
                    {
                        gameObject.GetComponent<CameraMovement3D>().shouldBeLocked = false;
                    }

                    characterScreen.GetComponent<CharacterScreen>().OpenCharacterScreen();
                }
            }
        }
    }
