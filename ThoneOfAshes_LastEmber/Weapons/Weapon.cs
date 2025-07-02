using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThoneOfAshes_LastEmber.Weapons
{
    public abstract class Weapon
    {
        protected Texture2D texture; // Texture for the weapon
        protected Vector2 playerPosition; // Position of the player

        public Weapon(Texture2D texture)
        {
            this.texture = texture;
        }

        public virtual void Update(GameTime gameTime, Vector2 playerPosition)
        {
            this.playerPosition = playerPosition; // Update the player's position
        }

        public abstract void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition);

    }
}
