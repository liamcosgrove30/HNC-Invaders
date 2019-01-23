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
using System.IO;

namespace Invaders
{
    public class EnterHighScoreScreen : Screen
    {
        ////////////////////////////////////////////////////////////////////////////////
        // DECLARATIONS
        ////////////////////////////////////////////////////////////////////////////////

        string currentName = "";

        ////////////////////////////////////////////////////////////////////////////////
        // INITIALIZE THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Initialize(ScreenManager mgr)
        {
            base.Initialize(mgr);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // UPDATE THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardHelper.UpdateStringInput(ref currentName, gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                addNameToHighScores();
                manager.GoToScreen("viewHighScore");
                string outputFile = "highscores.txt";
                StreamWriter outputFilestream = new StreamWriter(outputFile);
                for (int counter = 0; counter < 10; counter++)
                {
                    outputFilestream.WriteLine(sharedData.highscoreNames[counter]);
                    outputFilestream.WriteLine(sharedData.highscores[counter]);
                }
                outputFilestream.Close();
            }
        }

        public void addNameToHighScores()
        {
            string nameToMoveDown = "";
            int scoreToMoveDown = 0;
            string newname = currentName;
            int newscore = sharedData.score;

            int place = -1;  // -1 represents not getting on the high score table.
            for (int loop = sharedData.highscores.Count() - 1; loop >= 0; loop--)
                if (sharedData.score > sharedData.highscores[loop]) place = loop;
            if (place == -1) return;    // not on high score table, do not add.

            for (int position = place; position < sharedData.highscores.Count(); position++)
            {
                nameToMoveDown = sharedData.highscoreNames[position];
                scoreToMoveDown = sharedData.highscores[position];
                sharedData.highscoreNames[position] = newname;
                sharedData.highscores[position] = newscore;
                newname = nameToMoveDown;
                newscore = scoreToMoveDown;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawText.Aligned("HIGH SCORE", HorizontalAlignment.Center, VerticalAlignment.Center, 0.5f, 0.5f, Color.White, manager.defaultFont);
            DrawText.Aligned("enter your name", HorizontalAlignment.Center, VerticalAlignment.Center, 0.5f, 0.65f, Color.White, manager.defaultFont);
            DrawText.Aligned(currentName, HorizontalAlignment.Center, VerticalAlignment.Center, 0.5f, 0.8f, Color.White, manager.defaultFont);
            base.Draw(gameTime);
        }
    }
}
