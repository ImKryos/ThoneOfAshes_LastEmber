using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThoneOfAshes_LastEmber
{
    public class Cindersoul
    {
        public Vector2 Position;
        private Texture2D texture;
        private float floatSpeed = 20f; // bobbing speed in pixels per second
        private float timer = 0f; // timer to track the bobbing effect

        public bool Collected = false; // Flag to check if the Cindersoul has been collected

        public Cindersoul(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.Position = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += delta;
            Position.Y += (float)Math.Sin(timer * 4f) * floatSpeed * delta; // gentle bob
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
                0.05f, // Scale of the Cindersoul
                SpriteEffects.None, 
                0f // Layer depth
            );
        }
    }
}
