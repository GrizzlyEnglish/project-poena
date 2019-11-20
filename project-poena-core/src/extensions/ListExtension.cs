using Project_Poena.Common.Interfaces;
using System.Collections.Generic;

namespace Project_Poena.Extensions
{
    public static class ListExtension
    {
        //Removes flagged its from list returning the removed items
        public static List<T> RemoveHandled<T>(this List<T> actions) where T : IRemovable
        {
            List<T> flagged = actions.FindAll(action => action.IsFlagged());
            actions.RemoveAll(action => flagged.Contains(action));
            return flagged;
        }
    }
}
