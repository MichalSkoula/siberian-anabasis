﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiberianAnabasis.Screens;
using SiberianAnabasis.Shared;
using static SiberianAnabasis.Enums;

namespace SiberianAnabasis.Objects
{
    public class Soldier : BasePerson
    {
        private List<Bullet> bullets = new List<Bullet>();
        private int centerRadius = 96;
        private bool isFast = true;

        public int? DeploymentX { get; set; } = null;

        private List<Animation> animations = new List<Animation>()
        {
            new Animation(Assets.Images["SoldierRight"], 4, 10),
            new Animation(Assets.Images["SoldierRight"], 4, 10),
            new Animation(Assets.Images["SoldierLeft"], 4, 10),
            new Animation(Assets.Images["SoldierLeft"], 4, 10),
        };

        public Soldier(int x, int y, Direction direction, int health = 100, int caliber = 10)
        {
            this.Anim = this.animations[(int)Direction.Left];
            this.Hitbox = new Rectangle(x, y, this.Anim.FrameWidth, this.Anim.FrameHeight);
            this.Direction = direction;
            this.Health = health;
            this.Caliber = caliber;
            this.Speed = 100;

            this.particleBlood = new ParticleSource(
                new Vector2(this.X, this.Y),
                new Tuple<int, int>(this.Width / 2, this.Height / 2),
                Direction.Down,
                2,
                Assets.ParticleTextureRegions["Blood"]
            );
        }

        public new void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // particles
            this.particleBlood.Update(deltaTime, new Vector2(this.X, this.Y));

            if (this.Dead)
            {
                return;
            }

            // is soldier moving?
            bool isMoving = false;
            if (Tools.GetRandom(4) == 1 || this.isFast)
            {
                if (this.Direction == Direction.Right)
                {
                    this.X += (int)(deltaTime * this.Speed);
                    isMoving = true;
                }
                else if (this.Direction == Direction.Left)
                {
                    this.X -= (int)(deltaTime * this.Speed);
                    isMoving = true;
                }
            }

            if (isMoving)
            {
                this.Anim.Loop = true;
                this.Anim = this.animations[(int)this.Direction];
            }
            else
            {
                this.Anim.Loop = false;
                this.Anim.ResetLoop();
            }

            this.Anim.Update(deltaTime);

            this.isFast = true;

            if (this.DeploymentX == null)
            {
                // run towards town center
                if (this.X < VillageScreen.MapWidth / 2 - this.centerRadius)
                {
                    this.Direction = Direction.Right;
                }
                else if (this.X > VillageScreen.MapWidth / 2 + this.centerRadius)
                {
                    this.Direction = Direction.Left;
                }

                // when near the base, can be slow and randomly change direction
                if (this.X < VillageScreen.MapWidth / 2 + this.centerRadius && this.X > VillageScreen.MapWidth / 2 - this.centerRadius)
                {
                    this.isFast = false;
                    if (Tools.GetRandom(128) < 2)
                    {
                        this.ChangeDirection();
                    }
                }
            } 
            else
            {
                if (this.X < this.DeploymentX - this.centerRadius / 2)
                {
                    this.Direction = Direction.Right;
                }
                else if (this.X > this.DeploymentX + this.centerRadius / 2)
                {
                    this.Direction = Direction.Left;
                }

                // when near the base, can be slow and randomly change direction
                if (this.X < this.DeploymentX + this.centerRadius / 2 && this.X > this.DeploymentX - this.centerRadius / 2)
                {
                    this.isFast = false;
                    if (Tools.GetRandom(128) == 1)
                    {
                        this.ChangeDirection();
                    }
                }
            }
            

            /*
            // when near the base, can be slow and randomly change direction
            if (this.X < VillageScreen.MapWidth / 2 + this.centerRadius && this.X > VillageScreen.MapWidth / 2 - this.centerRadius)
            {
                this.isFast = false;
                if (Tools.GetRandom(128) < 2)
                {
                    this.ChangeDirection();
                }
            }
            */
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Anim.Draw(spriteBatch, this.Hitbox, this.FinalColor);
            this.DrawHealth(spriteBatch);

            // bullets
            foreach (var bullet in this.bullets)
            {
                bullet.Draw(spriteBatch);
            }

            // particles
            this.particleBlood.Draw(spriteBatch);
        }
    }
}
