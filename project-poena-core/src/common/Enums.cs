namespace Project_Poena.Common.Enums
{
    public enum Priority 
    {
        High = 0,
        Medium = 1,
        Low = 2
    }

    public enum StateEnum
    {
        Started,
        InProgress,
        Completed,
        Paused
    }

    public enum ScreenType
    {
        Play
        
    }

    public enum SceneType
    {
        Overworld,
        Battle
    }

    public enum ActionDeviceType
    {
        Keyboard,
        Mouse,
        Controller
    }

    public enum ActionType
    {
        Pressed,
        Held,
        Positional
    }

    public enum TileDirections
    {
        Top, //x--, y--
        TopRight, //y--
        BottomRight, //x++
        Bottom, //x++, y++
        BottomLeft, //y++
        TopLeft //x--
    }

    public enum PriorityEnum
    {
        Low,
        Medium,
        High
    }

    public enum BoardSize
    {
        Small = 2,
        Medium = 4,
        Large = 6
    }

    public enum EntityTypeEnum
    {
        //TODO: rce - Add entities
        Debug,
        DebugNPC
    }

    /// <summary>
    /// Allows logs to be prioritized
    /// </summary>
    public enum LogLevel 
    {
        Debug,
        Information,
        Critical
    }

}