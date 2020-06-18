using System;
using Project_Poena.Common.Enums;

namespace Project_Poena.Input {

    /// <summary>
    /// Input watcher allows for contexts to set up
    /// a watcher that watches for an action type and
    /// action name, once met the watcher fires the action
    /// and marks the action has handled
    /// </summary>
    public class InputWatcher {

        /// <summary>
        /// The action name we are watching
        /// </summary>
        /// <value>Gets and Sets the aciton name</value>
        public string actionName {get; private set;}
        /// <summary>
        /// The actual action that happens when the paramaters are met
        /// </summary>
        /// <value>Gets and Sets the watcher action</value>
        public Action<MappedInputAction> watcherAction {get; private set;}
        /// <summary>
        /// The type of action we are watching
        /// </summary>
        /// <value>Gets and Sets the action type</value>
        public ActionType? actionType {get; set;}

        /// <summary>
        /// News a input watcher with only watcher on action name with any type
        /// </summary>
        /// <param name="actionName">The action name we are watching</param>
        /// <param name="watcherAction">The actual action that needs to take place</param>
        public InputWatcher(string actionName, Action<MappedInputAction> watcherAction) {
            this.actionName = actionName;
            this.watcherAction = watcherAction;
        }

        /// <summary>
        /// News a input watcher allows for full control of what to check
        /// </summary>
        /// <param name="actionName">The action name we are watching</param>
        /// <param name="actionType">The action type we are watching</param>
        /// <param name="watcherAction">The actual action that needs to take place</param>
        public InputWatcher(string actionName, ActionType actionType, Action<MappedInputAction> watcherAction) {
            this.actionName = actionName;
            this.actionType = actionType;
            this.watcherAction = watcherAction;
        }

    }

}