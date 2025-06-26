using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThoneOfAshes_LastEmber
{
    public class Enemy
    {
        public Vector2 Position;
        private Texture2D texture;
        private float speed = 50f; // Speed in pixels per second

        public Enemy(Texture2D texture, Vector2 spawnPosition)
        {
            this.texture = texture;
            this.Position = spawnPosition;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move toward the player
            Vector2 direction = playerPosition - Position;
            if (direction != Vector2.Zero)
                direction.Normalize();

            Position += direction * speed * delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            float scale = 0.1f; // Scale down the texture for better visibility

            spriteBatch.Draw(
                texture, 
                Position,
                null,
                Color.White,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
                );
        }
    }
}
