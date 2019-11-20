using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project_Poena.Input;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Enums;

namespace Project_Poena.Managers
{

    //Handlers the capturing of the input and mapping it to the give maps
    public class InputHandler : ISaveable
    {

        //Determines how many frames of down there must be before swapping to held state
        private const int _keyboard_held_frames = 10;
        private const int _mouse_held_frames = 15;

        private List<InputAction> current_actions;

        private LinkedList<KeyboardState> past_keyboards;
        private LinkedList<MouseState> past_mice;

        public InputHandler()
        {
            this.current_actions = new List<InputAction>();
            this.past_keyboards = new LinkedList<KeyboardState>();
            this.past_mice = new LinkedList<MouseState>();
        }

        public void Update()
        {
            //Clear the previous frames actions
            this.current_actions.Clear();

            //Gather input based on current devices used
            //TODO: rce - Fixed this to check available devices before gatehr input
            this.GatherKeyboardInput();
            this.GatherMouseInput();
        }

        private void GatherControllerInput()
        {
            // TODO: rce - Determine how to get raw input of controller
        }

        //Here we gather the mouse raw input
        private void GatherMouseInput()
        {
            MouseState mouseState = Mouse.GetState();

            // Anon mouse state checker
            ActionType? getMouseActionType(ButtonState current, ButtonState past, Func<MouseState, bool> check)
            {
                if (current == ButtonState.Pressed) 
                {
                    if (past == ButtonState.Released) 
                    {
                        //Button is pressed
                        return ActionType.Pressed;
                    }

                    int count = 0;
                    foreach(MouseState ms in past_mice)
                    {
                        if (check(ms)) count++;
                        else break;
                    }

                    if (count == _mouse_held_frames) return ActionType.Held;
                }

                return null;
            }

            //Anony to check if action type is null and create the input action
            InputAction createInputAction(string actionName, ButtonState current, ButtonState past, 
                Func<MouseState, bool> check, Point position, Point? prev_position)
            {
                ActionType? at = getMouseActionType(current, past, check);
                if (at != null)
                {
                    return new InputAction(ActionDeviceType.Mouse, at.Value, actionName, position, prev_position);
                }

                return null;
            }

            //Setup position
            Point pos = mouseState.Position;
            Point? past_pos = past_mice.Last?.Value.Position ?? null;

            //Setup array for easy loop
            InputAction[] actions = new InputAction[3];

            //Left click
            actions[0] = createInputAction("left_mouse_button", mouseState.LeftButton, 
                    past_mice?.Last?.Value.LeftButton ?? ButtonState.Released, 
                    ms => ms.LeftButton == ButtonState.Pressed, pos, past_pos);
            
            //Right click
            actions[1] = createInputAction("right_mouse_button", mouseState.RightButton, 
                    past_mice?.Last?.Value.RightButton ?? ButtonState.Released, 
                    ms => ms.RightButton == ButtonState.Pressed, pos, past_pos);

            //Middle Click
            actions[2] = createInputAction("middle_mouse_button", mouseState.MiddleButton, 
                    past_mice?.Last?.Value.MiddleButton ?? ButtonState.Released, 
                    ms => ms.MiddleButton == ButtonState.Pressed, pos, past_pos);

            //Add available actions
            foreach(InputAction ia in actions)
            {
                if (ia != null)
                {
                    this.current_actions.Add(ia);
                }
            }

            //Add Pure Mouse position
            this.current_actions.Add(new InputAction(ActionDeviceType.Mouse, ActionType.Positional, "mouse_position", pos));

            //Add the scrool wheel if the past value was different
            if (mouseState.ScrollWheelValue != (this.past_mice?.Last?.Value.ScrollWheelValue ?? 0) ||
                mouseState.HorizontalScrollWheelValue != (this.past_mice?.Last?.Value.HorizontalScrollWheelValue ?? 0))
            {
                Point current_scroll = new Point(mouseState.HorizontalScrollWheelValue, mouseState.ScrollWheelValue);
                Point past_scroll = 
                    new Point(this.past_mice.Last.Value.HorizontalScrollWheelValue, this.past_mice.Last.Value.ScrollWheelValue);

                InputAction scroll_ia = 
                    new InputAction(ActionDeviceType.Mouse, ActionType.Positional, "mouse_scroll", current_scroll, past_scroll);

                this.current_actions.Add(scroll_ia);
            }

            if (this.past_mice.Count >= _mouse_held_frames) this.past_mice.RemoveFirst();
            this.past_mice.AddLast(mouseState);
        }

        //Here we need to get the raw input of the input type
        private void GatherKeyboardInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            foreach(Keys k in keyboardState.GetPressedKeys()) 
            {
                //If the first keyboard in the queue was up it is a pressed
                if (past_keyboards.Last.Value.IsKeyUp(k)) {
                    this.current_actions.Add(new InputAction(ActionDeviceType.Keyboard, ActionType.Pressed, k.ToString()));
                } 
                //There was at least one prior frame of the key being down
                else {
                    //Check if the key was pressed for enough frames to be held
                    int count = 0;
                    foreach(KeyboardState ks in past_keyboards)
                    {
                        if (ks.IsKeyDown(k)) {
                            count += 1;
                        } else {
                            //Up to the last time it was down
                            break;
                        }
                    }

                    if(count == _keyboard_held_frames) {
                        this.current_actions.Add(new InputAction(ActionDeviceType.Keyboard, ActionType.Held, k.ToString()));
                    }
                }
            }

            if (this.past_keyboards.Count >= _keyboard_held_frames) this.past_keyboards.RemoveFirst();
            this.past_keyboards.AddLast(keyboardState);
        }

        // Take the screens mapping to raw input and return the available commands
        public List<MappedInputAction> GetMappedInputs(List<InputMapping> mappings)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return this.current_actions.Select(ca => new MappedInputAction(ca, ca.action_name)).ToList();
            }

            List<MappedInputAction> mappedActions = 
                new List<MappedInputAction>();

            foreach(InputAction ia in this.current_actions)
            {
                InputMapping mapping = mappings.Find(m => m.raw_input.Equals(ia.action_name));
                string action_name = ia.action_name;

                //Check if we have a mapping
                if (mapping != null) 
                {
                    action_name = mapping.mapped_input;
                }

                //Otherwise just add with reference to itself
                mappedActions.Add(new MappedInputAction(ia, action_name));
            }

            return mappedActions;
        }

        //Only one screen is capable of using an action
        public void DigestAction(InputAction action)
        {
            this.current_actions.Remove(action);
        }

        public void DigestAction(MappedInputAction mappedAction)
        {
            this.DigestAction(mappedAction.raw_action);
        }

        public void Load(string path)
        {
            //If the path is empty load the default
        }

        public void Save(string path){}
    }

}