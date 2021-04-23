using System;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common;
using Poena.Core.Scene.Battle.Layers;

namespace Poena.Core.Scene.Battle
{
    /*
     *   Battle scenes are when the player engages and enemy and begins the battle sequence
     *   
     *   Layers
     *      BoardLayer
     *      CharacterLayer
     *      UILayer
     * 
     */
    public class BattleScene : AbstractScene
    {
        public BattleScene(MouseListener mouseListener, TouchListener touchListener) : base(mouseListener, touchListener)
        {
            // Set the namespace
            this.SceneNamespace = "battle_scene";
            //Create the scene adding the necessary layers
            this.AddLayer(new BattleEntityLayer());
            this.AddLayer(new BattleUILayer());
        }

        public override StateEnum GetState()
        {
            return StateEnum.InProgress;
        }

        public override void Load(string path)
        {
            throw new NotImplementedException();
        }

        public override void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
