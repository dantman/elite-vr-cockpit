Elite VR Cockpit (Alpha)
================

**Elite VR Cockpit** is a SteamVR overlay for Elite Dangerous. The overlay is currently in an early alpha stage but contains a fair bit of basic functionality.

*Note: The early alpha has only been tested on the Vive with the Vive wands. Normally other HMDs work fine without any extra work. However the virtual HAT/POV switch on the joystick is designed around the Vive's trackpad and code to implement the equivalent on controllers with joysticks (Oculus Touch) or Joystick + small trackpad/trackbutton pairs (Windows Mixed Reality and Knuckles) has not been implemented yet.*

* [Download](https://github.com/dantman/elite-vr-cockpit/releases)
* [Getting Started](GETTING-STARTED.md)
* [Report a bug](https://github.com/dantman/elite-vr-cockpit/issues)
* [Oculus Rift SteamVR Workaround](OCULUS-WORKAROUND.md)


![Throttle](Images/ScreenshotThrottle.png) ![Joystick](Images/ScreenshotJoystick.png) ![6DOF Controller](Images/Screenshot6DOFController.png) ![Buttons](Images/ScreenshotButton.png) ![Map](Images/ScreenshotMapControls.png)


## Warning for OpenVR Advanced Settings users

OpenVR Advanced Settings (OVAS) alters how SteamVR's tracking origins and seated position resets function. Two times in the past this has resulted in an OVAS bug that breaks the Elite VR Cockpit overlay or breaks Elite Dangerous. If you use OVAS please make sure you are running the latest version to ensure you are not running one of the broken versions of OVAS. Additionally if you encounter a bug where resetting the seated position does not work or the cockpit overlay is in the wrong location and resetting the seated position does not work, please uninstall OpenVR Advanced Settings first and check that the bug still exists before reporting it as a bug in Elite VR Cockpit.

## Features

### Seated position reset

The overlay has a seated position reset binding that can be bound to buttons on the VR controllers. This will reset your seated position to your current HMD location. Without needing to grab your keyboard or open the SteamVR dashboard settings.

- Vive wands: Hold down the application menu button on **both** of the Vive wands for a few seconds.
- Valve Index controllers: Click and the thumb sticks on **both** Index controllers for a few seconds.

### Virtual throttle and joystick

A virtual throttle and virtual joystick are located in the location of the in-game's throttle and joystick.

- The controls can be grabbed with the grip button to interact with them.
  - If you press and release the grip button quickly you will continue grabbing until you press and release the grip button again.
- The joystick is VTOL VR inspired. Only rotation of the controller is tracked. So you can grab the joystick then move your hand and rest the base of your controller on your leg to get a stable base to move around the joystick. No need to float your hand in mid-air.
- When edit mode is unlocked you can grab with the grip button and reposition the throttle and joystick.
- The trigger and application menu button are bound to different joystick buttons depending on whether they are grabbing the joystick or throttle.
- There are separate throttle/joystick positions for ships and for SRVs due to the different cockpit layouts.
  - The overlay automatically switches layouts when you deploy/dock your SRV.

#### HAT/POV switch

When grabbing the joystick the trackpads and thumbsticks on your controller are bound to 2 separate joystick HAT/POV switches and a pair of HAT/POV center buttons (which are associated with BTN4 and BTN5). You can check what inputs are bound to what POVs on the controls panel that is visible when edit mode and menu mode are both turned on.

If you have a thumbstick then thumbstick directions will be bound to directions on one of the POVs and thumbstick click will be bound to that POV's center button.

If you have a small trackpad then swipes on that trackpad will be bound to directions on one of the POVs and trackpad press will be bound to that POV's center button.

However if you have a large trackpad like on the Vive wands this trackpad may be bound to 2 POVs.

![Trackpad HAT Regions](Images/TrackpadHAT.png)

- Pressing the trackpad in the center of the trackpad will be bound to POV1 center press.
- Pressing the edges of the trackpad will be bound to directional presses on POV1.
- Dragging your finger along the trackpad will be bound to directional presses of POV2 at regular intervals.

For example on the Vive you could bind BTN4 to UI select. The POV2 direction to UI navigation directions like an omidirectional scroll wheel. And POV1 left/right to next/previous category. Which would allow you to slide your finger on the trackpad to navigate through holo panel options, press the edges of the trackpad to switch panel categories, and press the middle to select.

On controllers with both a small trackpad and a thumbstick you could bind one of the POVs to UI navigation and the other to next/previous category.

### Holographic buttons

The overlay provides a variety of ship functions as holographic buttons.

* *(Some button icons have not been gathered yet)*
* Buttons can be activated by pulling the trigger while interacting with them.
* When edit mode is unlocked, buttons can be grabbed by holding down the grip button (the edit mode button itself included) and repositioned wherever you like. *(positions will be saved when edit mode is re-locked)*
* Ship function buttons are not hardcoded to specific key/button bindings.
  * Your custom bindings will be read and the button will output whatever keypress that function is bound to. So you do not need specific bindings to use the overlay.
  * However all ship functions you wish to use must have *a* keyboard binding to work. Buttons for ship functions that you have not bound to a keyboard key yet will appear red.
  * Some ship functions have a "default" key they will output when no keyboard key is bound. This will allow you to easily bind controls without pulling out a keyboard by selecting an option in the controls menu and pressing a red ship function button to output the default key binding.
  * *(Some keyboard keys currently do not have mappings)*
* Most toggle functions have tooltips that will automatically change when ship status changes. (e.g. The landing gear button will switch between "Deploy landing gear" and "Retract landing gear" depending on whether the gear is up or down)
* New ship function buttons can be spawned from a button panel and deleted by moving them onto a trashbin icon if you do not need them.

### Elite Dangerous integration

The overlay is integrated with the status API provided by the game.

- Overlay items will not appear until the game begins to run.
- Cockpit controls will disappear when switching to the map screens.
- Some buttons for functions only available in the main ship will disappear when switching to a figher or SRV.
- Virtual joystick and throttle will change position when you switch to the SRV.
- Ship function holobuttons automatically use any keyboard bindings your ED custom bindings so you don't need specific configuration for them.
- You will be warned when ED not focused and will not receive input from the virtual joystick, throttle, or holographic buttons.

## License

See the [LICENSE.md](LICENSE.md) file for license information.
