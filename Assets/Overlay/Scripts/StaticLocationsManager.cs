using System;
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

        void OnEnable()
        {
            RefreshAnchors();
        }
        void OnValidate()
        {
            RefreshAnchors();
        }

        private void RefreshAnchors()
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

        /// <summary>
        /// Places StaticLocation objects in the scene from the passed state.
        /// </summary>
        /// <param name="state"></param>
        public void PlaceAll(OverlayState state)
        {
            if (registeredObjects == null)
            {
                Debug.LogWarning($"Registered objects are not available. Cannot place objects from loaded State. {gameObject.name}");
            }

            if (state.staticLocations == null) return;

            for (var i = 0; i < state.staticLocations.Length; i++)
            {
                string _key = state.staticLocations[i].key;

                //Try to find a matching registered object
                int findIndex = registeredObjects.FindIndex(ro => ro.key == _key);
                if (findIndex == -1) continue;
                
                // Assign position and rotation from the loaded state 
                Vector3 _pos = state.staticLocations[i].overlayTransform.pos;
                Vector3 _rot = state.staticLocations[i].overlayTransform.rot;
                registeredObjects[findIndex].target.transform.localPosition = _pos;
                registeredObjects[findIndex].target.transform.localEulerAngles= _rot;
            }
        }
    }
}