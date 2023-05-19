using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Handles controlbutton added events. Adds them to the overlay and/or the savedstate
    /// </summary>
    public class ControlButtonSpawnManager
    {
        private SavedGameState savedGameState;

        public float controlButtonWidth = 0.05f;
        public float spacing = 0.01f;
        public Vector3 startPos;

        public ControlButtonSpawnManager(SavedGameState _savedGameState)
        {
            savedGameState = _savedGameState;

            // centered in front of face (approximately)
            startPos = new Vector3(0, 0.8f, 0.4f);

            // offset 3 widths to the left (+ spacing)
            startPos -= new Vector3(3 * controlButtonWidth, 0, 0);
            startPos -= new Vector3(3 * spacing, 0, 0);
        }

        public Vector3 GetSpawnLocation()
        {
            // Starting point
            float _tryZ = startPos.z;
            float _tryY = startPos.y;
            float _tryX = startPos.x;
            Vector3 tryPosition = new Vector3(_tryX, _tryY, _tryZ);


            for (int z = 0; z < 5; z++)
            {             
                // This should give 5 "rows" and 5 "columns" to try to place a new button
                for (int y = 0; y < 5; y++)
                {
                   
                    for (int x = 0; x < 5; x++)
                    {
                        
                        tryPosition = new Vector3(_tryX, _tryY, _tryZ);
                        if (IsPositionAvailable(tryPosition))
                        {
                            return tryPosition;
                        }

                        // Set a new X value
                        _tryX += controlButtonWidth + spacing;

                    }
                    _tryX = startPos.x;

                    // Set a new Y value
                    _tryY += controlButtonWidth + spacing;

                }
                _tryY = startPos.y;
                // Keep moving backwards until a space is found
                _tryZ -= (2* controlButtonWidth) + spacing;
            }

            Debug.LogError($"Could not find a place to spawn controlButton: Spawn may include overlapping items");
            return new Vector3(0, .7f, 0.3f) + startPos;

        }

        /// <summary>
        /// Checks to see if any existing controlButton is too close to the test position
        /// </summary>
        /// <param name="tryPos"></param>
        /// <returns>True if a space is available</returns>
        private bool IsPositionAvailable(Vector3 tryPos)
        {
            var positions = savedGameState.controlButtons.Select(button => button.overlayTransform.pos).ToList<Vector3>();
            return !positions.Any(v => Vector3.Distance(v, tryPos) <= controlButtonWidth);
        }
    }
}
