using System.IO;
using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace OverlayTests
{
    public class UpgradeManagerTests : MonoBehaviour
    {
        private OverlayStateUpgradeManager upgradeManager;
        private string version4TempStatePath;

        [SetUp]
        public void Setup()
        {
            upgradeManager = new OverlayStateUpgradeManager();

            version4TempStatePath = Path.Combine(Application.temporaryCachePath, "Version4StateFile.json");
            var json = LegacyOverlayStates.version4json;
            File.WriteAllText(version4TempStatePath, json);
        }

        /// <summary>
        /// In version 4, staticLocations did not contain a "key" value. They were stored in a dict. From
        /// version 5 forward, they were switched to an array of values, each containing a value called "key"
        /// </summary>
        [Test]
        public void Version4_Upgrades_Complete()
        {
            
            //Arrange
                //Use the legacy version of the State struct to read the original state
            var json = LegacyOverlayStates.version4json;
            LegacyOverlayStates.Version4Struct originalState =
                JsonConvert.DeserializeObject<LegacyOverlayStates.Version4Struct>(json);

            
            //Act
            OverlayState returnedState = upgradeManager.UpgradeOverlayStateFile(version4TempStatePath, 4);


            //Assert
            Assert.NotNull(returnedState);
            Assert.AreEqual(OverlayManager.currentFileVersion, returnedState.version);

            //Not Null
            Assert.NotNull(returnedState.controlButtons);
            Assert.NotNull(returnedState.staticLocations);
            Assert.NotNull(returnedState.booleanSettings);

            //Same Size
            Assert.AreEqual(originalState.controlButtons.Length, returnedState.controlButtons.Length);
            Assert.AreEqual(9, returnedState.staticLocations.Length);
            Assert.AreEqual(originalState.booleanSettings.Length, returnedState.booleanSettings.Length);

            // Moved appropriately (different location, not rotated)
            Assert.AreEqual(originalState.controlButtons[0].loc.rot, returnedState.controlButtons[0].overlayTransform.rot);
            Assert.AreNotEqual(originalState.controlButtons[0].loc.pos, returnedState.controlButtons[0].overlayTransform.pos);

        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(version4TempStatePath);
        }

    }
}
