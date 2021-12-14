﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.Objects
{
    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public class Player : Component
    {
        private int _speed = 300;

        private Animation _anim;

        private List<Animation> _animations = new List<Animation>()
        {
            new Animation(Assets.playerUp, 3, 10),
            new Animation(Assets.playerRight, 3, 10),
            new Animation(Assets.playerDown, 3, 10),
            new Animation(Assets.playerLeft, 3, 10),
        };

        public Player(Texture2D sprite, int x, int y)
        {
            _anim = _animations[(int)Direction.Down];
            Hitbox = new Rectangle(x, y, _anim.FrameWidth, _anim.FrameHeight);
        }

        public override void Update(float deltaTime)
        {
            bool isMoving = true;
            Rectangle newHitbox = Hitbox;

            if (Controls.Keyboard.IsPressed(Keys.Up))
            {
                newHitbox.Y -= (int)(deltaTime * _speed);
                _anim = _animations[(int)Direction.Up];
            }
            else if (Controls.Keyboard.IsPressed(Keys.Right))
            {
                newHitbox.X += (int)(deltaTime * _speed);
                _anim = _animations[(int)Direction.Right];
            }
            else if (Controls.Keyboard.IsPressed(Keys.Down))
            {
                newHitbox.Y += (int)(deltaTime * _speed);
                _anim = _animations[(int)Direction.Down];
            }
            else if (Controls.Keyboard.IsPressed(Keys.Left))
            {
                newHitbox.X -= (int)(deltaTime * _speed);
                _anim = _animations[(int)Direction.Left];
            }
            else
            {
                isMoving = false;
            }

            if (isMoving)
            {
                Hitbox = newHitbox;
                _anim.Loop = true;
            }
            else 
            {
                _anim.Loop = false;
                _anim.ResetLoop();
            }
            
            _anim.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _anim.Draw(spriteBatch, Hitbox);
        }
    }
}
