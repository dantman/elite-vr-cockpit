using System.IO;
using System.Linq;
using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace OverlayTests
{
    public class UpgradeManagerTests
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
        
        [Test]
        public void Version4_Upgrades_Complete()
        {

            #region ---------------Arrange------------------
            //Use the legacy version of the State struct to read the original state
            var json = LegacyOverlayStates.version4json;
            LegacyOverlayStates.Version4Struct originalState =
                JsonConvert.DeserializeObject<LegacyOverlayStates.Version4Struct>(json);

            #endregion

            #region --------------Act-----------------------
            // Upgrade the file
            SavedStateFile returnedState = upgradeManager.UpgradeOverlayStateFile(version4TempStatePath, 4);

            #endregion

            #region --------------Assert-----------------------
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
            #endregion
        }

        [Test]
        public void ControlButtons_Raised()
        {

            #region ---------------Arrange------------------
            //Use the legacy version of the State struct to read the original state
            var json = LegacyOverlayStates.version4json;
            LegacyOverlayStates.Version4Struct originalState =
                JsonConvert.DeserializeObject<LegacyOverlayStates.Version4Struct>(json);

            #endregion

            #region --------------Act-----------------------
            // Upgrade the file
            SavedStateFile returnedState = upgradeManager.UpgradeOverlayStateFile(version4TempStatePath, 4);

            #endregion

            #region --------------Assert-----------------------
            // Moved appropriately (different location, not rotated)
            Assert.AreEqual(originalState.controlButtons[0].loc.rot, returnedState.controlButtons[0].overlayTransform.rot);
            Assert.AreNotEqual(originalState.controlButtons[0].loc.pos, returnedState.controlButtons[0].overlayTransform.pos);
            #endregion
        }

        [Test]
        public void StaticLocation_Keys_Added()
        {

            #region ---------------Arrange------------------
            //Use the legacy version of the State struct to read the original state
            var json = LegacyOverlayStates.version4json;
            LegacyOverlayStates.Version4Struct originalState =
                JsonConvert.DeserializeObject<LegacyOverlayStates.Version4Struct>(json);

            #endregion

            #region --------------Act-----------------------
            // Upgrade the file
            SavedStateFile returnedState = upgradeManager.UpgradeOverlayStateFile(version4TempStatePath, 4);

            #endregion

            #region --------------Assert-----------------------
            // The StaticLocations value is an array, instead of 9 independently defined SavedTransforms
            Assert.IsNotNull(returnedState.staticLocations[0]);
            #endregion
        }

        [Test]
        public void Anchor_Info_Generated()
        {
            //Use the legacy version of the State struct to read the original state
            var json = LegacyOverlayStates.version4json;
            LegacyOverlayStates.Version4Struct originalState =
                JsonConvert.DeserializeObject<LegacyOverlayStates.Version4Struct>(json);


            #region --------------Act-----------------------
            // Upgrade the file
            SavedStateFile returnedState = upgradeManager.UpgradeOverlayStateFile(version4TempStatePath, 4);

            // Save a reference to a couple of known buttons from the sample file
            SavedControlButton buggy_scoop = returnedState.controlButtons.FirstOrDefault(x => x.type == "ToggleCargoScoop_Buggy");
            SavedControlButton fss_quit = returnedState.controlButtons.FirstOrDefault(x => x.type == "ExplorationFSSQuit");
            #endregion


            #region --------------Assert-----------------------
            // ControlButton objects have non-null values for anchorStatusFlag and anchorGuiFocus
            Assert.IsNotNull(returnedState.controlButtons[0].anchorStatusFlag);
            Assert.IsNotNull(returnedState.controlButtons[0].anchorGuiFocus);
            Assert.AreEqual("InSRV", buggy_scoop.anchorStatusFlag); // should end up in the SRV Anchor
            Assert.AreEqual("FSSMode", fss_quit.anchorGuiFocus); // should end up in the FSS anchor
            #endregion
        }      

        [TearDown]
        public void TearDown()
        {
            File.Delete(version4TempStatePath);
        }

    }
}
