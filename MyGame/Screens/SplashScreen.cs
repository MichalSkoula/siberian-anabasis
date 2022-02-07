﻿namespace MyGame.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using MonoGame.Extended.Screens;
    using MyGame.Controls;

    public class SplashScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private double timer = 2;

        public SplashScreen(Game1 game) : base(game) { }

        public override void Initialize()
        {
            // load and apply settings from json file
            // we load this also in menuscreen
            Settings.LoadSettings(Game);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Controls.Keyboard.HasBeenPressed(Keys.Escape) || Controls.Keyboard.HasBeenPressed(Keys.Enter) || Controls.Mouse.HasBeenPressed())
            {
                // skip splash screen
                this.Game.LoadScreen(typeof(Screens.MenuScreen));
            }

            timer -= Game.DeltaTime;
            if (timer < 0)
            {
                this.Game.LoadScreen(typeof(Screens.MenuScreen));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.Game.Matrix = null;
            this.Game.DrawStart();

            this.Game.SpriteBatch.DrawString(Assets.FontMedium, "MyGame splash screen", new Vector2(10, 140), Color.White);

            this.Game.DrawEnd();
        }
    }
}
