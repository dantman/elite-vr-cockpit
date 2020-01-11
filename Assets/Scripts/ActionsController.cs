using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;
    using NameType = InputBindingNameInfoManager.NameType;

    public class ActionsController : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float trackpadCenterButtonRadius = 0.5f;

        public enum InputAction
        {
            // Basic interactions
            InteractUI,
            GrabHold,
            GrabToggle,
            GrabPinch,
            // Seated position resets
            ResetSeatedPosition,
            MaybeResetSeatedPosition,
            // Control buttons
            ButtonPrimary,
            ButtonSecondary,
            ButtonAlt,
            ButtonPOV1,
            ButtonPOV2,
            // Menu/UI buttons
            MenuBack,
            MenuSelect,
            MenuNestedToggle,
            MenuNavigateUp,
            MenuNavigateDown,
            MenuNavigateLeft,
            MenuNavigateRight,
            UIBack,
            UISelect,
            // @todo UI Nested Toggle (Expand/Collapse)
            UINavigateUp,
            UINavigateDown,
            UINavigateLeft,
            UINavigateRight,
            UITabPrevious,
            UITabNext,
            // Trackpad POV/Menu/UI
            POV1Trackpad,
            POV2Trackpad,
            MenuNavigateTrackpad,
            UINavigateTrackpad,
            UITabTrackpad,
            // Joystick POV/Menu/UI
            POV1Joystick,
            POV2Joystick,
            MenuNavigateJoystick,
            UINavigateJoystick,
            UITabJoystick,
        }

        public enum OutputAction
        {
            InteractUI,
            GrabHold,
            GrabToggle,
            GrabPinch,
            ResetSeatedPosition,
            // POV
            POV1,
            POV2,
            // Control buttons
            ButtonPrimary,
            ButtonSecondary,
            ButtonAlt,
            // Menu
            MenuBack,
            MenuSelect,
            MenuNestedToggle,
            MenuNavigate,
            // UI Navigate/Tab
            UIBack,
            UISelect,
            UINavigate,
            UITabPrevious,
            UITabNext,
        }

        public struct ActionChange
        {
            public OutputAction action;
            public Hand hand;
            public bool state;

            public ActionChange(Hand hand, OutputAction action, bool state)
            {
                this.hand = hand;
                this.action = action;
                this.state = state;
            }
        }

        public struct DirectionActionChange
        {
            public OutputAction action;
            public Hand hand;
            public Direction direction;
            public bool state;

            public DirectionActionChange(Hand hand, OutputAction action, Direction direction, bool state)
            {
                this.hand = hand;
                this.action = action;
                this.direction = direction;
                this.state = state;
            }
        }

        private static Dictionary<OutputAction, Events.Event<T>> GenerateEventsForOutputActions<T>()
        {
            var values = Enum.GetValues(typeof(OutputAction));
            var dict = new Dictionary<OutputAction, Events.Event<T>>(values.Length);
            foreach (OutputAction outputAction in values)
            {
                dict[outputAction] = new Events.Event<T>();
            }
            return dict;
        }

        public static Dictionary<OutputAction, Events.Event<ActionChange>> ActionPressed = GenerateEventsForOutputActions<ActionChange>();
        public static Dictionary<OutputAction, Events.Event<ActionChange>> ActionUnpress = GenerateEventsForOutputActions<ActionChange>();
        public static Dictionary<OutputAction, Events.Event<DirectionActionChange>> DirectionActionPressed = GenerateEventsForOutputActions<DirectionActionChange>();
        public static Dictionary<OutputAction, Events.Event<DirectionActionChange>> DirectionActionUnpressed = GenerateEventsForOutputActions<DirectionActionChange>();

        public static Events.Event<Hand, Vector3?, Quaternion?> HandPoseUpdate = new Events.Event<Hand, Vector3?, Quaternion?>();

        public enum Hand
        {
            Unknown,
            Left,
            Right,
        }

        public enum Direction : byte
        {
            Up,
            Right,
            Down,
            Left
        }

        private delegate void BooleanInputActionHandler(InputAction inputAction, Hand hand, bool newState);
        private delegate void Vector2InputActionHandler(Hand hand, Vector2 newState);
        private delegate IEnumerator TrackpadInputActionHandler(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running);
        private delegate void TrackpadPressActionHandler(InputAction inputAction, Hand hand, Vector2 position, bool newState);
        private delegate void JoystickPositionChangeActionHandler(InputAction inputAction, Hand hand, Vector2 axis);

        private delegate bool EmitStateChangeDelegate(bool newState);
        private delegate bool EmitDirectionStateChangeDelegate(Direction direction, bool newState);

        private Dictionary<InputAction, BooleanInputActionHandler> booleanInputActionHandlers;
        private Dictionary<InputAction, Vector2InputActionHandler> vector2InputActionHandlers;
        private Dictionary<InputAction, TrackpadInputActionHandler> trackpadInputActionHandlers;
        private Dictionary<InputAction, TrackpadPressActionHandler> trackpadPressActionHandlers;
        private Dictionary<InputAction, JoystickPositionChangeActionHandler> joystickActionHandlers;

        private Dictionary<InputAction, OutputAction> simpleBooleanActionMapping;
        private Dictionary<InputAction, (OutputAction, Direction)> directionalBooleanActionMapping;
        private Dictionary<InputAction, OutputAction> directionalTrackpadSlideMapping;
        private Dictionary<InputAction, (OutputAction, OutputAction)> trackpadPressActionMapping;
        private Dictionary<InputAction, OutputAction> joystickDirectionActionMapping;

        private Dictionary<(InputAction, Hand), Action> trackpadPressUnpressHandler = new Dictionary<(InputAction, Hand), Action>();
        private Dictionary<(InputAction, Hand), (Direction, Action)> joystickDirectionUnpressHandler = new Dictionary<(InputAction, Hand), (Direction, Action)>();

        public static string[] GetBindingNames(IBindingsController bindingsController, OutputAction outputAction, NameType nameType)
        {
            string[] MergeBindings(params InputAction[] inputActions)
            {
                return inputActions.SelectMany(inputAction => bindingsController.GetBindingNames(inputAction, nameType)).ToArray();
            }

            switch (outputAction)
            {
                case OutputAction.ButtonPrimary: return MergeBindings(InputAction.ButtonPrimary);
                case OutputAction.ButtonSecondary: return MergeBindings(InputAction.ButtonSecondary);
                case OutputAction.ButtonAlt: return MergeBindings(InputAction.ButtonAlt);
                case OutputAction.POV1: return MergeBindings(InputAction.ButtonPOV1, InputAction.POV1Trackpad, InputAction.POV1Joystick);
                case OutputAction.POV2: return MergeBindings(InputAction.ButtonPOV2, InputAction.POV2Trackpad, InputAction.POV2Joystick);
            }

            throw new Exception(string.Format("OutputAction.{0} is not handled by GetBindingNames", outputAction));
        }

        void OnEnable()
        {
            booleanInputActionHandlers = new Dictionary<InputAction, BooleanInputActionHandler>
            {
                { InputAction.MaybeResetSeatedPosition, OnMaybeResetSeatedPosition },
            };
            vector2InputActionHandlers = new Dictionary<InputAction, Vector2InputActionHandler> { };
            trackpadInputActionHandlers = new Dictionary<InputAction, TrackpadInputActionHandler> { };
            trackpadPressActionHandlers = new Dictionary<InputAction, TrackpadPressActionHandler> { };
            joystickActionHandlers = new Dictionary<InputAction, JoystickPositionChangeActionHandler>();
            simpleBooleanActionMapping = new Dictionary<InputAction, OutputAction>();
            directionalBooleanActionMapping = new Dictionary<InputAction, (OutputAction, Direction)>();
            directionalTrackpadSlideMapping = new Dictionary<InputAction, OutputAction>();
            trackpadPressActionMapping = new Dictionary<InputAction, (OutputAction, OutputAction)>();
            joystickDirectionActionMapping = new Dictionary<InputAction, OutputAction>();

            // Basic interactions
            MapBooleanInputActionToOutputAction(InputAction.InteractUI, OutputAction.InteractUI);
            MapBooleanInputActionToOutputAction(InputAction.GrabHold, OutputAction.GrabHold);
            MapBooleanInputActionToOutputAction(InputAction.GrabToggle, OutputAction.GrabToggle);
            MapBooleanInputActionToOutputAction(InputAction.GrabPinch, OutputAction.GrabPinch);
            // Basic seated position reset
            MapBooleanInputActionToOutputAction(InputAction.ResetSeatedPosition, OutputAction.ResetSeatedPosition);
            // Throttle/Joystick controls
            MapBooleanInputActionToOutputAction(InputAction.ButtonPrimary, OutputAction.ButtonPrimary);
            MapBooleanInputActionToOutputAction(InputAction.ButtonSecondary, OutputAction.ButtonSecondary);
            MapBooleanInputActionToOutputAction(InputAction.ButtonAlt, OutputAction.ButtonAlt);
            MapBooleanInputActionToOutputAction(InputAction.ButtonPOV1, OutputAction.POV1);
            MapBooleanInputActionToOutputAction(InputAction.ButtonPOV2, OutputAction.POV2);
            // Menu/UI buttons
            MapBooleanInputActionToOutputAction(InputAction.MenuBack, OutputAction.MenuBack);
            MapBooleanInputActionToOutputAction(InputAction.MenuSelect, OutputAction.MenuSelect);
            MapBooleanInputActionToOutputAction(InputAction.MenuNestedToggle, OutputAction.MenuNestedToggle);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateUp, OutputAction.MenuNavigate, Direction.Up);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateDown, OutputAction.MenuNavigate, Direction.Down);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateLeft, OutputAction.MenuNavigate, Direction.Left);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateRight, OutputAction.MenuNavigate, Direction.Right);
            MapBooleanInputActionToOutputAction(InputAction.UIBack, OutputAction.UIBack);
            MapBooleanInputActionToOutputAction(InputAction.UISelect, OutputAction.UISelect);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateUp, OutputAction.UINavigate, Direction.Up);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateDown, OutputAction.UINavigate, Direction.Down);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateLeft, OutputAction.UINavigate, Direction.Left);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateRight, OutputAction.UINavigate, Direction.Right);
            MapBooleanInputActionToOutputAction(InputAction.UITabPrevious, OutputAction.UITabPrevious);
            MapBooleanInputActionToOutputAction(InputAction.UITabNext, OutputAction.UITabNext);
            // Trackpad POV/Menu/UI
            MapTrackpadSlideToDirectionOutputAction(InputAction.POV1Trackpad, OutputAction.POV1);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.POV1Trackpad, OutputAction.POV1, OutputAction.POV1);
            MapTrackpadSlideToDirectionOutputAction(InputAction.POV2Trackpad, OutputAction.POV2);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.POV2Trackpad, OutputAction.POV2, OutputAction.POV2);
            MapTrackpadSlideToDirectionOutputAction(InputAction.MenuNavigateTrackpad, OutputAction.MenuNavigate);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.MenuNavigateTrackpad, OutputAction.MenuNavigate, OutputAction.MenuSelect);
            MapTrackpadSlideToDirectionOutputAction(InputAction.UINavigateTrackpad, OutputAction.UINavigate);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.UINavigateTrackpad, OutputAction.UINavigate, OutputAction.UISelect);
            trackpadInputActionHandlers[InputAction.UITabTrackpad] = OnUITabTrackpadInput;
            trackpadPressActionHandlers[InputAction.UITabTrackpad] = OnUITabTrackpadPress;
            // Joystick POV/Menu/UI
            MapJoystickToDirectionOutputAction(InputAction.POV1Joystick, OutputAction.POV1);
            MapJoystickToDirectionOutputAction(InputAction.POV2Joystick, OutputAction.POV2);
            MapJoystickToDirectionOutputAction(InputAction.MenuNavigateJoystick, OutputAction.MenuNavigate);
            MapJoystickToDirectionOutputAction(InputAction.UINavigateJoystick, OutputAction.UINavigate);
            joystickActionHandlers[InputAction.UITabJoystick] = OnUITabJoystickAxisChange;
        }

        void OnDisable()
        {
        }

        #region Binding implementation interface
        /**
         * Interface for binding implementations to update the pose of a hand
         */
        public void HandPoseChange(Hand hand, Vector3? newPosition, Quaternion? newRotation)
        {
            HandPoseUpdate.Invoke(hand, newPosition, newRotation);
        }

        /**
         * Interface for input binding implementations to call when the state of a boolean input action has changed
         */
        public void TriggerBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (booleanInputActionHandlers.ContainsKey(inputAction))
            {
                booleanInputActionHandlers[inputAction](inputAction, hand, newState);
            }
            else
            {
                Debug.LogWarningFormat("No boolean handler for input action: {0}", inputAction.ToString());
            }
        }

        /**
         * Interface for input binding implementations to call to start a trackpad touching coroutine
         */
        public IEnumerator TriggerTrackpadInputAction(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running)
        {
            if (trackpadInputActionHandlers.ContainsKey(inputAction))
            {
                return trackpadInputActionHandlers[inputAction](inputAction, hand, position, running);
            }
            else
            {
                Debug.LogWarningFormat("No trackpad handler for input action: {0}", inputAction.ToString());
                return null;
            }
        }

        /**
         * Interface for input binding implementations to call when a trackpad has been pressed or released
         */
        public void TriggerTrackpadPressAction(InputAction inputAction, Hand hand, Vector2 position, bool newState)
        {
            if (trackpadPressActionHandlers.ContainsKey(inputAction))
            {
                trackpadPressActionHandlers[inputAction](inputAction, hand, position, newState);
            }
            else
            {
                Debug.LogWarningFormat("No trackpad press handler for input action: {0}", inputAction.ToString());
            }
        }

        /**
         * Interface for input binding implementations to call when a joystick's axis has changed
         */
        public void TriggerJoystickAxisChangeAction(InputAction inputAction, Hand hand, Vector2 axis)
        {
            if (joystickActionHandlers.ContainsKey(inputAction))
            {
                joystickActionHandlers[inputAction](inputAction, hand, axis);
            }
            else
            {
                Debug.LogWarningFormat("No joystick handler for input action: {0}", inputAction.ToString());
            }
        }
        #endregion

        private void EmitActionStateChange(Hand hand, OutputAction action, bool state)
        {
            var ev = new ActionChange(hand, action, state);
            if (state)
            {
                ActionPressed[action].Invoke(ev);
            }
            else
            {
                ActionUnpress[action].Invoke(ev);
            }
        }

        private void EmitDirectionActionStateChange(Hand hand, OutputAction action, Direction direction, bool state)
        {
            var ev = new DirectionActionChange(hand, action, direction, state);
            if (state)
            {
                DirectionActionPressed[action].Invoke(ev);
            }
            else
            {
                DirectionActionUnpressed[action].Invoke(ev);
            }
        }

        #region Boolean
        private void MapBooleanInputActionToOutputAction(InputAction inputAction, OutputAction outputAction)
        {
            simpleBooleanActionMapping[inputAction] = outputAction;
            booleanInputActionHandlers[inputAction] = OnMappedBooleanInputAction;
        }

        private void OnMappedBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (simpleBooleanActionMapping.ContainsKey(inputAction))
            {
                EmitActionStateChange(hand, simpleBooleanActionMapping[inputAction], newState);
            }
            else
            {
                Debug.LogWarningFormat("No simple boolean action mapping for input action: {0}", inputAction.ToString());
            }
        }

        private void MapBooleanInputActionToDirectionOutputAction(InputAction inputAction, OutputAction outputAction, Direction direction)
        {
            directionalBooleanActionMapping[inputAction] = (outputAction, direction);
            booleanInputActionHandlers[inputAction] = OnDirectionMappedBooleanInputAction;
        }

        private void OnDirectionMappedBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (directionalBooleanActionMapping.ContainsKey(inputAction))
            {
                var (outputAction, direction) = directionalBooleanActionMapping[inputAction];
                EmitDirectionActionStateChange(hand, outputAction, direction, newState);
            }
            else
            {
                Debug.LogWarningFormat("No directional boolean action mapping for input action: {0}", inputAction.ToString());
            }
        }
        #endregion

        #region Trackpad slide
        private void MapTrackpadSlideToDirectionOutputAction(InputAction inputAction, OutputAction outputAction)
        {
            directionalTrackpadSlideMapping[inputAction] = outputAction;
            trackpadInputActionHandlers[inputAction] = OnMappedTrackpadInput;
        }

        private IEnumerator OnMappedTrackpadInput(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running)
        {
            if (!directionalTrackpadSlideMapping.ContainsKey(inputAction))
            {
                Debug.LogWarningFormat("No trackpad input action mapping for input action: {0}", inputAction.ToString());
                return null;
            }
            var outputAction = directionalTrackpadSlideMapping[inputAction];

            return DoAbstractTrackpadInput(
                inputAction, hand, position, running,
                (dir, newButtonState) =>
                {
                    EmitDirectionActionStateChange(hand, outputAction, dir, newButtonState);
                    return true;
                });
        }

        private IEnumerator OnUITabTrackpadInput(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running)
        {
            return DoAbstractTrackpadInput(
                inputAction, hand, position, running,
                (dir, newButtonState) =>
                {
                    switch (dir)
                    {
                        case Direction.Left:
                            EmitActionStateChange(hand, OutputAction.UITabPrevious, newButtonState);
                            return true;
                        case Direction.Right:
                            EmitActionStateChange(hand, OutputAction.UITabNext, newButtonState);
                            return true;
                        default:
                            return false;
                    }
                });
        }

        public class TrackpadInterval
        {
            // A "safe" fallback interval that allows about 1 interval for a swipe across most of most trackpads
            public static readonly TrackpadInterval Default = Circular(.75f);

            public static TrackpadInterval Circular(float interval)
                => new TrackpadInterval { Horizontal = interval, Vertical = interval };
            public static TrackpadInterval Oval(float horizontal, float vertical)
                => new TrackpadInterval { Horizontal = horizontal, Vertical = vertical };

            public float Horizontal { get; private set; }
            public float Vertical { get; private set; }

            protected TrackpadInterval() { }

            /**
             * Get the Horizontal or Vertical interval as a float depending on the given direction
             */
            public float ForDirection(Direction dir)
            {
                switch (dir)
                {
                    case Direction.Up:
                    case Direction.Down:
                        return Vertical;
                    case Direction.Right:
                    case Direction.Left:
                        return Horizontal;
                    default:
                        throw new ArgumentException("Unknown direction");
                }
            }
        }

        private IEnumerator DoAbstractTrackpadInput(
            InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running,
            EmitDirectionStateChangeDelegate emitDirectionStateChange
            )
        {
            // Wait a tick before starting, a race condition results in the current position always starting as (0, 0)
            yield return null;

            var trackpadInterval = ActionsControllerBindingsLoader.CurrentBindingsController?.GetTrackpadSwipeInterval(hand);

            Vector2 anchorPos = position.Current;
            yield return null;
            while (running.current)
            {
                var pos = position.Current;
                var deltaPos = pos - anchorPos;
                float magnitude = 0;
                Direction dir = GetLargestVectorDirection(deltaPos, ref magnitude);
                if (magnitude >= trackpadInterval.ForDirection(dir))
                {
                    anchorPos = pos;
                    if (emitDirectionStateChange(dir, true))
                    {
                        // Wait long enough for ED to recieve any keypresses
                        yield return KeyboardInterface.WaitForKeySent();

                        emitDirectionStateChange(dir, false);
                    }
                }

                yield return null;
            }
        }
        #endregion

        #region Trackpad press
        private void MapTrackpadPressToDirectionAndButtonOptionAction(InputAction inputAction, OutputAction directionOutputAction, OutputAction centerOutputAction)
        {
            trackpadPressActionMapping[inputAction] = (directionOutputAction, centerOutputAction);
            trackpadPressActionHandlers[inputAction] = OnMappedTrackpadPress;
        }

        private void OnMappedTrackpadPress(InputAction inputAction, Hand hand, Vector2 position, bool newState)
        {
            if (!directionalTrackpadSlideMapping.ContainsKey(inputAction))
            {
                Debug.LogWarningFormat("No trackpad input action mapping for input action: {0}", inputAction.ToString());
                return;
            }
            var (directionOutputAction, centerOutputAction) = trackpadPressActionMapping[inputAction];

            DoAbstractTrackpadPress(inputAction, hand, position, newState,
                (dir, newButtonState) =>
                {
                    EmitDirectionActionStateChange(hand, directionOutputAction, dir, newButtonState);
                    return true;
                },
                (newButtonState) =>
                {
                    EmitActionStateChange(hand, centerOutputAction, newButtonState);
                    return true;
                });
        }

        private void OnUITabTrackpadPress(InputAction inputAction, Hand hand, Vector2 position, bool newState)
        {
            DoAbstractTrackpadPress(inputAction, hand, position, newState,
                (dir, newButtonState) =>
                {
                    switch (dir)
                    {
                        case Direction.Left:
                            EmitActionStateChange(hand, OutputAction.UITabPrevious, newButtonState);
                            return true;
                        case Direction.Right:
                            EmitActionStateChange(hand, OutputAction.UITabNext, newButtonState);
                            return true;
                        default:
                            return false;
                    }
                },
                (newButtonState) =>
                {
                    EmitActionStateChange(hand, OutputAction.UISelect, newButtonState);
                    return true;
                });
        }

        private void DoAbstractTrackpadPress(
            InputAction inputAction, Hand hand, Vector2 position, bool newState,
            EmitDirectionStateChangeDelegate emitDirectionStateChange, EmitStateChangeDelegate emitCenterStateChange
        )
        {
            // Release any previous press whether we have a newState=false unpress, or somehow got a second press without an unpress
            if (trackpadPressUnpressHandler.ContainsKey((inputAction, hand)))
            {
                trackpadPressUnpressHandler[(inputAction, hand)]();
            }

            if (newState)
            {
                float magnitude = 0;
                Direction dir = GetLargestVectorDirection(position, ref magnitude);

                if (magnitude > trackpadCenterButtonRadius)
                {
                    // Directional button press
                    if (emitDirectionStateChange(dir, true))
                    {
                        trackpadPressUnpressHandler[(inputAction, hand)] = () =>
                        {
                            trackpadPressUnpressHandler.Remove((inputAction, hand));
                            emitDirectionStateChange(dir, false);
                        };
                    }
                }
                else
                {
                    // Center button press
                    if (emitCenterStateChange(true))
                    {
                        trackpadPressUnpressHandler[(inputAction, hand)] = () =>
                        {
                            trackpadPressUnpressHandler.Remove((inputAction, hand));
                            emitCenterStateChange(false);
                        };
                    }
                }
            }
        }
        #endregion

        #region Joystick
        private void MapJoystickToDirectionOutputAction(InputAction inputAction, OutputAction outputAction)
        {
            joystickActionHandlers[inputAction] = OnMappedJoystickAxisChange;
            joystickDirectionActionMapping[inputAction] = outputAction;
        }

        private void OnMappedJoystickAxisChange(InputAction inputAction, Hand hand, Vector2 axis)
        {
            if (!joystickDirectionActionMapping.ContainsKey(inputAction))
            {
                Debug.LogWarningFormat("No joystick input action mapping for input action: {0}", inputAction.ToString());
                return;
            }
            var outputAction = joystickDirectionActionMapping[inputAction];

            DoAbstractJoystickInput(
                inputAction, hand, axis,
                (dir, newButtonState) =>
                {
                    EmitDirectionActionStateChange(hand, outputAction, dir, newButtonState);
                    return true;
                });
        }

        private void OnUITabJoystickAxisChange(InputAction inputAction, Hand hand, Vector2 axis)
        {
            DoAbstractJoystickInput(
                inputAction, hand, axis,
                (dir, newButtonState) =>
                {
                    switch (dir)
                    {
                        case Direction.Left:
                            EmitActionStateChange(hand, OutputAction.UITabPrevious, newButtonState);
                            return true;
                        case Direction.Right:
                            EmitActionStateChange(hand, OutputAction.UITabNext, newButtonState);
                            return true;
                        default:
                            return false;
                    }
                });
        }

        private void DoAbstractJoystickInput(
            InputAction inputAction, Hand hand, Vector2 axis,
            EmitDirectionStateChangeDelegate emitDirectionStateChange
        )
        {
            bool isReleased = Mathf.Abs(axis.x) < float.Epsilon && Mathf.Abs(axis.y) < float.Epsilon;

            Direction? dir = null;
            if (!isReleased)
            {
                float _magnitude = 0;
                dir = GetLargestVectorDirection(axis, ref _magnitude);
            }

            // Release any previous press whether we have an axis=0,0 release, or rotated to a different direction
            if (joystickDirectionUnpressHandler.ContainsKey((inputAction, hand)))
            {
                (Direction prevDirection, Action unpressHandler) = joystickDirectionUnpressHandler[(inputAction, hand)];
                if (dir == null || dir != prevDirection)
                {
                    unpressHandler();
                }
            }

            if (dir != null)
            {
                if (emitDirectionStateChange(dir.Value, true))
                {
                    joystickDirectionUnpressHandler[(inputAction, hand)] = (dir.Value, () =>
                    {
                        joystickDirectionUnpressHandler.Remove((inputAction, hand));
                        emitDirectionStateChange(dir.Value, false);
                    }
                    );
                }
            }
        }
        #endregion

        #region Reset Seated Position
        private readonly HashSet<Hand> maybeResetSeatedPositionHandPressed = new HashSet<Hand>();
        private bool maybeResetSeatedPositionBothHandsPressed = false;

        private void OnMaybeResetSeatedPosition(InputAction inputAction, Hand hand, bool newState)
        {
            if (newState) { maybeResetSeatedPositionHandPressed.Add(hand); }
            else { maybeResetSeatedPositionHandPressed.Remove(hand); }
            bool bothPressed = maybeResetSeatedPositionHandPressed.Contains(Hand.Left) && maybeResetSeatedPositionHandPressed.Contains(Hand.Right);
            if (maybeResetSeatedPositionBothHandsPressed != bothPressed)
            {
                EmitActionStateChange(Hand.Unknown, OutputAction.ResetSeatedPosition, bothPressed);
                maybeResetSeatedPositionBothHandsPressed = bothPressed;
            }
        }
        #endregion

        private Direction GetLargestVectorDirection(Vector2 v, ref float magnitude)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            {
                if (v.x < 0f)
                {
                    magnitude = -v.x;
                    return Direction.Left;
                }
                else
                {
                    magnitude = v.x;
                    return Direction.Right;
                }
            }
            else
            {
                if (v.y < 0f)
                {
                    magnitude = -v.y;
                    return Direction.Down;
                }
                else
                {
                    magnitude = v.y;
                    return Direction.Up;
                }
            }
        }
    }
}
