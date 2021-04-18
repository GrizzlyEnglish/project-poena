using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Poena.Core.Input.Actions;

namespace Poena.Core.Input.Extensions
{
    public static class InputExtension
    {

        /// <summary>
        /// Filters out a full list of actions based on the watchers passed in
        /// Then allows for only necessary watchers to apply
        /// </summary>
        /// <param name="actions">The current list of wactions</param>
        /// <param name="watchers">The waters we are looking fo</param>
        /// <returns>
        /// A list of watchers that we want to activate
        /// </returns>
        public static void FireAvailableWatchers(this List<MappedInputAction> actions, List<InputWatcher> watchers)
        {
            foreach (InputWatcher watcher in watchers) 
            {
                // See if we have a mapped input that matches the parameters
                MappedInputAction mia = actions
                    .Where(a => 
                        a.mapped_action == watcher.actionName && 
                        (watcher.actionType == null || a.raw_action.action_type == watcher.actionType)
                    )
                    .FirstOrDefault();
                if (mia != null) {
                    // Fire the watcher
                    watcher.watcherAction.Invoke(mia);
                    // Notify the action was handled
                    mia.SetHandled();
                }
            }
        }

        public static MappedInputAction GetMousePosition(this List<MappedInputAction> actions)
        {
            return actions.FirstOrDefault(a => a.mapped_action == "pointer_position");
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
