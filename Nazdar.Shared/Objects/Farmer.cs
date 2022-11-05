﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nazdar.Screens;
using Nazdar.Shared;
using System;
using System.Collections.Generic;
using static Nazdar.Enums;

namespace Nazdar.Objects
{
    public class Farmer : BasePerson
    {
        private int centerRadius = 32;
        private bool isFast = true;

        public const int DefaultHealth = 100;
        public const int DefaultCaliber = 2;

        public bool IsFarming { get; set; } = false;

        public int? DeploymentX { get; set; } = null;

        private List<Animation> animations = new List<Animation>()
        {
            new Animation(Assets.Images["FarmerRight"], 4, 10),
            new Animation(Assets.Images["FarmerRight"], 4, 10),
            new Animation(Assets.Images["FarmerLeft"], 4, 10),
            new Animation(Assets.Images["FarmerLeft"], 4, 10),
        };

        public Farmer(int x, int y, Direction direction, int health = DefaultHealth, int caliber = DefaultCaliber)
        {
            this.Anim = this.animations[(int)Direction.Left];
            this.Hitbox = new Rectangle(x, y, this.Anim.FrameWidth, this.Anim.FrameHeight);
            this.Direction = direction;
            this.Health = health;
            this.Speed = 100;
            this.Caliber = caliber;
            this.Color = UniversalColors[Tools.GetRandom(UniversalColors.Length)];

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

            // is he moving?
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
            this.IsFarming = false;

            // go somewhere
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
                // is deployed somewhere
                if (this.X < this.DeploymentX - this.centerRadius / 2)
                {
                    this.Direction = Direction.Right;
                }
                else if (this.X > this.DeploymentX + this.centerRadius / 2)
                {
                    this.Direction = Direction.Left;
                }

                // when near the desired spot, can be slow and randomly change direction
                // and also farming
                if (this.X < this.DeploymentX + this.centerRadius / 2 && this.X > this.DeploymentX - this.centerRadius / 2)
                {
                    this.isFast = false;
                    this.IsFarming = true;
                    if (Tools.GetRandom(128) == 1)
                    {
                        this.ChangeDirection();
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Anim.Draw(spriteBatch, this.Hitbox, this.FinalColor);
            this.DrawHealth(spriteBatch);
            this.DrawCaliber(spriteBatch);

            // particles
            this.particleBlood.Draw(spriteBatch);
        }
    }
}