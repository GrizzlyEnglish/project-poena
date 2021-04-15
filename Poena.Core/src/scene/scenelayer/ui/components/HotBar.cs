using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Events;
using Project_Poena.Common.Variables;
using Project_Poena.Common.Enums;
using Project_Poena.Extensions;
using Project_Poena.Sprites;
using Project_Poena.Cameras;
using Project_Poena.Input;
using Project_Poena.Input.Extensions;
using Project_Poena.Entity.Entities;
using Project_Poena.Entity.Components;

namespace Project_Poena.Scene.SceneLayer
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

        private readonly int icon_length = 5;
        
        private HotBarIcon[] icons;

        private Vector2 visible_position;
        
        private readonly Vector2 pan_distance = new Vector2(0, 150);

        public HotBar(UISceneLayer layer) : base(layer)
        {
            this.foreground_sprite.SetTexturePath(Variables.AssetPaths.UI_PATH + "/ui_test_hotbar");
            this.foreground_sprite.position.speed = 2.25f;

            this.ClearIcons();
            this.SetIconPositions();

            //Start in hidden state
            this.Hide();
        }
        
        private void SetIconPositions()
        {
            Vector2 half_height = new Vector2(0,this.background_height / 2);

            //Go to far left of hot bar and center height
            Vector2 start_pos = this.foreground_sprite.position.position - 
                new Vector2(this.background_width / 2, this.background_height / 2) + 
                half_height;

            int distance = 100;

            for (int i = 0; i < icon_length; i++)
            {
                start_pos.X += distance;
                if (this.icons[i] != null) this.icons[i].SetPosition(start_pos);
            }
        }

        public override void SetPosition(Vector2 position)
        {
            this.visible_position = position;

            //Set the current position to below the threshold
            base.SetPosition(this.visible_position + this.pan_distance);
        }

        public void GenerateIcons(ECEntity ent)
        {
            Texture2D[] icons = new Texture2D[this.icons.Length];

            //Basic attack icon

            //Skill icon
            SkillComponent skill = ent.GetComponent<SkillComponent>();
            this.SetIcon(HotBarItems.Skill, skill.skill_icon, skill);

            //Weapon skill icon

            //Block

            //Stall

        }

        public void ClearIcons()
        {
            icons = new HotBarIcon[icon_length];
        }

        private void SetIcon(HotBarItems iconType, Texture2D icon, Component comp)
        {
            int position = (int)iconType;
            icons[position] = new HotBarIcon(position, icon, comp);
            icons[position].Show();
        }

        public override void RenderForeground(SpriteBatch spriteBatch)
        {
            //Regular render
            base.RenderForeground(spriteBatch);

            //Render icons
            foreach(HotBarIcon icon in this.icons)
            {
                if (icon != null) icon.Render(spriteBatch);
            }
        }

        public new StateEnum Update(double delta)
        {
            //Check the event channel if someone new was selected
            Event evt = EventQueueHandler.GetInstance().GetEvent("entity", "selected");
            
            if (evt != null && !this.is_visible)
            {
                //Show the hot bar
                this.GenerateIcons((ECEntity)evt.data);
                this.is_visible = true;
                this.foreground_sprite.position.MoveDistance(-this.pan_distance);
            }
            else if (this.is_visible && !this.foreground_sprite.position.in_motion)
            {
                ECEntity ent = this.ui_scene_layer.current_scene.entity_manager.GetSelectedEntity();
                if (ent == null)
                {
                    //Nobody is currently selected clear the icons and hide the hotbar
                    this.foreground_sprite.position.MoveDistance(this.pan_distance, () => {
                        this.is_visible = false;
                        this.ClearIcons();
                    });
                }
                
            }
            
            this.foreground_sprite.Update(delta);
            this.SetIconPositions();

            return StateEnum.InProgress;
        }
        
        public override List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
        {
            if (this.is_visible)
            {
                MappedInputAction mouse = actions.GetMousePosition();

                if (mouse != null)
                {
                    //Check if the position is within the hot bar
                    Point? pos = mouse.raw_action.position;

                    if (this.IsWithinBounds(pos))
                    {
                        //Now determine the icon the mouse is over
                        for (int i = 0; i < icon_length; i++)
                        {
                            HotBarIcon hbi = this.icons[i];
                            if (hbi != null && hbi.IsWithinBounds(pos))
                            {
                                string message = "entity_" + ((HotBarItems)i).ToString().ToLower();
                                Component comp = hbi.component;
                                //Notify the entity that they an action is being performed
                                this.ui_scene_layer.current_scene.system_manager.Message(message, comp);
                                //This was selected break
                                break;
                            }
                        }
                    }
                }
            }
            return actions;
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
                this.foreground_sprite = new Sprite();
                this.foreground_sprite.SetTexture(icon);
                this.foreground_sprite.SetScale(.18f);
                this.SetTextureDimensions(100, 100);
                this.component = comp;
            }

            public override void SetPosition(Vector2 icon_position)
            {
                this.foreground_sprite.position.SetPosition(icon_position);
            }

            public override List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
            {
                return actions;
            }
        }
    }
}
