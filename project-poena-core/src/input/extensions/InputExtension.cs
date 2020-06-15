using Microsoft.Xna.Framework;
using Project_Poena.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Poena.Input.Extensions
{
    public static class InputExtension
    {
        public static List<MappedInputAction> GetAvailableActions(this List<MappedInputAction> actions, 
            List<string> watchingActions)
        {
            return actions.Where(a => watchingActions.Contains(a.mapped_action)).ToList();
        }

        public static MappedInputAction GetMousePosition(this List<MappedInputAction> actions)
        {
            return actions.FirstOrDefault(a => a.mapped_action == "mouse_position");
        }

        public static void UnprojectCoordinates(this List<MappedInputAction> actions, Func<Point, Vector2> unprojectFunc)
        {
            actions.ForEach(a =>
            {
                if (a.raw_action.position.HasValue)
                {
                    Vector2 coords = unprojectFunc(a.raw_action.position.Value);
                    a.raw_action.unprojected_position = coords;
                }
            });
        }

    }
}
