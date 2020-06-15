using Microsoft.Xna.Framework;
using Project_Poena.Common.Enums;

namespace Project_Poena.Input
{
    public class InputAction
    {

        public ActionType action_type { get; private set; }
        public ActionDeviceType device_type { get; private set; }
        public string action_name { get; private set; }

        public Point? position {get; private set;}
        public Vector2? unprojected_position { get; set; }
        private Point? previous_position {get; set;}
        public Vector2 distance { 
            get {
                if (previous_position == null) return new Vector2(0,0);
                else {
                    return position.Value.ToVector2() - previous_position.Value.ToVector2();
                }
            }
        }

        public InputAction(ActionDeviceType decvice_type, ActionType action_type, 
                string action_name, Point? position = null, Point? previous_position = null)
        {
            this.device_type = device_type;
            this.action_type = action_type;
            this.action_name = action_name;

            //Used for positional actions, ie mouse
            this.position = position;
            this.previous_position = previous_position;
        }

    }

}