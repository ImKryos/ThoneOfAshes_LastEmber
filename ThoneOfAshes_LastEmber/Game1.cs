using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ThoneOfAshes_LastEmber
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D playerTexture;
        Texture2D kindledTexture; // Placeholder for kindled texture
        Vector2 playerPosition;
        float playerSpeed = 200f; // Speed in pixels per second

        Texture2D ashwretchTexture; // Placeholder for Ashwretch texture
        List<Enemy> enemies = new List<Enemy>();


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 32x32 white square as a placeholder player sprite
            playerTexture = new Texture2D(GraphicsDevice, 32, 32);
            kindledTexture = Content.Load<Texture2D>("kindled");
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.OrangeRed; // Flame-y placeholder
            playerTexture.SetData(data);

            playerPosition = new Vector2(300, 200); // Starting position of the player

            
            ashwretchTexture = Content.Load<Texture2D>("ashwretch"); // Load Ashwretch texture
            enemies.Add(new Enemy(ashwretchTexture, new Vector2(400, 300))); // Add an enemy at a specific position


        }

        protected override void Update(GameTime gameTime)
        {
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

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
                scale,                       // SCALE (try 0.1 to 0.4 range)
                SpriteEffects.None,         // no flipping
                0f                          // layer depth
            );

            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            


            base.Draw(gameTime);
        }
    }
}
