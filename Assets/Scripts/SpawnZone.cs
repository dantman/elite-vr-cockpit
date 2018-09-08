using UnityEngine;

namespace EVRC
{
    public class SpawnZone : MonoBehaviour
    {
        public Vector2 size;
        public Vector2 cellWidth;
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

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;

            DrawRectGizmo(new Rect(-size / 2f, size));
        }

        private void DrawRectGizmo(Rect rect)
        {
            var ul = new Vector2(rect.xMin, rect.yMax);
            var ur = new Vector2(rect.xMax, rect.yMax);
            var ll = new Vector2(rect.xMin, rect.yMin);
            var lr = new Vector2(rect.xMax, rect.yMin);
            Gizmos.DrawLine(ul, ur);
            Gizmos.DrawLine(ll, lr);
            Gizmos.DrawLine(ul, ll);
            Gizmos.DrawLine(ur, lr);
        }
    }
}
