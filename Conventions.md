# Conventions

## Basic guidelines to follow
1. Modular
    a) Systems not directly dependent on each other
        - ScriptableObject varaiables should be referenced to share data between systems
    b) Prefabs are self-contained
        - All logic should be within the prefab (as components)
        - GameEvents can be used to test external stuff
        - ScriptableObject references for external variables (even if something else populates the variables)
    c) Use Components that do only one thing
    d) Child gameobjects group by function/purpose?
2. Editable
    a) Change the game without code
        - ScriptableObject references can be changed in the editor
        - swap/modify component settings from editor to change behavior
3. Debuggable
    a) Test in isolation (modularity)
        - prefabs can be loaded into a scene with nothing else and work as expected
        - build test settings into prefab (if we need to load data to a ScriptableObject/init a GameEvent, etc.)

## Implementation
1. Base ScriptableObjects Types
    - [GameEvents & Listeners](#custom-gameevents)
    -


## Naming Conventions
### Inputs/Bindings/Controls/etc.
_Input Actions are designed 1to separate the logical meaning of an input from the physical means of input (that is, activity on an input device) that generate the input._
a) `Input Device`: physical peice of hardware
b) `Control`: separate, individual part of an input device. For example, an xbox controller's controls include, A/B/X/Y, two Joysticks, two triggers, etc.
c) `Interaction`: the ways to use a control. For example, You can click a joystick or move it in a direction. You can press, long press, double tap a button.
d) `Action`: A logical input such as "Select Target" or "Engage FSS"
e) `Binding`: A connection between an Action and one or more Controls represented by a control path. At run time, a Binding is resolved to yield zero or more Controls, which the Input System then connects to the Action.

7. SteamVR Controls vs vJoy Bound controls
    - only use SteamVR controls when a native binding isn't accessible. In other words: if you can map it by editing the Custom.X.0.binds file to a vJoy control, do it that way.
    Examples of good uses for SteamVR:
        - FSS Mode => Camera Activate
            - This control doesn't exist in the bindings file for Elite Dangerous
        - InteractUI => default SteamVR behavior and is used to do holographic button presses

****************************************


# Custom GameEvents
These custom scripts allow for an improved Unity event system by creating a more flexible and extensible approach to event-driven programming. This pattern was first presented by Ryan Hipple in a 2017 Unite Austin talk.

GameEvents are found in the `Assets > GameEvents` folder. You can create new assets through the `right-click menu > GameEvents`
- Base Event (no parameters): raises an event with no parameters
- Any other label: will raise the parameter type listed
    - ex: CockpitMode

## Overview
Traditionally, Unity events are tied to a specific object or script, making them less flexible and difficult to use in complex systems. The custom GameEvent scriptable object and GameEventListener MonoBehaviour address this issue by providing a more decoupled approach to events. When implemented correctly, you can drop any prefab into a scene *alone* for debugging/testing.

A GameEvent is a scriptable object that holds a list of GameEventListeners, which are MonoBehaviours that listen for the event to be raised. When a GameEvent is raised, all of its listeners are notified, allowing for multiple objects across the scene to respond to the event without requiring direct references to each other. This behavior isn't significantly different than the standard event listener behavior. The only real difference is that the GameEvent is defined through a ScriptableObject instead of inside a class and the GameEventListener is a separate Monobehavior component.

## Usage
Unless you need to create a new type of GameEvent or customize the listener, you should be able to do **most** of this from the inspector.
To use this pattern in your project, follow these steps:

1. Create a new GameEvent scriptable object in your project by right-clicking in the Project window and selecting "Create > GameEvent".
2. In any script that needs to raise the event, create a public reference to the GameEvent object and attach the scriptableObject in the inspector
3. Call the Raise method when appropriate (typically where you would Send a SteamVR_Event)
4. In any script that needs to listen for the event, attach a GameEventListener component to the gameObject. The "standard" UnityEvent menu will be displayed in the inspector. Follow normal procedures to define the behavior that needs to happen in response to the event.
5. For more information on how to use these custom scripts, see [Ryan Hipple's 2017 Unite Austin](https://www.youtube.com/watch?v=raQ3iHhE_Kk) talk or the Unity documentation.

## Extending this Pattern
**You only need to create new scripts if you need a GameEvent that requires a new type of parameter**
There are templates for both the GameEventListener and the GameEvent. They should appear in the right-click menu, but if not they're in the ScriptTemplates folder.

## Benefits
The custom GameEvent and GameEventListener scripts offer several benefits over traditional Unity events, including:

- Decoupling of events from specific objects or scripts
- Simplification of complex systems by allowing multiple objects to listen for the same event without direct references to each other
- Increased flexibility by allowing for custom event parameters
- Reduced overhead by eliminating the need for Unity's event system to manage event listeners

<hr/>
# Structure (Maybe)
1. Monitors - check for information (bindings update, game state changes, etc.). Send Unity Events
    - File system watcher
    - Wrist turn watcher
    - Game status watcher
    -
2. Managers - non-unity states, assets, settings
    - InputBindingNameInfoManager
    - EDStateManager
    - EDControlBindings (EDControlManager)ED
    - vJoyInterface (vJoyManager)
        - SetButton
        - SetMapAxis
        - EnableMapAxis
        - IsDeviceValid()
        - Acquire Device
    - KeyboardInterface (KeyboardManager)
3. Controllers - interact with unity objects, control logic. Should be a monobehavior. Will often have an associated Manager
    - ActionsController?
    - vJoyInterface => move Monobehavior methods to a new class
    - KeyboardInterface => move Monobehavior methods to a new class
4. Interfaces - Helper functions for the primary type and map connections to other classes. Should **not** be a monobehavior class


5. GameObject Scripts - attached directly

**Maybe:**
1. Service - Loads/sends data from any backend services, Sends Events

