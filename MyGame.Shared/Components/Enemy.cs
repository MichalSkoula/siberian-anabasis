﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Screens;
using static MyGame.Enums;

namespace MyGame.Components
{
    public class Enemy : Component
    {
        private int speed = 200;

        private Animation anim;

        private List<Bullet> bullets = new List<Bullet>();

        private List<Animation> animations = new List<Animation>()
        {
            new Animation(Assets.PlayerUp, 4, 10),
            new Animation(Assets.PlayerRight, 4, 10),
            new Animation(Assets.PlayerDown, 4, 10),
            new Animation(Assets.PlayerLeft, 4, 10),
        };

        public bool ToDelete { get; set; }

        public Enemy(int x, int y, Direction direction, int health = 100, int caliber = 30)
        {
            this.anim = this.animations[(int)Direction.Down];
            this.Hitbox = new Rectangle(x, y, this.anim.FrameWidth, this.anim.FrameHeight);
            this.Direction = direction;
            this.Health = health;
            this.Caliber = caliber;
        }

        public override void Update(float deltaTime)
        {
            // is enemy moving?
            bool isMoving = true;
            Rectangle newHitbox = this.Hitbox;
            if (this.Direction == Direction.Right)
            {
                newHitbox.X += (int)(deltaTime * this.speed);
            }
            else if (this.Direction == Direction.Left)
            {
                newHitbox.X -= (int)(deltaTime * this.speed);
            }
            else
            {
                isMoving = false;
            }

            if (isMoving)
            {
                this.Hitbox = newHitbox;
                this.anim.Loop = true;
                this.anim = this.animations[(int)this.Direction];
            }
            else
            {
                this.anim.Loop = false;
                this.anim.ResetLoop();
            }

            this.anim.Update(deltaTime);

            // out of game map
            if (this.Hitbox.X < 0 || this.Hitbox.X > VillageScreen.MapWidth)
            {
                this.ToDelete = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.anim.Draw(spriteBatch, this.Hitbox, Color.Red);
            this.DrawHealth(spriteBatch);

            // bullets
            foreach (var bullet in this.bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
