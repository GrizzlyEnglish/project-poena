using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project_Poena.Common.Enums;
using Project_Poena.Input;

namespace Project_Poena.OpenGL.Input 
{

    /// <summary>
    /// Gathers Mouse Input and generates a list of input actions
    /// </summary>
    public class MouseInputGather : IInputGather
    {

        /// <summary>
        /// Store the past amount of frames mice state for determing held state
        /// </summary>
        private LinkedList<MouseState> pastMice;

        /// <summary>
        /// How many frames before we signal a button as held as well as how many to store
        /// </summary>
        private const int _mouse_held_frames = 10;

        public MouseInputGather()
        {
            pastMice = new LinkedList<MouseState>();
        }

        /// <summary>
        /// Determines if the action is held, pressed or neither
        /// </summary>
        /// <param name="current">The current button state</param>
        /// <param name="past">The past button state</param>
        /// <param name="check">How to check prior frames for held state</param>
        /// <returns>
        /// The button state or null if released
        /// </returns>
        private ActionType? getMouseActionType(ButtonState current, ButtonState past, Func<MouseState, bool> check)
        {
            // Only check when we have pressed down the mouse
            if (past == ButtonState.Pressed) {
                // Check how many past frames adhere to the check
                int count = pastMice.Where(check).Count();
                // If the current state is still down for greater than the held frames send held
                if (current == ButtonState.Pressed && count > _mouse_held_frames) {
                    return ActionType.Held;
                }
                // Otherwise its a click if less than
                else if (current == ButtonState.Released && count <=_mouse_held_frames) {
                    // Button is pressed
                    return ActionType.Pressed;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates an InputAction if needed
        /// </summary>
        /// <param name="actionName">The type of action, ie left click or right click</param>
        /// <param name="current">The current state of the action</param>
        /// <param name="past">The previous frames state of the action</param>
        /// <param name="check">How to check other frames for count of pressed</param>
        /// <param name="position">The position of the mouse at the action point</param>
        /// <param name="prev_position">The previous position of the mouse (for distance)</param>
        /// <param name="heldName">Allows for changing the action name on a held state</param>
        /// <returns>
        /// An action if determined it was needed or null 
        /// </returns>
        private InputAction createInputAction(string actionName, ButtonState current, ButtonState past, 
            Func<MouseState, bool> check, Point position, Point? prev_position, string heldName = null)
        {
            ActionType? at = getMouseActionType(current, past, check);
            if (at != null)
            {
                if (at == ActionType.Held) {
                    actionName = heldName ?? actionName;
                }
                return new InputAction(ActionDeviceType.Mouse, at.Value, actionName, position, prev_position);
            }

            return null;
        }

        /// <summary>
        /// Gathers mousouse inputs that we are wathcing for like clicks and position
        /// </summary>
        /// <returns>
        /// A list of gathered mouse inputs 
        /// </returns>
        public List<InputAction> Gather()
        {
            List<InputAction> results = new List<InputAction>();

            MouseState mouseState = Mouse.GetState();

            // Setup position
            Point pos = mouseState.Position;
            Point? past_pos = pastMice.Last?.Value.Position ?? null;

            // Setup array for easy loop
            InputAction[] actions = new InputAction[3];

            // Left click
            actions[0] = createInputAction("left_button", mouseState.LeftButton, 
                    pastMice?.Last?.Value.LeftButton ?? ButtonState.Released, 
                    ms => ms.LeftButton == ButtonState.Pressed, pos, past_pos);

            // Right click
            actions[1] = createInputAction("right_button", mouseState.RightButton, 
                    pastMice?.Last?.Value.RightButton ?? ButtonState.Released, 
                    ms => ms.RightButton == ButtonState.Pressed, pos, past_pos);

            // Middle Click
            actions[2] = createInputAction("middle_button", mouseState.MiddleButton, 
                    pastMice?.Last?.Value.MiddleButton ?? ButtonState.Released, 
                    ms => ms.MiddleButton == ButtonState.Pressed, pos, past_pos);

            // Add available actions
            foreach(InputAction ia in actions)
            {
                if (ia != null)
                {
                    results.Add(ia);
                }
            }

            // Add Pure Mouse position
            results.Add(new InputAction(ActionDeviceType.Mouse, ActionType.Positional, "pointer_position", pos));

            // Add the scrool wheel if the past value was different
            if (mouseState.ScrollWheelValue != (pastMice?.Last?.Value.ScrollWheelValue ?? 0) ||
                mouseState.HorizontalScrollWheelValue != (pastMice?.Last?.Value.HorizontalScrollWheelValue ?? 0))
            {
                Point current_scroll = new Point(mouseState.HorizontalScrollWheelValue, mouseState.ScrollWheelValue);
                Point past_scroll = 
                    new Point(this.pastMice.Last.Value.HorizontalScrollWheelValue, pastMice.Last.Value.ScrollWheelValue);

                InputAction scroll_ia = 
                    new InputAction(ActionDeviceType.Mouse, ActionType.Positional, "scroll", current_scroll, past_scroll);

                results.Add(scroll_ia);
            }

            if (pastMice.Count() > _mouse_held_frames) {
                pastMice.RemoveFirst();
            }

            pastMice.AddLast(mouseState);

            return results;
        }
    }

}