using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThoneOfAshes_LastEmber.Weapons
{
    public class Ashbrand : Weapon
    {
        private Texture2D ashbrandTexture;
        private float orbitRadius = 60f; // Radius of the orbit around the player
        private float angle = 0f; // Angle for the orbiting effect
        private float rotationSpeed = 2f; // Speed of the orbiting effect - radians per second
        private Vector2 position; // Position of the Ashbrand relative to the player

        public Ashbrand(Texture2D texture) : base(texture)
        {
            this.ashbrandTexture = texture;
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            angle += rotationSpeed * delta; // Update the angle based on rotation speed

            // Keep angle within 0 to 2π range
            if (angle >= MathHelper.TwoPi)
            {
                angle -= MathHelper.TwoPi;
            }

            // Calculate orbit position around the player
            position = playerPosition + new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * orbitRadius;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            Vector2 origin = new Vector2(ashbrandTexture.Width / 2f, ashbrandTexture.Height / 2f);
            float scale = 0.05f; // Scale of the Ashbrand

            spriteBatch.Draw(
                ashbrandTexture,
                position - cameraPosition, // Position of the Ashbrand
                null, // No source rectangle
                Color.White, // Color of the Ashbrand
                angle, // Rotation based on the angle
                origin, // Origin for rotation
                scale, // Scale of the Ashbrand
                SpriteEffects.None, // No special effects
                0f // Layer depth
            );
        }

        public Vector2 GetPosition()
        {
            return position; // Return the current position of the Ashbrand
        }
    }
}
