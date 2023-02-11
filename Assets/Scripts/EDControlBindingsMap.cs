using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using static EVRC.ControlButtonAsset;
    using CockpitMode = CockpitUIMode.CockpitMode;
    using EDControlButton = EDControlBindings.EDControlButton;

    public static class EDControlBindingsMap
    {
        public enum ControlsMode
        {
            None = 0,
            NotRunning,
            Map,
            MainShip,
            CockpitPanel,
            StationServices,
            SRV,
            Fighter,
            FSS,
            DSS,
            Menu,
            Landing
        }

        public static readonly Dictionary<ControlsMode, List<EDControlButton>> controlBindingsMap = new Dictionary<ControlsMode, List<EDControlButton>>()
        {
            { ControlsMode.None,new List<EDControlButton>() {} },
            { ControlsMode.NotRunning,new List<EDControlButton>() {} },
            { ControlsMode.Map, 
                new List<EDControlButton>() 
                    {
                        //Camera
                        EDControlButton.CamPitchUp,
                        EDControlButton.CamPitchDown,
                        EDControlButton.CamYawLeft,
                        EDControlButton.CamYawRight,
                        EDControlButton.CamTranslateForward,
                        EDControlButton.CamTranslateBackward,
                        EDControlButton.CamTranslateLeft,
                        EDControlButton.CamTranslateRight,
                        EDControlButton.CamTranslateUp,
                        EDControlButton.CamTranslateDown,
                        EDControlButton.CamZoomIn,
                        EDControlButton.CamZoomOut,
                        
                        // UI Controls
                        EDControlButton.UI_Up,
                        EDControlButton.UI_Down,
                        EDControlButton.UI_Left,
                        EDControlButton.UI_Right,
                        EDControlButton.UI_Select,
                        EDControlButton.UI_Back,
                        EDControlButton.UI_Toggle,
                        EDControlButton.CycleNextPanel,
                        EDControlButton.CyclePreviousPanel,
                        EDControlButton.CycleNextPage,
                        EDControlButton.CyclePreviousPage,
                    } 
            },
            { ControlsMode.MainShip,
                new List<EDControlButton>()
                    {
                        //Flight Controls
                        EDControlButton.YawLeftButton,
                        EDControlButton.YawRightButton,
                        EDControlButton.YawToRollButton,
                        EDControlButton.RollLeftButton,
                        EDControlButton.RollRightButton,
                        EDControlButton.PitchUpButton,
                        EDControlButton.PitchDownButton,
                        EDControlButton.LeftThrustButton,
                        EDControlButton.RightThrustButton,
                        EDControlButton.UpThrustButton,
                        EDControlButton.DownThrustButton,
                        EDControlButton.ForwardThrustButton,
                        EDControlButton.BackwardThrustButton,
                        EDControlButton.ForwardKey,
                        EDControlButton.BackwardKey,
                        EDControlButton.SetSpeedMinus100,
                        EDControlButton.SetSpeedMinus75,
                        EDControlButton.SetSpeedMinus50,
                        EDControlButton.SetSpeedMinus25,
                        EDControlButton.SetSpeedZero,
                        EDControlButton.SetSpeed25,
                        EDControlButton.SetSpeed50,
                        EDControlButton.SetSpeed75,
                        EDControlButton.SetSpeed100,
                        
                        //Cockpit Controls
                        EDControlButton.ToggleCargoScoop,
                        EDControlButton.UseAlternateFlightValuesToggle,
                        EDControlButton.ToggleReverseThrottleInput,
                        EDControlButton.ToggleFlightAssist,
                        EDControlButton.UseBoostJuice,
                        EDControlButton.HyperSuperCombination,
                        EDControlButton.Supercruise,
                        EDControlButton.Hyperspace,
                        EDControlButton.DisableRotationCorrectToggle,
                        EDControlButton.OrbitLinesToggle,
                        EDControlButton.EjectAllCargo,
                        EDControlButton.LandingGearToggle,
                        EDControlButton.MicrophoneMute,
                        EDControlButton.UseShieldCell,
                        EDControlButton.FireChaffLauncher,
                        EDControlButton.NightVisionToggle,
                        EDControlButton.DeployHardpointToggle,
                        EDControlButton.ToggleButtonUpInput,
                        EDControlButton.DeployHeatSink,
                        EDControlButton.ShipSpotLightToggle,
                        EDControlButton.RadarIncreaseRange,
                        EDControlButton.RadarDecreaseRange,
                        EDControlButton.ChargeECM,
                        EDControlButton.EngineColourToggle,
                        EDControlButton.WeaponColourToggle,

                        // Targeting
                        EDControlButton.SelectTarget,
                        EDControlButton.CycleNextTarget,
                        EDControlButton.CyclePreviousTarget,
                        EDControlButton.SelectHighestThreat,
                        EDControlButton.CycleNextHostileTarget,
                        EDControlButton.CyclePreviousHostileTarget,
                        EDControlButton.CycleNextSubsystem,
                        EDControlButton.CyclePreviousSubsystem,
                        EDControlButton.SelectTargetsTarget,

                        // Wingman & Orders
                        EDControlButton.TargetWingman0,
                        EDControlButton.TargetWingman1,
                        EDControlButton.TargetWingman2,
                        EDControlButton.OrderFocusTarget,
                        EDControlButton.OrderAggressiveBehaviour,
                        EDControlButton.OrderDefensiveBehaviour,
                        EDControlButton.OpenOrders,
                        EDControlButton.OrderRequestDock,
                        EDControlButton.OrderFollow,
                        EDControlButton.OrderHoldFire,
                        EDControlButton.OrderHoldPosition,
                        EDControlButton.OpenCodexGoToDiscovery,
                        EDControlButton.WingNavLock,
                        
                        // Combat
                        EDControlButton.TargetNextRouteSystem,
                        EDControlButton.PrimaryFire,
                        EDControlButton.SecondaryFire,
                        EDControlButton.CycleFireGroupNext,
                        EDControlButton.CycleFireGroupPrevious,
                        
                        // Pip management
                        EDControlButton.IncreaseEnginesPower,
                        EDControlButton.IncreaseWeaponsPower,
                        EDControlButton.IncreaseSystemsPower,
                        EDControlButton.ResetPowerDistribution,
                        
                        // Focus & Panels
                        EDControlButton.UIFocus,
                        EDControlButton.FocusLeftPanel,
                        EDControlButton.FocusCommsPanel,
                        EDControlButton.QuickCommsPanel,
                        EDControlButton.FocusRadarPanel,
                        EDControlButton.FocusRightPanel,
                        EDControlButton.GalaxyMapOpen,
                        EDControlButton.SystemMapOpen,
                        EDControlButton.ExplorationFSSEnter,
                        EDControlButton.FriendsMenu,
                        EDControlButton.Pause,
                    }
            },
            { ControlsMode.CockpitPanel,
                new List<EDControlButton>()
                    {
                        EDControlButton.UI_Up,
                        EDControlButton.UI_Down,
                        EDControlButton.UI_Left,
                        EDControlButton.UI_Right,
                        EDControlButton.UI_Select,
                        EDControlButton.UI_Back,
                        EDControlButton.UI_Toggle,
                        EDControlButton.CycleNextPanel,
                        EDControlButton.CyclePreviousPanel,
                        EDControlButton.CycleNextPage,
                        EDControlButton.CyclePreviousPage,
                    }
            },
            { ControlsMode.StationServices,
                new List<EDControlButton>()
                    {
                        EDControlButton.UI_Up,
                        EDControlButton.UI_Down,
                        EDControlButton.UI_Left,
                        EDControlButton.UI_Right,
                        EDControlButton.UI_Select,
                        EDControlButton.UI_Back,
                        EDControlButton.UI_Toggle,
                        EDControlButton.CycleNextPanel,
                        EDControlButton.CyclePreviousPanel,
                        EDControlButton.CycleNextPage,
                        EDControlButton.CyclePreviousPage,
                    }
            },
            { ControlsMode.SRV,
                new List<EDControlButton>()
                    {
                        EDControlButton.ToggleDriveAssist,
                        EDControlButton.SteerLeftButton,
                        EDControlButton.SteerRightButton,
                        EDControlButton.IncreaseSpeedButtonMax,
                        EDControlButton.DecreaseSpeedButtonMax,
                        EDControlButton.RecallDismissShip,
                        EDControlButton.VerticalThrustersButton,
                        EDControlButton.PhotoCameraToggle_Buggy,
                        EDControlButton.BuggyRollLeftButton,
                        EDControlButton.BuggyRollRightButton,
                        EDControlButton.BuggyPitchUpButton,
                        EDControlButton.BuggyPitchDownButton,
                        EDControlButton.BuggyPrimaryFireButton,
                        EDControlButton.BuggySecondaryFireButton,
                        EDControlButton.AutoBreakBuggyButton,
                        EDControlButton.HeadlightsBuggyButton,
                        EDControlButton.ToggleBuggyTurretButton,
                        EDControlButton.BuggyTurretYawLeftButton,
                        EDControlButton.BuggyTurretYawRightButton,
                        EDControlButton.BuggyTurretPitchUpButton,
                        EDControlButton.BuggyTurretPitchDownButton,
                        EDControlButton.BuggyToggleReverseThrottleInput,
                        EDControlButton.IncreaseEnginesPower_Buggy,
                        EDControlButton.IncreaseWeaponsPower_Buggy,
                        EDControlButton.IncreaseSystemsPower_Buggy,
                        EDControlButton.ResetPowerDistribution_Buggy,
                        EDControlButton.ToggleCargoScoop_Buggy,
                        EDControlButton.EjectAllCargo_Buggy,
                        EDControlButton.UIFocus_Buggy,
                        EDControlButton.FocusLeftPanel_Buggy,
                        EDControlButton.FocusCommsPanel_Buggy,
                        EDControlButton.QuickCommsPanel_Buggy,
                        EDControlButton.FocusRadarPanel_Buggy,
                        EDControlButton.FocusRightPanel_Buggy,
                        EDControlButton.GalaxyMapOpen_Buggy,
                        EDControlButton.SystemMapOpen_Buggy,
                        EDControlButton.HeadLookToggle_Buggy,
                        EDControlButton.SelectTarget_Buggy,
                        EDControlButton.SetSpeedMinus100,
                        EDControlButton.SetSpeedMinus75,
                        EDControlButton.SetSpeedMinus50,
                        EDControlButton.SetSpeedMinus25,
                        EDControlButton.SetSpeedZero,
                        EDControlButton.SetSpeed25,
                        EDControlButton.SetSpeed50,
                        EDControlButton.SetSpeed75,
                        EDControlButton.SetSpeed100,
                    }
            },
            { ControlsMode.Fighter,
                new List<EDControlButton>()
                    {
                        EDControlButton.YawLeftButton,
                        EDControlButton.YawRightButton,
                        EDControlButton.YawToRollButton,
                        EDControlButton.RollLeftButton,
                        EDControlButton.RollRightButton,
                        EDControlButton.PitchUpButton,
                        EDControlButton.PitchDownButton,
                        EDControlButton.LeftThrustButton,
                        EDControlButton.RightThrustButton,
                        EDControlButton.UpThrustButton,
                        EDControlButton.DownThrustButton,
                        EDControlButton.ForwardThrustButton,
                        EDControlButton.BackwardThrustButton,
                        EDControlButton.ForwardKey,
                        EDControlButton.BackwardKey,
                        EDControlButton.SetSpeedMinus100,
                        EDControlButton.SetSpeedMinus75,
                        EDControlButton.SetSpeedMinus50,
                        EDControlButton.SetSpeedMinus25,
                        EDControlButton.SetSpeedZero,
                        EDControlButton.SetSpeed25,
                        EDControlButton.SetSpeed50,
                        EDControlButton.SetSpeed75,
                        EDControlButton.SetSpeed100,
                        // Targeting
                        EDControlButton.SelectTarget,
                        EDControlButton.CycleNextTarget,
                        EDControlButton.CyclePreviousTarget,
                        EDControlButton.SelectHighestThreat,
                        EDControlButton.CycleNextHostileTarget,
                        EDControlButton.CyclePreviousHostileTarget,
                        EDControlButton.CycleNextSubsystem,
                        EDControlButton.CyclePreviousSubsystem,
                        EDControlButton.SelectTargetsTarget,

                        // Combat
                        EDControlButton.TargetNextRouteSystem,
                        EDControlButton.PrimaryFire,
                        EDControlButton.SecondaryFire,
                        EDControlButton.CycleFireGroupNext,
                        EDControlButton.CycleFireGroupPrevious,
                        
                        // Pip management
                        EDControlButton.IncreaseEnginesPower,
                        EDControlButton.IncreaseWeaponsPower,
                        EDControlButton.IncreaseSystemsPower,
                        EDControlButton.ResetPowerDistribution,
                    }
            },
            { ControlsMode.FSS,
                new List<EDControlButton>()
                    {
                        EDControlButton.ExplorationFSSShowHelp,
                        EDControlButton.ExplorationFSSDiscoveryScan,
                        EDControlButton.ExplorationFSSCameraPitchDecreaseButton,
                        EDControlButton.ExplorationFSSCameraPitchIncreaseButton,
                        EDControlButton.ExplorationFSSCameraYawDecreaseButton,
                        EDControlButton.ExplorationFSSCameraYawIncreaseButton,
                        EDControlButton.ExplorationFSSMiniZoomIn,
                        EDControlButton.ExplorationFSSMiniZoomOut,
                        EDControlButton.ExplorationFSSTarget,
                        EDControlButton.ExplorationFSSZoomIn,
                        EDControlButton.ExplorationFSSZoomOut,
                    }
            },
            { ControlsMode.DSS,
                new List<EDControlButton>()
                    {
                        EDControlButton.ExplorationSAAExitThirdPerson,
                        EDControlButton.ExplorationSAANextGenus,
                        EDControlButton.ExplorationSAAPreviousGenus,
                    }
            },
            { ControlsMode.Menu,
                new List<EDControlButton>()
                    {
                        EDControlButton.UI_Up,
                        EDControlButton.UI_Down,
                        EDControlButton.UI_Left,
                        EDControlButton.UI_Right,
                        EDControlButton.UI_Select,
                        EDControlButton.UI_Back,
                    }
            },
            { ControlsMode.Landing,
                new List<EDControlButton>()
                    {
                        EDControlButton.YawLeftButton_Landing,
                        EDControlButton.YawRightButton_Landing,
                        EDControlButton.PitchUpButton_Landing,
                        EDControlButton.PitchDownButton_Landing,
                        EDControlButton.RollLeftButton_Landing,
                        EDControlButton.RollRightButton_Landing,
                        EDControlButton.LeftThrustButton_Landing,
                        EDControlButton.RightThrustButton_Landing,
                        EDControlButton.UpThrustButton_Landing,
                        EDControlButton.DownThrustButton_Landing,
                        EDControlButton.ForwardThrustButton_Landing,
                        EDControlButton.BackwardThrustButton_Landing,
                    }
            },
            
        };
    }
}
