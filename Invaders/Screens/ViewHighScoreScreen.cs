using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using GameLibrary;
using UIControls;

namespace Invaders
{
    public class ViewHighScoreScreen : Screen
    {
        ////////////////////////////////////////////////////////////////////////////////
        // DECLARATIONS
        ////////////////////////////////////////////////////////////////////////////////
            string[] displayStrings;

        ////////////////////////////////////////////////////////////////////////////////
        // INITIALIZE THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Initialize(ScreenManager mgr)
        {
            base.Initialize(mgr);
        }

        public override void Start()
        {
            displayStrings = new string[sharedData.highscores.Count() + 4];
            displayStrings[0] = "HIGH SCORES";
            displayStrings[1] = "";
            for (int i = 0; i < sharedData.highscores.Count(); i++)
                displayStrings[i + 2] = "[" + (i + 1) + "}. " + sharedData.highscoreNames[i] + ": " + sharedData.highscores[i];
            displayStrings[displayStrings.Count() - 2] = "";
            displayStrings[displayStrings.Count() - 1] = "p to continue";
        }

        ////////////////////////////////////////////////////////////////////////////////
        // UPDATE THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (KeyboardHelper.NewKeyPressed(Keys.P, gameTime))
                manager.GoToScreen("menu");
        }

        ////////////////////////////////////////////////////////////////////////////////
        // DRAW THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Draw(GameTime gameTime)
        {
            DrawText.Aligned(displayStrings[0], HorizontalAlignment.Center, VerticalAlignment.Center,
                0.5f, (1f / (float)(displayStrings.Count()+2)) * 2,
                Color.White, manager.defaultFont);
            for (int i = 1; i < displayStrings.Count(); i++)
                DrawText.Aligned(displayStrings[i], HorizontalAlignment.Left, VerticalAlignment.Center, 
                    0.1f, 1f / ((float)displayStrings.Count()+2) * (i+2), 
                    Color.White, manager.defaultFont);

            base.Draw(gameTime);
        }
    }
}
