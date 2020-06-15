using Project_Poena.Common.Interfaces;
using System.Collections.Generic;

namespace Project_Poena.Input
{
    public class MappedInputAction : IRemovable
    {
        public InputAction raw_action { get; private set; }

        //Action we are mapped to, otherwise give raw input
        private string _mapped_action;
        public string mapped_action
        {
            get
            {
                if (this._mapped_action == null) return this.raw_action.action_name;
                else return _mapped_action;
            }

            set
            {
                this._mapped_action = value.ToLower();
            }
        }

        private bool input_handled { get; set; }

        public MappedInputAction(InputAction raw_action, string mapped_action)
        {
            this.raw_action = raw_action;
            this.mapped_action = mapped_action.ToLower();
            this.input_handled = false;
        }

        public void SetHandled()
        {
            this.input_handled = true;
        }

        public bool IsFlagged()
        {
            return this.input_handled;
        }
    }

}