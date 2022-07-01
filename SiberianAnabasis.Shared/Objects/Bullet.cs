﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiberianAnabasis.Screens;
using static SiberianAnabasis.Enums;

namespace SiberianAnabasis.Components
{
    public class Bullet : Component
    {
        private int speed = 1000;

        public bool ToDelete { get; set; }

        public Bullet(int x, int y, Direction direction, int caliber)
        {
            this.Sprite = Assets.Bullet;
            this.Hitbox = new Rectangle(x, y - (this.Sprite.Height / 2), this.Sprite.Width, this.Sprite.Height);
            this.Direction = direction;
            this.Caliber = caliber;

            Assets.Blip.Play();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Sprite, this.Hitbox, Color.White);
        }

        public override void Update(float deltaTime)
        {
            // move it
            Rectangle newHitbox = this.Hitbox;
            if (this.Direction == Direction.Left)
            {
                newHitbox.X -= (int)(deltaTime * this.speed);
            }
            else if (this.Direction == Direction.Right)
            {
                newHitbox.X += (int)(deltaTime * this.speed);
            }

            this.Hitbox = newHitbox;

            // out of game map
            if (this.X < 0 || this.X > VillageScreen.MapWidth)
            {
                this.ToDelete = true;
            }
        }
    }
}