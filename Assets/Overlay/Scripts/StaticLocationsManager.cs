using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    [Serializable]
    public struct staticLocationKeyTargetMap
    {
        public string key;
        public GameObject target;
    }

    public class StaticLocationsManager : MonoBehaviour
    {
        [Header("GameObjects with saved states")]
        public List<staticLocationKeyTargetMap> registeredObjects;

        private bool ready = false;

        void OnEnable()
        {
            StartCoroutine(GetAnchors());
        }

        
        /// <summary>
        /// Use the StaticLocationAnchors from the scene to map parent gameObjects. SavedGameObjects will be identified based on the details in the Anchor.
        /// </summary>
        /// <remarks>This can be kinda slow, so other methods are waiting for this to complete</remarks>
        /// <returns></returns>
        private IEnumerator GetAnchors()
        {
            registeredObjects.Clear();

            var anchors = FindObjectsOfType<StaticLocationAnchor>(true).ToList();
            foreach (var anchor in anchors)
            {
                registeredObjects.Add(new staticLocationKeyTargetMap()
                {
                    key = anchor.key,
                    target = anchor.movable.targetTransform.gameObject,
                });
            }
            if (registeredObjects.Count != registeredObjects.Select(a => a.key).Distinct().Count())
            {
                Debug.LogError($"Duplicate keys found in StaticLocationAnchor list! {gameObject.name}");
            }

            yield return new WaitForSeconds(1.0f);
            ready = true;
        }

        public IEnumerator PlaceWhenReady(SavedGameObject[] loadedGameObjects)
        {
            while (!ready)
            {
                yield return new WaitForSeconds(1f);
            }
            PlaceAll(loadedGameObjects);

        }
        

        /// <summary>
        /// Places StaticLocation objects in the scene from the passed state.
        /// </summary>
        /// <param name="state"></param>
        public void PlaceAll(SavedGameObject[] loadedGameObjects)
        {
            if (registeredObjects == null)
            {
                Debug.LogWarning($"Registered objects are not available. Cannot place objects from loaded State. {gameObject.name}");
            }

            if (loadedGameObjects == null) return;

            for (var i = 0; i < loadedGameObjects.Length; i++)
            {
                string _key = loadedGameObjects[i].key;

                //Try to find a matching registered object
                int findIndex = registeredObjects.FindIndex(ro => ro.key == _key);
                if (findIndex == -1) continue;
                
                // Assign position and rotation from the loaded state 
                Vector3 _pos = loadedGameObjects[i].overlayTransform.pos;
                Vector3 _rot = loadedGameObjects[i].overlayTransform.rot;
                registeredObjects[findIndex].target.transform.localPosition = _pos;
                registeredObjects[findIndex].target.transform.localEulerAngles= _rot;
            }
        }

        /// <summary>
        /// Converts the registeredObjects into a saveable format.
        /// </summary>
        /// <returns></returns>
        public SavedGameObject[] GetCurrentStates()
        {
            var serializedResult = new List<SavedGameObject>();
            foreach (staticLocationKeyTargetMap obj in registeredObjects)
            {
                serializedResult.Add(new SavedGameObject()
                {
                    key = obj.key,
                    overlayTransform = new OverlayTransform()
                    {
                        pos = obj.target.transform.localPosition,
                        rot = obj.target.transform.localEulerAngles,
                    }

                });
            }
            return serializedResult.ToArray();
        }
    }
}