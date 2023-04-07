# GameEvent System
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
3. Call the Raise method when appropriate (typically where you would _Send_ a SteamVR_Event)
4. In any script that needs to listen for the event, attach a GameEventListener component to the gameObject. The "standard" UnityEvent menu will be displayed in the inspector. Follow normal procedures to define the behavior that needs to happen in response to the event.
5. For more information on how to use these custom scripts, see [Ryan Hipple's 2017 Unite Austin](https://www.youtube.com/watch?v=raQ3iHhE_Kk) talk or the Unity documentation.

## Extending this Pattern
**You only need to create new scripts if you need a GameEvent that requires a new type of parameter**
There are templates for both the GameEventListener and the GameEvent. They should appear in the `right-click menu > EVRC Script Templates`, but if not they're in the ScriptTemplates folder.

## Benefits
The custom GameEvent and GameEventListener scripts offer several benefits over traditional Unity events, including:

- Decoupling of events from specific objects or scripts
- Simplification of complex systems by allowing multiple objects to listen for the same event without direct references to each other
- Increased flexibility by allowing for custom event parameters
- Reduced overhead by eliminating the need for Unity's event system to manage event listeners