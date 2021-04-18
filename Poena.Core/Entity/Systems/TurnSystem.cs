using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;

namespace Poena.Core.Entity.Systems
{
    public class TurnSystem : ECSystem
    {

        private Texture2D turn_bar_background;
        private Texture2D turn_bar_foreground;
        private Vector2 sprite_center;

        public TurnSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override void Initiliaze()
        {
            
        }

        public override bool RecieveMessage(string message, object data)
        {

            if (message == "end_turn")
            {
                ECEntity ent = (ECEntity)data;
                this.EndTurn(ent);
            }

            return false;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            this.turn_bar_background = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + "EmptyBar");
            this.turn_bar_foreground = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + "BlueBar");
            this.sprite_center = new Vector2(this.turn_bar_foreground.Width / 2, this.turn_bar_foreground.Height / 2);
        }

        public override void Update(double dt)
        {
            ECEntity selected_ent = this.manager.EntityManager.GetSelectedEntity();

            //TODO: rce - Add pause mechanism
            if (selected_ent != null)
            {
                //We have a selected entity don't update ticker for people
                return;
            }

            List<ECEntity> entities =
                this.manager.EntityManager.GetEntities(new Type[] { typeof(TurnComponent) });

            foreach (ECEntity ent in entities)
            {
                TurnComponent turn = ent.GetComponent<TurnComponent>();

                //TODO: rce - Do we need to add logic to not update?
                turn.current_time += dt;

                //There turn is available
                if (turn.current_time >= turn.time_for_turn)
                {
                    //Make sure this doesn't overflow
                    turn.current_time = turn.time_for_turn;
                    //Force select the entity
                    this.manager.Message("select_entity", ent);
                    break;
                }

            }

        }

        public override void Render(SpriteBatch batch, RectangleF camera_bounds)
        {
            List<ECEntity> entities =
                this.manager.EntityManager.GetEntities(new Type[] { typeof(TurnComponent) });

            //Loop anybody that has a turn comp and render there bar
            foreach (ECEntity ent in entities)
            {
                PositionComponent pos = ent.GetComponent<PositionComponent>();
                TurnComponent turn = ent.GetComponent<TurnComponent>();
                
                //Shift it up a bit
                Vector2 position = new Vector2((int)pos.tile_position.X, (int)pos.tile_position.Y - 100);

                //Draw the fully background
                batch.Draw(this.turn_bar_background, position, null, Color.White,
                    0, this.sprite_center, 0.5f, SpriteEffects.None, 0);

                //We need to calc the rectange to draw
                int width = (int)(this.turn_bar_foreground.Width * (turn.current_time / turn.time_for_turn));
                Rectangle clipping_rect = new Rectangle(0, 0, width, this.turn_bar_foreground.Height);
                Rectangle source_rect = new Rectangle(0, 0, (int)(this.turn_bar_foreground.Width * .5f), (int)(this.turn_bar_foreground.Height * .5f));
                
                //Draw the clipped foreground
                batch.Draw(this.turn_bar_foreground, position, clipping_rect, Color.White,
                    0, this.sprite_center, 0.5f, SpriteEffects.None, 0);
            }
        }

        private void EndTurn(ECEntity ent)
        {
            TurnComponent turn = ent.GetComponent<TurnComponent>();
            //Reset turn ticker
            turn.current_time = 0;
            ent.RemoveComponent(typeof(SelectedComponent));
        }

    }
}
