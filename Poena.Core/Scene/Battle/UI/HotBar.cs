using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Entity;
using Poena.Core.Entity.Components;
using Poena.Core.Scene.Battle.Components;
using Poena.Core.Scene.Battle.Layers;
using Poena.Core.Scene.UI;
using Poena.Core.Sprites;

namespace Poena.Core.Scene.Battle.UI
{
    
    public class HotBar : UIComponent
    {
        private readonly int ICON_LENGTH = 5;
        
        private Sprite[] Icons;

        private Vector2 VisiblePosition;
        
        private readonly Vector2 PanDistance = new Vector2(0, 150);

        public HotBar(UISceneLayer layer) : base(layer)
        {
            this.ForegroundSprite.SetTexturePath(Variables.AssetPaths.UI_PATH + "/ui_test_hotbar");
            this.ForegroundSprite.Position.speed = 3.25f;

            this.ClearIcons();

            //Start in hidden state
            this.Hide();
        }
        
        private void SetIconPositions()
        {
            //Go to far left of hot bar and center height
            RectangleF dimensions = this.ForegroundSprite.Dimensions;
            Vector2 start_pos = new Vector2(dimensions.TopLeft.X, dimensions.TopLeft.Y);

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
            this.SetIcon(skill.AttackType, skill.HotBarTexture, skill);

            // Weapon skill icon

            // Items

            // Block

            // Stall

            this.SetIconPositions();
        }

        public void ClearIcons()
        {
            Icons = new Sprite[ICON_LENGTH];
        }

        private void SetIcon(AttackTypeEnum iconType, Texture2D icon, Component comp)
        {
            int position = (int)iconType;
            Icons[position] = new Sprite();
            Icons[position].SetTexture(icon);
            Icons[position].SetScale(.18f);
        }

        public override void RenderForeground(SpriteBatch spriteBatch)
        {
            //Regular render
            base.RenderForeground(spriteBatch);

            //Render icons
            foreach(Sprite icon in this.Icons)
            {
                if (icon != null) icon.Render(spriteBatch, default(RectangleF));
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
                this.ForegroundSprite.Position.MoveDistance(-this.PanDistance);
            }
            else if (ent == null && this.IsVisible && !this.ForegroundSprite.Position.in_motion)
            {
                //Nobody is currently selected clear the icons and hide the hotbar
                this.ForegroundSprite.Position.MoveDistance(this.PanDistance, () => {
                    this.IsVisible = false;
                    this.ClearIcons();
                });
            }
            
            this.ForegroundSprite.Update(delta);

            return StateEnum.InProgress;
        }

        public override bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            if (this.IsVisible)
            {
                //Check if the position is within the hot bar
                Point pos = mouseEvent.MouseEventArgs.Position;

                //Now determine the icon the mouse is over
                for (int i = 0; i < ICON_LENGTH; i++)
                {
                    if (Icons[i] != null && Icons[i].IsWithinBounds(pos))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void HandleMouseMoved(MouseEvent mouseEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
