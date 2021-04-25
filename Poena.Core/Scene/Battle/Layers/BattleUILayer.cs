using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Scene.Battle.UI;
using Poena.Core.Scene.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Scene.Battle.Layers
{
    public class BattleUILayer : UISceneLayer
    {
        public override void Initialize()
        {
            //Add components
            this.AddComponent(new HotBar(this));
        }

        public override void Entry()
        {
            // Update component positions
            HotBar hotbar = this.GetComponent<HotBar>();
            // TODO: Rce - Need a bottom anchor here so we can say the bottom of the screen is where to render
            hotbar.SetPosition(new Vector2((Camera.width / 2), Camera.height - 70));
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
            foreach (UIComponent comp in this.LayerComponents)
            {
                comp.Update(delta);
            }

            return StateEnum.InProgress;
        }

        public override bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            bool handled = false;
            foreach (UIComponent comp in this.LayerComponents)
            {
                handled = comp.HandleMouseClicked(mouseEvent);
                if (handled)
                {
                    return true;
                }
            }
            return false;
        }

        public override void HandleMouseMoved(MouseEvent mouseEvent)
        {
        }

        public override void HandleMouseDragged(MouseEvent mouseEvent)
        {
        }

        public override void HandleMouseWheeled(MouseEvent mouseEvent)
        {
        }
    }
}

