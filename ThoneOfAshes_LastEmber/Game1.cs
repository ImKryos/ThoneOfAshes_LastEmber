using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ThoneOfAshes_LastEmber
{
    public class Game1 : Game
    {

        SpriteFont uiFont;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D playerTexture;
        Texture2D kindledTexture; // Placeholder for kindled texture
        Vector2 playerPosition;
        float playerSpeed = 200f; // Speed in pixels per second

        Texture2D ashwretchTexture; // Placeholder for Ashwretch texture
        List<Enemy> enemies = new List<Enemy>();

        float spawnTimer = 0f; // Timer for spawning enemies
        float spawnInterval = 2f; // Time in seconds between enemy spawns
        Random random = new Random(); // For positioning

        int playerHP = 5; // Player health
        float damageCooldown = 1f; // Cooldown time in seconds after taking damage
        float damageTimer = 0f; // Timer to track damage cooldown

        List<DamagePopup> damagePopups = new List<DamagePopup>(); // List to hold damage popups



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            _graphics.PreferredBackBufferWidth = 1920; // Set the width of the window
            _graphics.PreferredBackBufferHeight = 1080; // Set the height of the window
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {

            Window.AllowUserResizing = true; // Allow the window to be resized

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            uiFont = Content.Load<SpriteFont>("UIFont"); // Load the UI font

            // Create a 32x32 white square as a placeholder player sprite
            playerTexture = new Texture2D(GraphicsDevice, 32, 32);
            kindledTexture = Content.Load<Texture2D>("kindled");
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.OrangeRed; // Flame-y placeholder
            playerTexture.SetData(data);

            playerPosition = new Vector2(
                _graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f
            ); // Start in the center of the screen 


            ashwretchTexture = Content.Load<Texture2D>("ashwretch"); // Load Ashwretch texture
            enemies.Add(new Enemy(ashwretchTexture, new Vector2(1, 1))); // Add an enemy at a specific position


        }

        protected override void Update(GameTime gameTime)
        {

            damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (keyState.IsKeyDown(Keys.W)) playerPosition.Y -= playerSpeed * deltaTime; // Move up
            if (keyState.IsKeyDown(Keys.S)) playerPosition.Y += playerSpeed * deltaTime; // Move down
            if (keyState.IsKeyDown(Keys.A)) playerPosition.X -= playerSpeed * deltaTime; // Move left
            if (keyState.IsKeyDown(Keys.D)) playerPosition.X += playerSpeed * deltaTime; // Move right

            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, playerPosition);
            }

            foreach (var enemy in enemies)
            {
                float distance = Vector2.Distance(playerPosition, enemy.Position);
                float collisionRadius = 40f; // Adjust this value based on your player and enemy sizes

                if (distance < collisionRadius && damageTimer >= damageCooldown)
                {
                    playerHP--; // Decrease player health ** UPDATE LATER with enemy damage value
                    damageTimer = 0f; // Reset the damage timer
                    damagePopups.Add(new DamagePopup(playerPosition + new Vector2(-10, -40), 1)); // Add a damage popup at the player's position
                
                    if (playerHP <= 0)
                    {
                        Exit(); // Exit the game if player health reaches 0d
                    }
                }
            }

            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f; // Reset the spawn timer

                // Get screen dimensions
                int screenWidth = _graphics.PreferredBackBufferWidth;
                int screenHeight = _graphics.PreferredBackBufferHeight;

                // Pick a spawn side: 0 = top, 1 = bottom, 2 = left, 3 = right
                int side = random.Next(4);
                Vector2 spawnPosition = Vector2.Zero;

                switch (side)
                {
                    case 0: // Top
                        spawnPosition = new Vector2(random.Next(0, screenWidth), -50);
                        break;
                    case 1: // Bottom
                        spawnPosition = new Vector2(random.Next(0, screenWidth), screenHeight + 50);
                        break;
                    case 2: // Left
                        spawnPosition = new Vector2(-50, random.Next(0, screenHeight));
                        break;
                    case 3: // Right
                        spawnPosition = new Vector2(screenWidth + 50, random.Next(0, screenHeight));
                        break;
                }

                // Create a new enemy at the spawn position
                enemies.Add(new Enemy(ashwretchTexture, spawnPosition));

            }

            for (int i = damagePopups.Count - 1; i >= 0; i--)
            {
                if (damagePopups[i].Update(gameTime))
                {
                    damagePopups.RemoveAt(i); // Remove the popup if its lifetime has expired
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw the player
            _spriteBatch.Begin(); 
            Vector2 origin = new Vector2(kindledTexture.Width / 2f, kindledTexture.Height / 2f); // Center the origin for rotation
            float scale = 0.1f; // Scale down the texture for better visibility
            _spriteBatch.Draw(
                kindledTexture,             // the texture
                playerPosition,             // position on screen
                null,                       // source rectangle (null = full image)
                Color.White,                // tint
                0f,                         // rotation
                origin,                     // origin
                scale,                      // SCALE (try 0.1 to 0.4 range)
                SpriteEffects.None,         // no flipping
                0f                          // layer depth
            );

            // Draw the player HP Bar
            int maxHP = 5; // Maximum health points
            int hpBarWidth = 40; // Width of the HP bar
            int hpBarHeight = 6; // Height of the HP bar

            float hpPercentage = (float)playerHP / maxHP; // Calculate the percentage of HP remaining
            Vector2 hpBarPosition = playerPosition + new Vector2(-hpBarWidth / 2, 40); // Position of the HP bar

            Texture2D hpBar = new Texture2D(GraphicsDevice, 1, 1);
            hpBar.SetData(new[] { Color.White }); // Create a 1x1 white texture for the HP bar

            _spriteBatch.Draw(hpBar, new Rectangle((int)hpBarPosition.X, (int)hpBarPosition.Y, hpBarWidth, hpBarHeight), Color.DarkRed); // background
            _spriteBatch.Draw(hpBar, new Rectangle((int)hpBarPosition.X, (int)hpBarPosition.Y, (int)(hpBarWidth * hpPercentage), hpBarHeight), Color.Orange); // foreground

            // Draw the enemies
            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }

            // Draw damage popups
            foreach (var popup in damagePopups)
            {
                popup.Draw(_spriteBatch, uiFont);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
