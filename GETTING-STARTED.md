Getting Started
===============

## 1. vJoy

Before running the Elite VR Cockpit you will need to install and configure [vJoy](http://vjoystick.sourceforge.net/site/).

Open up vJoy Configure and make sure you have the following configured:

* Axis: To simplify things make sure all axis are enabled
* Buttons: For now 8 buttons are enough, but you can future proof your setup by setting it to 32
* POV/HAT switches: Configure POVs to "4 Directions" and make sure there are 4 POVs configured

## 2. Download and run

Download the latest version of the Elite VR Cockpit from the [Releases](https://github.com/dantman/elite-vr-cockpit/releases) and extract it wherever you want. There is no installer so "Elite VR Cockpit.exe" can be run directly.

## 3. Running

Startup `Elite VR Cockpit.exe` and start Elite Dangerous. For now you will still need a keyboard or gamepad to navigate the menu. I recommend starting up the training missions to start configuring bindings instead of jumping straight into the game.

If your seated position isn't in the right spot, hold down the application menu buttons on both of your controllers for a few seconds and the seated position will be reset.

## 4. Axis bindings

In order for the throttle, joystick, and 6dof controller to work you will need to bind their axis in the controls

![Controls](Images/ScreenshotControls.png)

You can grab the joystick or throttle and rotate/move it after selecting a binding to send that axis to ED to bind it to that input. Likewise on the 6dof controller you can grab it and move it to send the thruster bindings.

Make sure you have the following bound, none of them should be inverted:

- `[JOY RZ-AXIS]` on yaw axis
- `[JOY X-AXIS]` on roll axis
- `[JOY Y-AXIS]` on pitch axis
- `[JOY Z-AXIS]` on throttle axis
- `[JOY RX-AXIS]` on the lateral thruster axis
- `[JOY RY-AXIS]` on the vertical thruster axis
- `[JOY U-AXIS]` on the forward/backward thruster axis

![Throttle](Images/ScreenshotThrottle.png) ![Joystick](Images/ScreenshotJoystick.png) ![6DOF Controller](Images/Screenshot6DOFController.png)

## 5. Joystick and throttle button bindings

The trigger and application menu are bound to separate joystick buttons. You can bind these to any control option you want by selecting a control option grabbing onto the throttle or joystick and pulling the trigger/pressing the application menu button.

Personally I recommend the following bindings:

* Primary fire: Trigger on the Joystick
* Secondary fire: Application menu on the Joystick
* Boost: Application menu on the Throttle
* Chaff: Trigger on the Throttle (if you have it)

## 6. Joystick HAT/POV switches

When grabbing the joystick the touchpad is bound to 2 separate joystick HAT/POV switches and a button.

![Trackpad HAT Regions](Images/TrackpadHAT.png)

- Pressing the trackpad in the center of the trackpad will output a virtual joystick BTN4 button press.
- Pressing the edges of the trackpad will output directional presses of the POV1 HAT switch.
- Dragging your finger along the trackpad will output directional presses of the POV2 HAT switch at regular intervals.

I recommend taking advantage of this to bind UI up/down/left/right to one of the POV switches and left/right on the other to UI previous/next category. It's up to you whether you want to slide your finger to navigate and press the edges to switch categories. or press in a direction to navigate and slide your finger to change categories.

Either way I also recommend binding the center button to UI select and the application menu to UI back.

Elite Dangerous still allows you to bind a button bound to a UI input to other functions. So even when binding the joystick's hat switches to UI actions you can still bind the hat switches to other controls that will be used when not looking at a holo panel.

For instance you may want to bind a direction to cycle fire groups, another to select a target you are looking at, and other targeting controls.

## 7. Holographic buttons

![Holographic buttons](Images/ScreenshotButton.png)

In front of you there should be an "Edit Mode" button with a padlock icon. Touch it with the circle on your controller and pull the trigger to interact with it and unlock edit mode.

When edit mode is unlocked you will be able to use the grip button to reposition the edit button, joystick/throttle, buttons, and any edit mode panel.

![Move buttons](Images/ScreenshotButtonMove.png)

The "Add Button" panel that shows up in front of you can be used to spawn new holographic buttons.

![Add buttons](Images/ScreenshotEditPanelButtons.png)

Buttons can be removed by moving them close to the trash icon and leaving them there for a few seconds.

![Remove buttons](Images/ScreenshotButtonTrash.png)

## 8. Holographic button bindings

You do not need to setup a specific button layout in order to use ship function buttons. Any keyboard binding you have configured for a control will be read from your custom bindings and the button will press that button combo.

However this does mean that keyboard bindings must be setup for any system function button that you wish to use. If a control is unconfigured or only has non-keyboard bindings such as gamepad bindings it will not be usable. If Elite VR Cockpit cannot find keyboard bindings for a button it will appear red when you spawn it.

![Holographic button with no keyboard binding](Images/ScreenshotButtonNoBindings.png)

Some of the buttons have a default key binding they will emit when red. If you don't have a specific key you wish to bind a control to you can select the relevant binding in your controls menu and press the red button to give it the default binding if the button has one. Otherwise, open up your controls menu and assign a keyboard binding to any of the controls you want to have red holographic buttons for.

Be sure to restart either Elite Dangerous or the overlay after you update your bindings. We don't currently detect changes to bindings while the game is running.
