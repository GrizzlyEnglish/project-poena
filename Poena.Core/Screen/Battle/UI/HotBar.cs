using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Components;
using Poena.Core.Screen.UI;
using Poena.Core.Sprites;

namespace Poena.Core.Screen.Battle.UI
{
    
    public class HotBar : UIComponent
    {
        private readonly int ICON_LENGTH = 5;
        
        private Sprite[] Icons;

        private Vector2 VisiblePosition;
        
        private readonly Vector2 PanDistance = new Vector2(0, 150);

        public HotBar(UISceneLayer layer) : base(layer)
        {
            this.ForegroundSprite.SetTexturePath(Assets.GetUIElement(UIElements.HotBar));
            this.ForegroundSprite.Position.speed = 3.25f;

            this.ClearIcons();

            //Start in hidden state
            this.Hide();
        }
        
        private void SetIconPositions()
        {
            //Go to far left of hot bar and center height
            RectangleF dimensions = this.ForegroundSprite.Dimensions;
            Vector2 start_pos = new Vector2(dimensions.Left + 7, dimensions.Top + 7);

            int distance = 100;

            for (int i = 0; i < ICON_LENGTH; i++)
            {
                if (this.Icons[i] != null) this.Icons[i].SetPosition(start_pos);
                start_pos.X += distance;
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
            this.SetIcon(skill.AttackType, skill.HotBarTexture);

            // Weapon skill icon

            // Items

            // Block

            // Stall
        }

        public void ClearIcons()
        {
            Icons = new Sprite[ICON_LENGTH];
        }

        private void SetIcon(AttackType iconType, Texture2D icon)
        {
            int position = (int)iconType;
            Icons[position] = new Sprite();
            Icons[position].SetTexture(icon);
            Icons[position].SetScale(.17f);
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
                // Show the hot bar
                this.GenerateIcons(ent);
                this.IsVisible = true;
                this.ForegroundSprite.Position.MoveDistance(-this.PanDistance);
            }
            else if (ent == null && this.IsVisible && !this.ForegroundSprite.Position.in_motion)
            {
                // Nobody is currently selected clear the icons and hide the hotbar
                this.ForegroundSprite.Position.MoveDistance(this.PanDistance, () => {
                    this.IsVisible = false;
                    this.ClearIcons();
                });
            }
            
            this.ForegroundSprite.Update(delta);
            this.SetIconPositions();

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
                        ECEntity selectedEntity = this.UISceneLayer.CurrentScene.GetSceneLayer<BattleEntityLayer>().EntityManager.GetEntity(typeof(SelectedComponent));
                        selectedEntity.AddComponent(new AttackingComponent
                        {
                            AttackType = (AttackType)i
                        });
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
