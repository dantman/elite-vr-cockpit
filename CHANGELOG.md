# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and this project tries to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Update Saved Games folder detection to fix issues with non-default user folder locations
- Add a button to the Desktop UI to open the UI to edit controller bindings for controllers that do not have bindings configured yet
- Fixed an issue with one vJoy device not being relinquished (credits to @yschuurmans)
- Fixed an issue with Alt button not working (credits to @yschuurmans)
- Fixed the "safe" trackpad sensitivity fallback so it's not so weak that some trackpads can't even get a single interval in
- Implemented joystick binding actions (only bound by default in Index bindings)
- Tuned the Index controller's bindings:
  - Tuned the trackpad sensitivity
  - Added Menu Select to the A button
  - Added a menu mode UINestedToggle button to thumbstick click
  - Added thumbstick bindings for menu/ui/pov2
- Fixed a mistake that resulted in the Primary/Secondary/Alt buttons in the Controls edit UI not working

## [0.6.0] - 2019-12-09

- Update SteamVR Plugin to 2.5
- **[Breaking]**: Replaced the legacy input system with the SteamVR Input API.
  - Vive controller, Index controller, and Oculus Touch bindings are included however only the Vive controller bindings are tested. Bindings on other platforms will have changed.
  - WMR bindings are not included, WMR controllers will no longer work without setting up user bindings.
  - POV1/2, Menu navigation, and Galaxy/UI navigation now have separate actions that can be bound in different ways.
- **[Breaking]**: Upgrade vJoy interface to 2.1.9. You will need to upgrade your vJoy installation if you have not already done so.
- Add buttons to the controls panel to display SteamVR bindings information and open the SteamVR bindings UI for Elite VR Cockpit.
- Change the controller pose from raw to grip. This may change the controller orientation on controllers other than the Vive wand.
- Display automatically generated information about joystick button and POV controller bindings in the controls panel.
- Trackpad sensitivity is now handled per controller type. Vive wands will retain the current sensitivity; all others will default to an excessively "safe" value until we are able to tune them.
- Project code has been relicensed from LGPL to MPL-2.0
- Releases now contain documentation and license information
- Fixed an issue with detecting Elite Dangerous when overlay is started after Elite Dangerous
- Fixed an issue with seated position not being updated when reset via the dashboard
- Added combat/analysis HUD mode button
- Added night vision toggle button
- **[Breaking]**: vJoy Device 2 now must also be configured.
- **[Breaking]**: Galaxy map axis bindings have changed, you will need to recreate your galaxy map bindings.
- **[Breaking]**: Because of the use of multiple devices, device ordering bugs may require you recreate all your joystick/throttle axis and button bindings (or manually update the DeviceIndex).
- Fixed a bug in the meta panel that caused it to re-render more often than necessary
- Implement a fixed position control axis to control the ship's the radar range
- Implement a power distribution panel

## [0.5.0] - 2019-02-24

- Add basic Galaxy/System map controls.
- Handle GUI focus for the new Orrery, Full Spectrum System Scanner, Detailed Surface Scanner, and Codex modes.
- Fix edit mode performance issues related to the size of the edit panel's texture.
- Reduce the number of key presses that get missed by ED.
- Allow holo buttons and POV buttons to be held down. Notably menu buttons can be held down so you can hold up/down to quickly scroll through the menu.
- SRV controls: Add holo buttons for controls that have SRV specific bindings in the driving controls. Also add headlights, dismiss/recall ship, deploy retract. And remove a few more ship specific holo buttons from the SRV.
- Fix broken modifier key bindings and add slash and capslock handling.
- Improve error handling for broken GraphicsConfigurationOverride files.
- Fix buttons sticking down when you let go of a control and controls being stuck in their input when they disappear due to mode switches.
- **[Breaking]**: Fix the joystick movement being inverted as of the Beyond update. You will need to flip your regular/inverted setting for your ship and driving pitch axis to keep the same controls as before.
- Make the Vive trackpad's "center" area for UI select and POV center button presses wider so it's easier to press.

## [0.4.0] - 2018-10-28

* Add the ability to control game menus by enabling a menu mode with a button
* Ability to override the current cockpit mode while in edit mode to display buttons for Main Ship, Fighter, SRV, and Map when not actually in those modes
* Controls panel for setting throttle/joystick axis bindings and joystick button bindings directly
* Add deadzone to the thrusters on the 6DOF controller
* Improve console handling of long multi-line console messages
* Display a helpful error message when encountering an Xml parse error in the graphics overrides file, instead of failing to start properly
* Display the name of the current scene application in the desktop UI to debug cases where the overlay cannot properly detect when ED is running

## [0.3.0] - 2018-09-16

- Quit gracefully instead of crashing when SteamVR closes
- Enable the Escape button
- Add a new 6DOF Controller when docking/landing or flying with FA off
- Add some optimizations that should hopefully reduce performance issues
- Fix bug that stopped the edit button from being movable
- Increase the toggle grab interval to make it easier to use
- Fix the save code so it doesn't exclude buttons that are not visible at the time you lock the edit button

## [0.2.0] - 2018-09-14

- Fix errors when a user has no custom bindings file or an old custom bindings file
- Fix error when a user has not configured a custom HUD color matrix
- Add drive assist button
- Add more button icons
- Add toggle grab support (credits to @dhleong)

## [0.1.1] - 2018-09-12

- Fix a number of bugs that stopped the standalone build in 0.1.0 from working.

## [0.1.0] - 2018-09-12 [YANKED]

- Initial early-alpha release with a collection of the basic functionality implemented.
