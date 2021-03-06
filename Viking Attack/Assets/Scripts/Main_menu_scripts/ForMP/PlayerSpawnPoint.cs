using UnityEngine;
/**
 * @author Victor Wikner
 */

    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake() => PlayerSpawnSystem.AddSpawnPoint(transform);

        private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPoint(transform);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            var position = transform.position;
            Gizmos.DrawSphere(position, 0.75f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + transform.forward * 2);
        }
    }

