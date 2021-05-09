namespace Poena.Core.Screen.Battle.Components
{
    public class TurnComponent
    {
        public double CurrentTime { get; set; }
        public double TimeForTurn { get; set; }
        public bool TurnComplete { get; set; }

        public bool ReadyForTurn { 
            get 
            { 
                return CurrentTime == TimeForTurn;  
            } 
        }

        public void EndTurn()
        {
            CurrentTime = 0;
        }
    }
}
