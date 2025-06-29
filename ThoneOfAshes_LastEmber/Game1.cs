﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThoneOfAshes_LastEmber
{
    public class Game1 : Game
    {
        Random rng = new Random(); // Random number generator
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
        float spawnInterval = 3f; // Time in seconds between enemy spawns
        Random random = new Random(); // For positioning

        int playerHP = 5; // Player health
        float damageCooldown = 1f; // Cooldown time in seconds after taking damage
        float damageTimer = 0f; // Timer to track damage cooldown
        int playerLevel = 1; // Player level, can be used for scaling difficulty or abilities
        int currentXP = 0; // Current experience points
        int xpToNextLevel = 5; // Experience points required to level up
        bool levelUpPending = false; // Flag to indicate if a level-up is pending

        List<DamagePopup> damagePopups = new List<DamagePopup>(); // List to hold damage popups

        Color damageFlashColor = Color.DarkRed; // Color for the damage flash effect
        float flashAlpha = 0f; // Alpha value for the flash effect
        float flashDuration = 0.2f; // Duration of the flash effect in seconds

        List<Projectile> projectiles = new List<Projectile>(); // List to hold projectiles
        Texture2D flameburstTexture; // Placeholder for the flameburst texture
        float projectileTimer = 0f; // Timer for projectile firing
        float projectileCooldown = 3.0f; // Cooldown time in seconds between projectiles

        Texture2D cindersoulTexture; // Placeholder for Cindersoul texture
        List<Cindersoul> cindersouls = new List<Cindersoul>(); // List to hold Cindersouls

        Rectangle upgradeButtonRect = new Rectangle(100, 250, 500, 60); // Rectangle for the upgrade button
        MouseState previousMouseState; // To track mouse state for button clicks


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
            flameburstTexture = Content.Load<Texture2D>("flameburst"); // Load the flameburst texture
            cindersoulTexture = Content.Load<Texture2D>("cindersoul"); // Load the Cindersoul texture

        }

        protected override void Update(GameTime gameTime)
        {

            damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (levelUpPending)
            {
                MouseState mouseState = Mouse.GetState();

                if (mouseState.LeftButton == ButtonState.Pressed && 
                    previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point clickPos = mouseState.Position;

                    if (upgradeButtonRect.Contains(clickPos))
                    {
                        projectileCooldown *= 0.8f; // Reduce cooldown time by 20%
                        levelUpPending = false; // Reset level-up pending flag
                    }
                }
                previousMouseState = mouseState; // Update previous mouse state

                return;
            }

            KeyboardState keyState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (keyState.IsKeyDown(Keys.W)) playerPosition.Y -= playerSpeed * deltaTime; // Move up
            if (keyState.IsKeyDown(Keys.S)) playerPosition.Y += playerSpeed * deltaTime; // Move down
            if (keyState.IsKeyDown(Keys.A)) playerPosition.X -= playerSpeed * deltaTime; // Move left
            if (keyState.IsKeyDown(Keys.D)) playerPosition.X += playerSpeed * deltaTime; // Move right

            projectileTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (projectileTimer >= projectileCooldown)
            {
                projectileTimer = 0f; // Reset the projectile timer

                // Fire toward the nearest enemy if one exists
                if (enemies.Count >0)
                {
                    Enemy closest = enemies.OrderBy(e => Vector2.Distance(playerPosition, e.Position)).First();

                    Vector2 direction = closest.Position - playerPosition;
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize(); // Normalize the direction vector
                    }

                    projectiles.Add(new Projectile(flameburstTexture, playerPosition, direction)); // Create a new projectile
                }
            }

            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, playerPosition);
            }

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue; // Skip dead enemies

                float distance = Vector2.Distance(playerPosition, enemy.Position);
                float collisionRadius = 40f; // Adjust this value based on your player and enemy sizes

                if (distance < collisionRadius && damageTimer >= damageCooldown)
                {
                    playerHP--; // Decrease player health ** UPDATE LATER with enemy damage value
                    damageTimer = 0f; // Reset the damage timer
                    damagePopups.Add(new DamagePopup(playerPosition + new Vector2(-10, -40), 1)); // Add a damage popup at the player's position
                    flashAlpha = 1f; // Set the flash effect to full alpha

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

            if (flashAlpha > 0)
            {
                float fadeSpeed = 1f / flashDuration; // Speed of the fade effect
                flashAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * fadeSpeed;
                if (flashAlpha < 0f) flashAlpha = 0f; // Ensure alpha doesn't go below 0
            }

            for (int i = projectiles.Count -1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime);
                if (projectiles[i].IsExpired)
                {
                    projectiles.RemoveAt(i); // Remove expired projectiles
                }
            }

            for (int i = projectiles.Count -1; i >= 0; i--)
            {
                var projectile = projectiles[i];

                foreach (var enemy in enemies)
                {
                    if (enemy.IsDead) continue; // Skip dead enemies

                    float distance = Vector2.Distance(projectile.Position, enemy.Position);
                    float hitRadius = 20f; // Adjust this value based on your projectile and enemy sizes

                    if (distance < hitRadius)
                    {
                        enemy.TakeDamage(1); // Deal damage to the enemy
                        if (enemy.IsDead)
                        {
                            if (rng.NextDouble() < 0.75) // 75% chance to drop a Cindersoul
                            {
                                cindersouls.Add(new Cindersoul(cindersoulTexture, enemy.Position)); // Add a Cindersoul at the enemy's position
                            }
                        }
                        projectiles.RemoveAt(i); // Remove the projectile after hitting
                        break; // Exit the loop to avoid modifying the collection during iteration
                    }
                }
            }

            enemies.RemoveAll(e => e.IsDead && e.health <= 0 && e.deathFade <= 0f); // Remove dead enemies

            for (int i = cindersouls.Count - 1; i >= 0; i--)
            {
                var soul = cindersouls[i];
                soul.Update(gameTime);

                float collectDistance = 30f; // Distance to collect the Cindersoul
                if (Vector2.Distance(playerPosition, soul.Position) < collectDistance)
                {
                    currentXP++;
                    if (currentXP >= xpToNextLevel)
                    {
                        playerLevel++;
                        currentXP -= xpToNextLevel; // Reset XP after leveling up
                        levelUpPending = true; // Set flag for level-up
                        xpToNextLevel = (int)(xpToNextLevel * 1.5f); // Increase XP needed for next level
                    }
                    cindersouls.RemoveAt(i); // Remove the Cindersoul after collection
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

            // Draw projectiles
            foreach (var projectile in projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            foreach (var soul in cindersouls)
            {
                soul.Draw(_spriteBatch);
            }

            _spriteBatch.DrawString(uiFont, $"XP: {currentXP}", new Vector2(10, 10), Color.LightGoldenrodYellow); // Draw the XP counter

            

            _spriteBatch.End();

            if (levelUpPending)
            {
                _spriteBatch.Begin();

                // Draw background box for the level-up prompt
                Texture2D upgradeBox = new Texture2D(GraphicsDevice, 1, 1);
                upgradeBox.SetData(new[] { Color.DarkSlateGray }); // Create a 1x1 dark slate gray texture for the box

                _spriteBatch.Draw(
                    upgradeBox,
                    upgradeButtonRect,
                    Color.DarkSlateGray * 0.8f
                ); // Draw the upgrade button background

                _spriteBatch.DrawString(
                    uiFont,
                    "Upgrade Flameburst: Cooldown reduced by 20%",
                    new Vector2(upgradeButtonRect.X + 10, upgradeButtonRect.Y + 20),
                    Color.White
                );

                _spriteBatch.End();
            }

                if (flashAlpha > 0f)
            {
                Texture2D flashTexture = new Texture2D(GraphicsDevice, 1, 1);
                flashTexture.SetData(new[] { Color.White }); // Create a 1x1 white texture for the flash effect

                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    flashTexture,
                    new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                    damageFlashColor * flashAlpha // Apply the flash color with alpha
                );
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
