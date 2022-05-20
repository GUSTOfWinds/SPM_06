using TMPro;
using UnityEngine;


    public class FriendlyNameDisplay : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField] private Transform target;
        [SerializeField] public TMP_Text text;
        [SerializeField] private GameObject nameSource;
        [SerializeField] private uint netIDOfSpottedPlayer;
        [SerializeField] private Camera mainCamera;
        private Vector3 wantedPos;

        public void Update()
        {
            Display();
        }

        private void Display()
        {
            if (mainCamera == null)
                return;

            wantedPos = mainCamera.WorldToScreenPoint(target.position);
            gameObject.transform.position = wantedPos;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public uint GetPersonalNetID()
        {
            return netIDOfSpottedPlayer;
        }

        public void Setup(Transform parent, uint spottedPlayerNetID, GameObject spottedPlayer, Camera mainCam)
        {
            transform.SetParent(parent.Find("UI"));
            nameSource = spottedPlayer;
            target = nameSource.transform.Find("Overhead");
            netIDOfSpottedPlayer = spottedPlayerNetID;
            text.text = nameSource.GetComponent<GlobalPlayerInfo>().GetName();
            mainCamera = mainCam;
        }
    }
