using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            SetUIPositions();
        }

        public override void WindowResizeEvent()
        {
            base.WindowResizeEvent();
            SetUIPositions();
        }

        private void SetUIPositions()
        {
            if (Loaded)
            {
                HotBar hotbar = this.GetComponent<HotBar>();
                hotbar.SetPosition(new Vector2((Camera.width / 2) - (hotbar.Width / 2), Camera.height - (hotbar.Height + 15)));
            }
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

