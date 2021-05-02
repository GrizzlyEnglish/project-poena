namespace Poena.Core.Screen.Battle.Components
{
    public class TurnComponent
    {
        public double current_time { get; set; }
        public double time_for_turn { get; set; }
        public bool ready_for_turn { get { return current_time == time_for_turn;  } }
    }
}
