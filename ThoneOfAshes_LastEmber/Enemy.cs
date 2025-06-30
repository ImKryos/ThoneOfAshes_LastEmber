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
        public bool IsDead = false; // Placeholder for enemy death logic
        public int health = 2; // Placeholder for enemy health
        public float deathFade = 1f; // Placeholder for death fade effect
        private float fadeSpeed = 1f; // Speed of fade effect

        public Enemy(Texture2D texture, Vector2 spawnPosition)
        {
            this.texture = texture;
            this.Position = spawnPosition;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                IsDead = true;
            }
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsDead)
            {
                deathFade -= fadeSpeed * delta; // Decrease the death fade over time
                if (deathFade < 0f) deathFade = 0f; // Ensure death fade does not go below 0
                return; // Skip update if dead
            }

            // Move toward the player
            Vector2 direction = playerPosition - Position;
            if (direction != Vector2.Zero)
                direction.Normalize();

            Position += direction * speed * delta;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            if (IsDead && deathFade <= 0f) return;

            float scale = 0.1f;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(
                texture,
                Position - cameraPosition,
                null,
                Color.White * (IsDead ? deathFade : 1f),
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
