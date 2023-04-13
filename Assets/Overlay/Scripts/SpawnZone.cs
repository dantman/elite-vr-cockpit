using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class SpawnZone : MonoBehaviour
    {
        public SpawnZoneCell[] spawnCells;

        /**
         * Get the first free Spawn Cell
         */
        public SpawnZoneCell GetFreeCell()
        {
            foreach (var cell in spawnCells)
            {
                if (cell.Free)
                {
                    return cell;
                }
            }

            Debug.LogWarning("All spawn cells are ocupied");
            return null;
        }

        /**
         * Position a recently created game object in the spawn zone
         */
        public bool Spawn(GameObject go)
        {
            var cell = GetFreeCell();
            if (cell == null) return false;

            go.transform.SetPositionAndRotation(cell.transform.position, cell.transform.rotation);
            return true;
        }
    }
}
