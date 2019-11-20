using Project_Poena.Scene.SceneLayer;
using Project_Poena.Common.Enums;
using Project_Poena.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Scene
{
    public class BattleScene : AbstractScene
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

        public BattleScene()
        {
            //Create the scene adding the necessary layers
            this.AddLayer(new BattleSceneLayer());
            this.AddLayer(new BattleSceneUI());
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
