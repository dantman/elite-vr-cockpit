Elite VR Cockpit (Beta)
================

**Elite VR Cockpit** is a SteamVR overlay for Elite Dangerous. The overlay is currently in an [beta stage](#known-limitations-of-beta-version) but is sufficient to play in every mode except for on-foot (which you can still play with keyboard/mouse).

* [Download](https://github.com/boyestrous/elite-vr-cockpit/releases)
* [Getting Started](GETTING-STARTED.md)
* [Youtube - Getting Started](https://www.youtube.com/channel/UCZcxz-04m5DO8kJgm1qorFA)
* [Report a bug](https://github.com/boyestrous/elite-vr-cockpit/issues)
* [Oculus Rift SteamVR Workaround](OCULUS-WORKAROUND.md)

<table>
  <tr>
    <td style="width: 200;
    height: 150;
    overflow: hidden">
      <img src="Assets/Documentation/Images/ScreenshotThrottle.png" alt="Throttle">
    </td>
    <td style="width: 200;
    height: 150;
    overflow: hidden">
      <img src="Assets/Documentation/Images/ScreenshotJoystick.png" alt="ScreenshotJoystick">
    </td>
    <td style="width: 200;
    height: 150;
    overflow: hidden">
      <img src="Assets/Documentation/Images/ScreenshotButton.png" alt="ScreenshotButton">
    </td>
  </tr>
</table>


# Features

## Virtual throttle and joystick

A virtual throttle and virtual joystick are overlayed on top of the in-game throttle/joystic. You can intuitively interact with these controls, just like you would if you had a physical HOTAS!

There are some additional quality-of-life features offered:
- Once you've grabbed a control, you don't have to keep your hand in that position. So you can grab the joystick then move your hand and rest the base of your controller on your leg or chair-arm to get a stable base to move around the joystick. _No need to float your hand in mid-air_.
- Edit mode allows you to reposition any holographic element, so you can place controls where they are comfortable for you.
- Holographic element positions are stored separately for your ship, SRV, and fighter, due to the different cockpit layouts.
- Holographic controls can indicate whether a control is active or inactive, so you can easily see whether your cargo scoop/hardpoints/etc are deployed

### HAT/POV switch
Similar to physical HOTAS controls, there are directional "HAT"/"POV" controls that can be used while you're gripping the virtual throttle or virtual joystick. There are _up to_ 2 POVs per hand, but most controllers just have a single POV.

If you have a thumbstick then thumbstick directions will be bound to directions on POV1 and thumbstick click will be bound to the POV1 center button.

If you have a small trackpad, then swipes on that trackpad will be bound to directions on POV1 and trackpad presses will be bound to the POV1 center button.

However if you have a large trackpad like on the Vive wands this trackpad may be bound to 2 POVs. Directional **swipes** will be for POV2 and directional **presses** will be for POV1. Center press will always be for the POV1 center button.

![Trackpad HAT Regions](Assets/Documentation/Images/TrackpadHAT.png)

- Pressing the trackpad in the center of the trackpad will be bound to POV1 center press.
- Pressing the edges of the trackpad will be bound to directional presses on POV1.
- Dragging your finger along the trackpad will be bound to directional presses of POV2 at regular intervals.

## Holographic buttons

The overlay provides a variety of ship functions as holographic buttons.

* *(Some button icons have not been gathered yet)*
* Buttons can be activated by pulling the trigger while interacting with them.
* When edit mode is unlocked, buttons can be grabbed by holding down the grip button (the edit mode button itself included) and repositioned wherever you like. *(positions will be saved when edit mode is re-locked)*
* Your in-game bindings will be read and the button will output whatever key-combination that function is bound to.
  * A binding must be present for the holographic button to work properly. *any* binding will do... Buttons for ship functions that you have not bound to a keyboard key yet will appear red.
* Most toggle functions have tooltips that will automatically change when ship status changes. (e.g. The landing gear button will switch between "Deploy landing gear" and "Retract landing gear" depending on whether the gear is up or down)
* New holographic buttons can be spawned and deleted from the Desktop Interface

## Elite Dangerous integration

The overlay is integrated with the status API provided by the game.
- The controls will change based on your in-game status and focus
  - Maps, Station Services, and FSS Mode don't show the Virtual Joystick/Throttle
  - SRV, Fighter, and Main Ship have separate sets of holographic buttons and a separate joystick/throttle, so you can save different placements for each view.
- You will be warned when ED not focused and will not receive input from the virtual joystick, throttle, or holographic buttons.

Journal API is not used. We recommend using EDDI for additional timely information.

# Known Limitations of Beta Version
* *Some keyboard keys currently do not have mappings* - We occasionally find keyboard buttons that aren't recognized. Buttons like the tilda (~), backtick (`), and apostrophe (') will occasionally throw strange characters. If you see this error, submit an issue and we'll add it.
* On Foot controls aren't supported _yet_. Use your keyboard/mouse.
* Holographic buttons will not respond to custom cockpit color schemes. Enjoy the default Orange!

# License

See the [LICENSE.md](LICENSE.md) file for license information.
