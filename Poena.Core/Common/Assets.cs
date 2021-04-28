using Poena.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common
{
    public static class Assets
    {
        public static readonly string BASE_PATH = "assets";
        public static readonly string FONT_PATH = BASE_PATH + "/fonts/";
        public static readonly string TILE_PATH = BASE_PATH + "/tiles/";
        public static readonly string UI_PATH = BASE_PATH + "/ui/";
        public static readonly string ENTITY_PATH = BASE_PATH + "/entities/";

        #region fonts
        public static readonly string BASE_FONT = FONT_PATH + "BaseFont";
        #endregion

        #region entities
        public static string GetEntity(EntityType ent)
        {
            switch (ent)
            {
                case EntityType.Adventurer:
                    return ENTITY_PATH + "icon_adventurer1";
                case EntityType.GiantRat:
                    return ENTITY_PATH + "icon_giantRat";
            }

            return null;
        }
        #endregion

        #region board
        public static string GetTileHighlight(TileHighlight highlight)
        {
            switch (highlight)
            {
                case TileHighlight.Movement:
                    return TILE_PATH + "HEX_Highlight_Test";
                case TileHighlight.Attack:
                    return TILE_PATH + "HEX_Highlight_Attack";
            }

            return null;
        }
        #endregion

        #region UI
        public static string GetUIElement(UIElements ui)
        {
            switch (ui)
            {
                case UIElements.EmptyActionBar:
                    return UI_PATH + "EmptyBar";
                case UIElements.BlueActionBar:
                    return UI_PATH + "BlueBar";
                case UIElements.HotBar:
                    return UI_PATH + "ui_test_hotbar";
            }

            return null;
        }
        #endregion
    }
}
