using Project_Poena.Common.Enums;

namespace Project_Poena.Common.Variables
{
    class Variables
    {

        public static int VIEWPORT_WIDTH = 960;
        public static int VIEWPORT_HEIGHT = 640;
        
        public static readonly int TILE_WIDTH = 216;
        public static readonly int TILE_HEIGHT = 216;

        public static PriorityEnum OUTPUT_PRIORITY = PriorityEnum.High;

        public static class AssetPaths
        {
            public static string BASE_PATH = "assets";
            public static string FONT_PATH = BASE_PATH + "/fonts/";
            public static string TILE_PATH = BASE_PATH + "/tiles/";
            public static string UI_PATH = BASE_PATH + "/ui/";
            public static string ENTITY_PATH = BASE_PATH + "/entities/";
        }

    }
}
