using System.Collections.Generic;
using System.Linq;
using Poena.Core.Common;
using Poena.Core.Input.Actions;
using Poena.Core.Input.Interface;

namespace Poena.Core.Input
{

    //Handlers the capturing of the input and mapping it to the give maps
    public class InputHandler : ISaveable
    {

        private List<InputAction> current_actions;

        private List<IInputGather> inputGathers;

        public InputHandler()
        {
            this.inputGathers = new List<IInputGather>();
            this.current_actions = new List<InputAction>();
        }

        /// <summary>
        /// Adds a gatherer to the list of input gathering done per frame
        /// </summary>
        /// <param name="gatherer"></param>
        public void AddGather(IInputGather gatherer) 
        {
            this.inputGathers.Add(gatherer);
        }

        /// <summary>
        /// Update this frames input, clear out past frames
        /// </summary>
        public void Update()
        {
            // Clear the previous frames actions
            this.current_actions.Clear();

            // Loop gathers and add
            foreach(IInputGather gatherer in inputGathers) {
                List<InputAction> results = gatherer.Gather();
                this.current_actions.AddRange(results);
            }
        }

        /// <summary>
        /// Take the screens mapping to raw input and return the available commands
        /// </summary>
        /// <param name="mappings">The mappings for the scene</param>
        /// <returns>
        /// A list of mapped actions
        /// </returns>
        public List<MappedInputAction> GetMappedInputs(List<InputMapping> mappings)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return this.current_actions.Select(ca => new MappedInputAction(ca, ca.action_name)).ToList();
            }

            List<MappedInputAction> mappedActions = new List<MappedInputAction>();

            foreach(InputAction ia in this.current_actions)
            {
                InputMapping mapping = mappings.Find(m => m.raw_input.Equals(ia.action_name));
                string action_name = ia.action_name;

                // Check if we have a mapping
                if (mapping != null) 
                {
                    action_name = mapping.mapped_input;
                }

                // Otherwise just add with reference to itself
                mappedActions.Add(new MappedInputAction(ia, action_name));
            }

            return mappedActions;
        }

        /// <summary>
        /// Digest the action, no one else needs to look and see if it needs to be used
        /// </summary>
        /// <param name="action">The action to digest</param>
        public void DigestAction(InputAction action)
        {
            this.current_actions.Remove(action);
        }

        /// <summary>
        /// Digest the action, no one else needs to look and see if it needs to be used
        /// </summary>
        /// <param name="action">The mapped action to digest</param>
        public void DigestAction(MappedInputAction mappedAction)
        {
            this.DigestAction(mappedAction.raw_action);
        }

        public void Load(string path)
        {
            // TODO: rce = Load the mappings
        }

        public void Save(string path)
        {
            // TODO: rce - Save the mappings
        }
    }

}