﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using Nazdar.Controls;
using Nazdar.Shared;
using System.Collections.Generic;
using static Nazdar.Enums;

namespace Nazdar.Screens
{
    public class MenuScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        public MenuScreen(Game1 game) : base(game) { }

        private Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        public override void Initialize()
        {
            // add big start buttons
            buttons.Add("startButton1", new Button(Offset.MenuX, 55, null, ButtonSize.Large, "Slot #1", true));
            buttons.Add("startButton2", new Button(Offset.MenuX, 90, null, ButtonSize.Large, "Slot #2"));
            buttons.Add("startButton3", new Button(Offset.MenuX, 125, null, ButtonSize.Large, "Slot #3"));

            // add other buttons
            buttons.Add("controlsButton", new Button(Offset.MenuX, 160 + 0 * 27, null, ButtonSize.Medium, "Controls"));
            buttons.Add("creditsButton", new Button(Offset.MenuX, 160 + 1 * 27, null, ButtonSize.Medium, "Credits"));
            buttons.Add("musicButton", new Button(Offset.MenuX, 160 + 2 * 27, null, ButtonSize.Medium, "Music:", text: "Off"));
            buttons.Add("soundsButton", new Button(Offset.MenuX, 160 + 3 * 27, null, ButtonSize.Medium, "Sounds:", text: "Off"));
            buttons.Add("vibrationsButton", new Button(Offset.MenuX, 160 + 4 * 27, null, ButtonSize.Medium, "Vibrations:", text: "Off"));

            // fullscreen - only on desktop = GL
            if (Game1.CurrentPlatform == Platform.GL)
            {
                buttons.Add("fullscreenButton", new Button(Offset.MenuX, 160 + 5 * 27, null, ButtonSize.Medium, "Fullscreen:", text: "Off"));
            }
            buttons.Add("exitButton", new Button(Offset.MenuX, 160 + 6 * 27, null, ButtonSize.Medium, "Exit"));
#if DEBUG
            if (Game1.CurrentPlatform != Platform.Android)
            {
                buttons.Add("openFolderButton", new Button(Offset.MenuX + 500, Offset.MenuFooter, null, ButtonSize.Small, "saves"));
            }
#endif

            // load saves to show info next to slot button
            string[] slots = new[] { Save.Slot1, Save.Slot2, Save.Slot3 };
            for (int i = 0; i < 3; i++)
            {
                FileIO saveFile = new FileIO(slots[i]);
                dynamic saveData = saveFile.Load();
                this.buttons.GetValueOrDefault("startButton" + (i + 1)).Data = Tools.ParseSaveData(saveData);
            }

            Audio.SongTransition(0.25f, "Menu");

            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Button.UpdateButtons(this.buttons);

            // start game
            if (this.buttons.GetValueOrDefault("startButton1").HasBeenClicked())
            {
                this.Game.SaveSlot = Save.Slot1;
                this.Game.LoadScreen(typeof(Screens.MapScreen));
            }
            else if (this.buttons.GetValueOrDefault("startButton2").HasBeenClicked())
            {
                this.Game.SaveSlot = Save.Slot2;
                this.Game.LoadScreen(typeof(Screens.MapScreen));
            }
            else if (this.buttons.GetValueOrDefault("startButton3").HasBeenClicked())
            {
                this.Game.SaveSlot = Save.Slot3;
                this.Game.LoadScreen(typeof(Screens.MapScreen));
            }

            // settings - fullscreen
            if (this.buttons.ContainsKey("fullscreenButton"))
            {
                if (Controls.Keyboard.HasBeenPressed(Keys.F) || Controls.Keyboard.HasBeenPressed(Keys.F11) || this.buttons.GetValueOrDefault("fullscreenButton").HasBeenClicked())
                {
                    this.Game.Graphics.IsFullScreen = !this.Game.Graphics.IsFullScreen;
                    this.Game.Graphics.ApplyChanges();
                    Settings.SaveSettings(Game);
                }

                this.buttons.GetValueOrDefault("fullscreenButton").Text = this.Game.Graphics.IsFullScreen ? "On" : "Off";
            }

            // settings - vibrations
            if (this.buttons.ContainsKey("vibrationsButton"))
            {
                if (this.buttons.GetValueOrDefault("vibrationsButton").HasBeenClicked())
                {
                    Game1.Vibrations = !Game1.Vibrations;
                    Settings.SaveSettings(Game);
                }

                this.buttons.GetValueOrDefault("vibrationsButton").Text = Game1.Vibrations ? "On" : "Off";
            }

            // settings - music
            if (Controls.Keyboard.HasBeenPressed(Keys.M) || this.buttons.GetValueOrDefault("musicButton").HasBeenClicked())
            {
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                Settings.SaveSettings(Game);
            }
            this.buttons.GetValueOrDefault("musicButton").Text = MediaPlayer.IsMuted ? "Off" : "On";

            // settings - sounds
            if (Controls.Keyboard.HasBeenPressed(Keys.S) || this.buttons.GetValueOrDefault("soundsButton").HasBeenClicked())
            {
                if (SoundEffect.MasterVolume == 0)
                {
                    SoundEffect.MasterVolume = 1;
                }
                else
                {
                    SoundEffect.MasterVolume = 0;
                }
                Settings.SaveSettings(Game);
            }
            this.buttons.GetValueOrDefault("soundsButton").Text = SoundEffect.MasterVolume == 1 ? "On" : "Off";

            // exit game from menu
            if (Controls.Keyboard.HasBeenPressed(Keys.Escape) || this.buttons.GetValueOrDefault("exitButton").HasBeenClicked())
            {
                this.Game.Exit();
            }

            // back to splash screen
            if (this.buttons.GetValueOrDefault("controlsButton").HasBeenClicked())
            {
                this.Game.LoadScreen(typeof(Screens.ControlsScreen));
            }

            // credits
            if (this.buttons.GetValueOrDefault("creditsButton").HasBeenClicked())
            {
                this.Game.LoadScreen(typeof(Screens.CreditsScreen));
            }

            // open save folder (only if DEBUG)
            if (this.buttons.ContainsKey("openFolderButton") && this.buttons.GetValueOrDefault("openFolderButton").HasBeenClicked())
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = Settings.GetPath(),
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.Game.Matrix = null;
            this.Game.DrawStart();

            // title
            this.Game.SpriteBatch.DrawString(Assets.Fonts["Large"], "NAZDAR! The Game", new Vector2(Offset.MenuX, Offset.MenuY), MyColor.White);

            // buttons
            int i = 0;
            foreach (KeyValuePair<string, Button> button in this.buttons)
            {
                // draw button
                button.Value.Draw(this.Game.SpriteBatch);

                // draw save slot content next to button
                if (button.Value.Data != null && button.Value.Data.Length > 0)
                {
                    this.Game.SpriteBatch.DrawString(
                        Assets.Fonts["Small"],
                        button.Value.Data[0],
                        new Vector2(2 * Offset.MenuX + button.Value.Hitbox.Width, button.Value.Hitbox.Y),
                        MyColor.White);
                    this.Game.SpriteBatch.DrawString(
                        Assets.Fonts["Small"],
                        button.Value.Data[1],
                        new Vector2(2 * Offset.MenuX + button.Value.Hitbox.Width, button.Value.Hitbox.Y + 10),
                        MyColor.White);
                    this.Game.SpriteBatch.DrawString(
                        Assets.Fonts["Small"],
                        button.Value.Data[2],
                        new Vector2(2 * Offset.MenuX + button.Value.Hitbox.Width, button.Value.Hitbox.Y + 20),
                        MyColor.White);
                }
                i++;
            }

            // version
            this.Game.SpriteBatch.DrawString(Assets.Fonts["Small"], "v" + Version.Number, new Vector2(Offset.MenuX + 565, Offset.MenuFooter), MyColor.Gray1);
#if DEBUG
            // save path
            this.Game.SpriteBatch.DrawString(Assets.Fonts["Small"], Settings.GetPath(), new Vector2(Offset.MenuX, Offset.MenuFooter + 10), MyColor.Gray2);
#endif
            // messages
            Game1.MessageBuffer.Draw(Game.SpriteBatch);

            this.Game.DrawEnd();
        }
    }
}
