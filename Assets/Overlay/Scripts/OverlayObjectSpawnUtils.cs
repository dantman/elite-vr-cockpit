using System;
using EVRC.Core.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace EVRC.Core.Overlay
{
    public partial class OverlayUtils
    {
        /// <summary>
        /// Finds the next available position for a GameObject, based on the spawnZoneStart defined in the ControlButtonManager.
        /// Will make 10 rows of 5 buttons and then will start over pushing further away in the Z direction until it finds a space where no other
        /// colliders are overlapping.
        /// </summary>
        /// <param name="startPos">the start position (bottom-left) </param>
        /// <param name="obj">the GameObject (prefab) to get a location for</param>
        /// <returns></returns>
        public static Vector3 GetSpawnLocation(Vector3 startPos, GameObject obj)
        {
            var sphereCollider = obj.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                throw new Exception("GameObject must have a SphereCollider component");
            }


            float spacing = 0.4f * sphereCollider.radius;
            
            // Starting point
            float _z = startPos.z;
            float _tryY= startPos.y;
            float _tryX = startPos.x;
            Vector3 tryPosition = new Vector3(_tryX, _tryY, _z);

            
            for (int z = 0; z < 25; z++)
            {
                // Keep moving backwards until a space is found
                _z -= (2 * sphereCollider.radius) + spacing;

                // This should give 10 "rows" and 5 "columns" to try to place a new button
                for (int y = 0; y <= 10; y++)
                {
                    // Set a new Y value
                    _tryY += (2 * sphereCollider.radius) + spacing;
                    for (int x = 0; x <= 5; x++)
                    {
                        // Set a new X value
                        _tryX += (2 * sphereCollider.radius) + spacing;

                        tryPosition = new Vector3(_tryX, _tryY, _z);
                        Collider[] colliders = Physics.OverlapBox(tryPosition, obj.transform.localScale / 2f);
                        if (colliders.Length == 0)
                        {
                            return tryPosition;
                        }

                    }

                    _tryX = startPos.x;

                }
                _tryY = startPos.y;
            }

            Debug.LogError($"Could not find a place to spawn controlButton: {obj.name}. Spawn may include overlapping items");
            return new Vector3(0, 0, 0.3f);


        }
    }
}