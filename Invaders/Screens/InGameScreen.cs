///////////////////////////////////////////////////////////////////////////////
// PORTFOLIO ASSIGNMENT 3
//
// To complete this assignment:
//
// 1) Declare an array or list for enemies.  Then create the ResetAliens code.
// 2) Add a loop in the Draw method that will draw all the enemies.
// 3) Add a loop in the Update that will check for the player bullet hitting
//   3a) On a collision, remove the alien & bullet, and gain points.
// 4) Keep count of aliens killed.  When all are dead, reset the level.
// 5) Make a variable that keeps track of alien movement direction.
// 6) In Update, Loop through aliens and move them.
// 7) In Update, check if any alien has hit a side of the screen.
//   7a) if they have, reverse all aliens direction...
//   7b) ... and make ANOTHER loop that will move them all downwards a bit.
// 8) Use some method of determining when an alien will attack (eg random)
// 9) When an alien attacks, spawn an enemy bullet sprite
// 10) Make the enemy bullet move down the screen
// 11) Check for collision between player and enemy bullet.
//   11a) On collision, remove bullet, respawn player, and lose a life.
// 12) Check for collision between alien bullet and shields.
//   12a) On collision, remove bullet, and reduce health of that shield.
// 13) After entering name (in EnterHighScoreScreen.cs), save high score(s).
// 14) In Initialize, load high score(s).
// 15) Make sure you have created a method with parameters somewhere.
// 16) Add suitable comments to every non-trivial block of code you added
//
///////////////////////////////////////////////////////////////////////////////

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
    ///////////////////////////////////////////////////////////////////////////
    // SHARED DATA: PUT VARIABLES HERE TO BE USED ACROSS DIFFERENT SCREENS
    // Remember to make them 'public' and 'static'.
    ///////////////////////////////////////////////////////////////////////////
    public static class sharedData
    {
        public static bool soundsOn = true;
        public static bool musicOn = true;
        public static int score = 0;
        public static int[] highscores = new int[10]
            { 100, 90, 80, 70, 60, 50, 40, 30, 20, 10};  // default scores in case of missing score file
        public static string[] highscoreNames = new string[10]
            { "Zoltan", "Zarnoff", "Zabu", "Zellner", "Zelbor", "Zelmina", "Zefar", "Zamora", "Zihan", "Jeff" };
    }

    public class InGameScreen : Screen
    {
        ////////////////////////////////////////////////////////////////////////////////
        // DECLARATIONS
        ////////////////////////////////////////////////////////////////////////////////

        // content objects
        Texture2D playerImage, projectileImage, shieldImage, alienImage;
        SoundEffect shootSound;

        // gameplay objects
        GameSprite player, playerProjectile;
        GameSprite[] shields = new GameSprite[3];
        List<GameSprite> aliens = new List<GameSprite>();
        List<GameSprite> alienProjectile = new List<GameSprite>();
        int[] shieldHealth = new int[3];
        const float FRICTION_COEFFICIENT = 0.8f;
        const int MAXGUNCOOLDOWN = 800;

        int lives;
        int fireTimer;
        int destroyed = 0;
        Random rng = new Random();
        

        ////////////////////////////////////////////////////////////////////////////////
        // INITIALIZE THE SCREEN
        ////////////////////////////////////////////////////////////////////////////////
        public override void Initialize(ScreenManager mgr)
        {
            base.Initialize(mgr);
            playerImage = Content.Load<Texture2D>("destroyer");
            projectileImage = Content.Load<Texture2D>("plasmaBolt");
            player = new GameSprite(playerImage, Vector2.Zero);
            playerProjectile = new GameSprite(projectileImage, Vector2.Zero);
            shootSound = Content.Load<SoundEffect>("plasmaShot");
            shieldImage = Content.Load<Texture2D>("shield");
            alienImage = Content.Load<Texture2D>("alien4");
            // create shields with Y co-ordinate calculated so you can just see the projectile before it hits them.
            for (int i = 0; i < shields.Count(); i++)
                shields[i] = new GameSprite(shieldImage,
                    new Vector2((i+1) * screenWidth / (shields.Count()+1),
                    screenHeight - playerImage.Height - shieldImage.Height - projectileImage.Height));
            
            //saves the highscores to a document 
            string inputFile = "highscores.txt";
            //Loads in highscores
            if (File.Exists(inputFile))
            {
                StreamReader inputFileStream = new StreamReader(inputFile);
                for (int counter = 0; counter < 10; counter++)
                {
                   sharedData.highscoreNames[counter] = inputFileStream.ReadLine();
                   sharedData.highscores[counter] = Convert.ToInt32(inputFileStream.ReadLine());
                }
                inputFileStream.Close();
            }
        }


        ////////////////////////////////////////////////////////////////////////////////
        // START A NEW GAME
        ////////////////////////////////////////////////////////////////////////////////
        public override void Start()
        {
            lives = 3;
            sharedData.score = 0;
            ResetLevel();
        }

        private void ResetLevel()
        {
            ResetPlayer();
            ResetShields();
            ResetAliens();
        }

       

        private void ResetShields()
        {
            for (int i = 0; i < shields.Count(); i++)
                shieldHealth[i] = 10;
        }

        private void ResetPlayer()
        {
            //set the players starting position to the bottom middle of the screen
            player.position = new Vector2(screenWidth / 2, screenHeight - playerImage.Height / 2);
            fireTimer = 0;
            playerProjectile.position = new Vector2(0, -100);   // start off-screen
            playerProjectile.velocity.Y = -12;
        }

        public override void Update(GameTime gameTime)
        {
            MovePlayer(gameTime);
            MovePlayerProjectile();
            MoveAliens();
            MoveAlienProjectile();

            // when the player runs out of lives, reset the game and the screen.
            if (lives <= 0)
            {
                if (sharedData.score > sharedData.highscores[9])
                {
                    manager.GoToScreen("enterHighScore");
                }
                else
                manager.GoToScreen("gameOver");
                aliens.Clear();
                destroyed = 0;
            }

            base.Update(gameTime);
        }

        private void ResetAliens()
        {
            //spawns in enemies
            for (int alienNum = 0; alienNum < 10; alienNum++)
            {
                aliens.Add(new GameSprite(alienImage, new Vector2(alienNum * 60 + 100 , 100)));
                aliens[alienNum].velocity = new Vector2(2, 0);
            }
            for (int alienNum = 10; alienNum < 20; alienNum++)
            {
                aliens.Add(new GameSprite(alienImage, new Vector2(alienNum * 60 - 500, 150)));
                aliens[alienNum].velocity = new Vector2(2, 0);
            }
            for (int alienNum = 20; alienNum < 30; alienNum++)
            {
                aliens.Add(new GameSprite(alienImage, new Vector2(alienNum * 60 - 1100, 200)));
                aliens[alienNum].velocity = new Vector2(2, 0);
            } 
            for (int alienNum = 30; alienNum < 40; alienNum++)
            {
                aliens.Add(new GameSprite(alienImage, new Vector2(alienNum * 60 - 1700, 250)));
                aliens[alienNum].velocity = new Vector2(2, 0);
            }
        }

        private void MoveAliens()
       {
            bool sideHit = false;
            //if the alien hits one side of the screen, make aliens move towards the other side and vice versa
            for (int aliencounter = 0; aliencounter < aliens.Count(); aliencounter++)
            {
                aliens[aliencounter].position += aliens[aliencounter].velocity;
                if (aliens[aliencounter].position.X > screenWidth - 40 || aliens[aliencounter].position.X < 40)
                {
                    sideHit = true;
                }
            }
            if (sideHit)
                for (int alienNumber = 0; alienNumber < aliens.Count(); alienNumber++)
                {
                    aliens[alienNumber].velocity = -aliens[alienNumber].velocity;
                    aliens[alienNumber].position.Y += 40;
                }

            //Sets the enemy fire rate
            if (rng.Next(0, 2000) < 30)
            {
                int alienFired = rng.Next(0, aliens.Count());
                GameSprite bulletFired = new GameSprite(projectileImage, aliens[alienFired].position);
                bulletFired.velocity.Y = 12;
                alienProjectile.Add(bulletFired);
                
            }
        }

        private void MoveAlienProjectile()
        { 
            // check for enemy bullet collisions with shields and if collision occurs, shield loses health and the enemies bullet is removed.
            for (int bulletNo = 0; bulletNo < alienProjectile.Count(); bulletNo++)
            {
                alienProjectile[bulletNo].position += alienProjectile[bulletNo].velocity;
                for (int counter = 0; counter < shields.Count(); counter++)
                    if (alienProjectile[bulletNo].collision(shields[counter]) && shieldHealth[counter] > 0)
                    {
                        shieldHealth[counter]--;
                        alienProjectile.RemoveAt(bulletNo);
                        break;
                    }
            }

            // checking for collision with player and when it happens, player loses a life and the level resets. The bullet also disappears.
            for (int bullet = 0; bullet < alienProjectile.Count(); bullet++)
            if(alienProjectile[bullet].collision(player))
            {
                alienProjectile[bullet].position.X = 5000;
                ResetPlayer();
                lives--;
            }
        }


        private void MovePlayerProjectile()
        {
            playerProjectile.position += playerProjectile.velocity;

            // check for collisions with shields
            for (int i = 0; i < shields.Count(); i++)
                if (playerProjectile.collision(shields[i]) && shieldHealth[i] > 0)
                {
                    shieldHealth[i]--;
                    playerProjectile.position.Y = -100;
                }
            // check for collisions between projectile and alien
            for (int counter = 0; counter < aliens.Count(); counter++)
                if(playerProjectile.collision(aliens[counter]))
            {
                sharedData.score += 10;
                playerProjectile.position.Y = -100;
                aliens.Remove(aliens[counter]);
                if (aliens.Count() == 0)
                {
                    ResetAliens(); 
                }
            }

            
        }

        private void MovePlayer(GameTime gameTime)
        {
            // if the player presses the left and right arrow keys he movesw left and right respectively.
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) player.velocity.X -= 3;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) player.velocity.X += 3;
            player.velocity *= FRICTION_COEFFICIENT;
            player.position += player.velocity;
            if (player.position.X < player.origin.X) player.position.X = player.origin.X;
            if (player.position.X > screenWidth - player.origin.X) player.position.X = screenWidth - player.origin.X;

            fireTimer -= gameTime.ElapsedGameTime.Milliseconds;
            //if the space bar is pressed and the gun has cooled down, fire a bullet, play a shooting sound and reset the gun cooldown.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && fireTimer < 0)
            {
                playerProjectile.position = player.position + new Vector2(0, -player.origin.Y);
                shootSound.Play();
                fireTimer = MAXGUNCOOLDOWN;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawText.AlignedScaledAndRotated("score: " + sharedData.score,
                HorizontalAlignment.Left,
                VerticalAlignment.Top,
                0, 0, 0, 0.5f, Color.White, manager.defaultFont);
            DrawText.AlignedScaledAndRotated("lives: " + lives,
                HorizontalAlignment.Right,
                VerticalAlignment.Top,
                1, 0, 0, 0.5f, Color.White, manager.defaultFont);
            DrawText.AlignedScaledAndRotated("high score: " + sharedData.highscores[0],
                HorizontalAlignment.Center,
                VerticalAlignment.Top,
                0.5f, 0, 0, 0.5f, Color.White, manager.defaultFont);
            player.Draw(spriteBatch);
            playerProjectile.Draw(spriteBatch);
            for (int i = 0; i < shields.Count(); i++)
            {
                int shade = shieldHealth[i] * 26;
                Color colourtoDraw = new Color(0, shade, 0, shade);
                shields[i].Draw(spriteBatch, colourtoDraw);
            }
            //draws the enemy bullets
            for (int counter = 0; counter < alienProjectile.Count(); counter++)
            {
                alienProjectile[counter].Draw(spriteBatch);
            }

            //draws each enemy sprite
            foreach (GameSprite alien in aliens)
                {
                    alien.Draw(spriteBatch);
                }

            base.Draw(gameTime);
        }
    }
}
