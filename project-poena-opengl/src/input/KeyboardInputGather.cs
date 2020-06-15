
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Project_Poena.Common.Enums;
using Project_Poena.Input;

namespace Project_Poena.OpenGL.Input {

    public class KeyboardInputGather : IInputGather
    {
        /// <summary>
        /// Hold a linked list of the past keyboard states to determine if key is being held
        /// </summary>
        private LinkedList<KeyboardState> pastKeyboards;

        /// <summary>
        /// How many frames counts a held key
        /// </summary>
        private const int _keyboard_held_frames = 5;

        public KeyboardInputGather() 
        {
            pastKeyboards = new LinkedList<KeyboardState>();
        }

        /// <summary>
        /// Gather all the key actions and return a list of input actions
        /// </summary>
        /// <returns>
        /// A list of down or held keys
        /// </returns>
        public List<InputAction> Gather()
        {
            List<InputAction> results = new List<InputAction>();
            KeyboardState keyboardState = Keyboard.GetState();

            foreach(Keys k in keyboardState.GetPressedKeys()) 
            {
                // If the first keyboard in the queue was up it is a pressed
                if (pastKeyboards.Last.Value.IsKeyUp(k)) {
                    results.Add(new InputAction(ActionDeviceType.Keyboard, ActionType.Pressed, k.ToString()));
                } 
                // There was at least one prior frame of the key being down
                else {
                    // Check if the key was pressed for enough frames to be held
                    int count = pastKeyboards.Where(ks => ks.IsKeyDown(k)).Count();
                    if(count == _keyboard_held_frames) {
                        results.Add(new InputAction(ActionDeviceType.Keyboard, ActionType.Held, k.ToString()));
                    }
                }
            }

            if (pastKeyboards.Count >= _keyboard_held_frames) { 
                pastKeyboards.RemoveFirst();
            }

            pastKeyboards.AddLast(keyboardState);

            return results;
        }
    }

}