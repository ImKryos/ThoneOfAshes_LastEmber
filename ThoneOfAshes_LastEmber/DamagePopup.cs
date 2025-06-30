using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThoneOfAshes_LastEmber
{
    public class DamagePopup
    {
        public Vector2 Position;
        public string Text;
        private float lifetime = 1f; // Duration the popup will be visible
        private float timer = 0f; // Timer to track the lifetime of the popup

        public DamagePopup(Vector2 position, int damage)
        {
            Position = position;
            Text = "-" + damage.ToString();
        }

        public bool Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += delta;
            Position.Y -= 20 * delta; // Move the popup upwards
            return timer > lifetime; // Return true if the popup should be 
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 cameraPosition)
        {
            spriteBatch.DrawString(font, 
                Text, // Draw the text
                Position - cameraPosition, // Position of the popup
                Color.OrangeRed, // Color of the text
                0f, // Rotation of the text
                Vector2.Zero, // Origin of the text (no offset)
                1.5f, // Scale of the text
                SpriteEffects.None, // No special effects
                0f // Layer depth (0f is the default)
             );
        }

    }
}
