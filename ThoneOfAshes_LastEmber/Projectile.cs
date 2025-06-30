using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThoneOfAshes_LastEmber
{
    public class Projectile
    {

        public Vector2 Position;
        public Vector2 Velocity;
        private Texture2D texture;
        private float speed = 400f; // Speed in pixels per second
        private float lifetime = 3f; // Lifetime of the projectile in seconds
        private float timer = 0f; // Timer to track the lifetime of the projectile
        public bool IsExpired => timer >= lifetime; // Property to check if the projectile has expired

        public Projectile(Texture2D texture, Vector2 startPosition, Vector2 direction)
        {
            this.texture = texture;
            this.Position = startPosition;
            this.Velocity = direction * speed; // Set the velocity based on the direction and speed
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * delta; // Update the position based on velocity and elapsed time
            timer += delta; // Update the timer
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(
                texture, 
                Position - cameraPosition, 
                null, 
                Color.White, 
                0f, 
                origin, 
                0.03f, // Scale of the projectile
                SpriteEffects.None, 
                0f // Layer depth
            );
        }

    }
}
