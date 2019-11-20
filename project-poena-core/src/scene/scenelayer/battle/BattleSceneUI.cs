using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Enums;
using Project_Poena.Entity.Managers;
using Project_Poena.Events;
using Project_Poena.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Scene.SceneLayer
{
    public class BattleSceneUI : UISceneLayer
    {
        public override void Initialize()
        {
            //Add components
            this.AddComponent(new HotBar(this));
        }
        
        public override void Entry()
        {
            //Update component positions
            HotBar hotbar = this.GetComponent<HotBar>();
            hotbar.SetPosition(new Vector2((camera.width / 2), camera.height - (hotbar.background_height / 2) - 15));
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void HandleEvents()
        {
            throw new NotImplementedException();
        }
        
        public override void Load(string path)
        {
            throw new NotImplementedException();
        }

        public override void Save(string path)
        {
            throw new NotImplementedException();
        }

        public override StateEnum UpdateLayer(double delta)
        {
            //Update the components
            foreach(UIComponent comp in this.layer_components)
            {
                comp.Update(delta);
            }

            return StateEnum.InProgress;
        }

        public override List<MappedInputAction> HandleLayerInput(List<MappedInputAction> actions)
        {
            foreach(UIComponent comp in this.layer_components)
            {
                comp.HandleInput(actions);
            }

            return actions;
        }
    }
}
