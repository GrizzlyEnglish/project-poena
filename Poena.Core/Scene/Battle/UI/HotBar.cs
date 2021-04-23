using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity;
using Poena.Core.Entity.Components;
using Poena.Core.Scene.Battle.Layers;
using Poena.Core.Scene.UI;
using Poena.Core.Sprites;

namespace Poena.Core.Scene.Battle.UI
{
    
    public class HotBar : UIComponent
    {
        internal enum HotBarItems
        {
            Basic = 0,
            Skill = 1,
            Weapon = 2,
            Block = 3,
            Stall = 4
        }

        private readonly int ICON_LENGTH = 5;
        
        private HotBarIcon[] Icons;

        private Vector2 VisiblePosition;
        
        private readonly Vector2 PanDistance = new Vector2(0, 150);

        public HotBar(UISceneLayer layer) : base(layer)
        {
            this.ForegroundTextureName.SetTexturePath(Variables.AssetPaths.UI_PATH + "/ui_test_hotbar");
            this.ForegroundTextureName.position.speed = 3.25f;

            this.ClearIcons();
            this.SetIconPositions();

            //Start in hidden state
            this.Hide();
        }
        
        private void SetIconPositions()
        {
            Vector2 half_height = new Vector2(0,this.BackgroundHeight / 2);

            //Go to far left of hot bar and center height
            Vector2 start_pos = this.ForegroundTextureName.position.position - 
                new Vector2(this.BackgroundWidth / 2, this.BackgroundHeight / 2) + 
                half_height;

            int distance = 100;

            for (int i = 0; i < ICON_LENGTH; i++)
            {
                start_pos.X += distance;
                if (this.Icons[i] != null) this.Icons[i].SetPosition(start_pos);
            }
        }

        public override void SetPosition(Vector2 position)
        {
            this.VisiblePosition = position;

            Vector2 pos = position;

            // Set the current position to below the threshold
            if (!IsVisible)
            {
                pos = this.VisiblePosition + this.PanDistance;
            }

            base.SetPosition(pos);
        }

        public void GenerateIcons(ECEntity ent)
        {
            // Skill icon
            SkillComponent skill = ent.GetComponent<SkillComponent>();
            this.SetIcon(HotBarItems.Skill, skill.skill_icon, skill);

            // Weapon skill icon

            // Items

            // Block

            // Stall

        }

        public void ClearIcons()
        {
            Icons = new HotBarIcon[ICON_LENGTH];
        }

        private void SetIcon(HotBarItems iconType, Texture2D icon, Component comp)
        {
            int position = (int)iconType;
            Icons[position] = new HotBarIcon(position, icon, comp);
            Icons[position].Show();
        }

        public override void RenderForeground(SpriteBatch spriteBatch)
        {
            //Regular render
            base.RenderForeground(spriteBatch);

            //Render icons
            foreach(HotBarIcon icon in this.Icons)
            {
                if (icon != null) icon.Render(spriteBatch);
            }
        }

        public override StateEnum Update(double delta)
        {
            //Check the event channel if someone new was selected
            ECEntity ent = this.UISceneLayer.CurrentScene.GetSceneLayer<BattleEntityLayer>().EntityManager.GetSelectedEntity();
            
            if (ent != null && !this.IsVisible)
            {
                //Show the hot bar
                this.GenerateIcons(ent);
                this.IsVisible = true;
                this.ForegroundTextureName.position.MoveDistance(-this.PanDistance);
            }
            else if (ent == null && this.IsVisible && !this.ForegroundTextureName.position.in_motion)
            {
                //Nobody is currently selected clear the icons and hide the hotbar
                this.ForegroundTextureName.position.MoveDistance(this.PanDistance, () => {
                    this.IsVisible = false;
                    this.ClearIcons();
                });
            }
            
            this.ForegroundTextureName.Update(delta);
            this.SetIconPositions();

            return StateEnum.InProgress;
        }

        public override bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            if (this.IsVisible)
            {
                //Check if the position is within the hot bar
                Point pos = mouseEvent.MouseEventArgs.Position;

                if (this.IsWithinBounds(pos))
                {
                    //Now determine the icon the mouse is over
                    for (int i = 0; i < ICON_LENGTH; i++)
                    {
                        HotBarIcon hbi = this.Icons[i];
                        if (hbi != null && hbi.IsWithinBounds(pos))
                        {
                            string message = "entity_" + ((HotBarItems)i).ToString().ToLower();
                            Component comp = hbi.component;
                            //Notify the entity that they an action is being performed
                            this.UISceneLayer.CurrentScene.GetSceneLayer<BattleEntityLayer>().SystemManager.Message(message, comp);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void HandleMouseMoved(MouseEvent mouseEvent)
        {
            throw new System.NotImplementedException();
        }

        /*
         * 
         * Hot bar icons are the specific icons for an action
         * The icons are all loaded via the layer and placed here
         * when an entity is selected
         * 
         */
        private class HotBarIcon : UIComponent
        {
            public Component component;

            public HotBarIcon(int bar_position, Texture2D icon, Component comp)
            {
                this.ForegroundTextureName = new Sprite();
                this.ForegroundTextureName.SetTexture(icon);
                this.ForegroundTextureName.SetScale(.18f);
                this.SetTextureDimensions(100, 100);
                this.component = comp;
            }

            public override void SetPosition(Vector2 icon_position)
            {
                this.ForegroundTextureName.position.SetPosition(icon_position);
            }

            public override bool HandleMouseClicked(MouseEvent mouseEvent)
            {
                throw new System.NotImplementedException();
            }

            public override void HandleMouseMoved(MouseEvent mouseEvent)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
