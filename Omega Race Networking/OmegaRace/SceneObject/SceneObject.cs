using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmegaRace
{
    // Base class for anything drawn on screen
    public abstract class SceneObject
    {
        // The sprite representing the scene object in the game world
        private Texture2D sprite; // Only accessible from this class
        public Texture2D Sprite
        {
            get { return sprite; }
            set
            {
                sprite = value;
                origin.X = (sprite.Width / 2);
                origin.Y = (sprite.Height / 2);
            }
        }
        protected Texture2D setSpriteByString(String spriteName) // Loads a sprite and set the origin for rotations
        {
            sprite = GameplayScreen.content.Load<Texture2D>(spriteName);
            origin.X = (sprite.Width / 2);
            origin.Y = (sprite.Height / 2);
            return sprite;
        }

        // For drawing rotations
        private Vector2 origin;

        // The damage it does when it collides with something
        protected int damage = 0; // Really is zero or one
        public int Damage
        {
            get { return damage; } // Remember: This is the damage is DEALS
        }

        // Determines if it needs to be drawn and updated
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // The position in the game world
        private Vector2 position;
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (collider != null) // Make sure to move the collider, too
                {
                    collider.Position = position;
                }
                 
            }
        }

        // The position at last update, used for collisions
        private Vector2 lastPosition;
        public virtual Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        // The velocity of the scene object
        private Vector2 velocity;
        public virtual Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        // The rotation of the scene object
        private float rotation;
        public virtual float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        // A collider for the scene object - may be null
        private CollisionBase collider;
        public CollisionBase Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        // For use by the GameScene class to find items to remove from the list of scene objects
        // Will be removed at the end of update
        private bool readyToRemove;
        public bool ReadyToRemove
        {
            get { return readyToRemove; }
            set { readyToRemove = value; }
        }

        // Used in collisions to transfer energy
        protected float momentumConstant = -0.25f;

        protected SceneObject(float _rotation, String spriteName)
        {
            Position = Vector2.Zero;
            Rotation = _rotation;

            sprite = GameplayScreen.content.Load<Texture2D>(spriteName);

            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            readyToRemove = false;
        }


        // Base constructor for a scene object with no sprite
        protected SceneObject(Vector2 _position, float _rotation)
        {
            Position = _position;
            Rotation = _rotation;

            sprite = null;

            origin = Vector2.Zero;

            readyToRemove = false;
        }

        // Base constructor for a scene object with a sprite, also sets the origin at the center
        protected SceneObject(Vector2 _position, float _rotation, String spriteName)
        {
            Position = _position;
            Rotation = _rotation;

            sprite = GameplayScreen.content.Load<Texture2D>(spriteName);

            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            readyToRemove = false;
        }

        // Moves the object and its collider along its velocity and sets the lastPosition
        public virtual void Update(float deltaTime)
        {
            lastPosition = position;
            position += velocity * deltaTime;

            if (collider != null)
            {
                collider.Position = position;
            }
             
        }

        // Draws the scene object with its rotation about the origin if it has a sprite
        public virtual void Draw()
        {
            if (sprite != null)
            {
                SpriteBatch spriteBatch = GameplayScreen.GameInstance.SpriteBatch;
                spriteBatch.Begin();
                spriteBatch.Draw(sprite, position, null, Color.White, rotation,
                                    origin, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }
             
        }

        // Overridable onRemove method called when the object is removed from the Scene
        public virtual void onRemove()
        {

        }

        // Checks if the scene object is colliding with a specific item
        public bool isColliding(SceneObject item)
        {
            if (collider != null && item.Collider != null)
            {
                return (item.Collider.isColliding(collider));
            }
            else
            {
                return false;
            }
        }

        // Called when an item collides with this
        // Moves the item out of the collision along the collision normal
        public virtual void onCollide(SceneObject collision)
        {
            // Move the collider back
            collider.Position = lastPosition;
            // Find the normal of the collision
            Vector2 rNormal = collision.ReflectionNormal(velocity);
            if (rNormal.Length() != 0) // If the normal is not length zero,
            {
                rNormal.Normalize(); // Normalize it
                while (collider.isColliding(collision.collider)) // As long as we are still colliding
                {
                    lastPosition += rNormal; // Move back along the normal
                    collider.Position = lastPosition; // Move the collider with
                }
            }
             
            position = lastPosition; // Move to the new position
            collider.Position = position; // And move the collider with
            // Only ships need to worry about transferring momentum
        }

        // Overridable; Determines the normal of reflection based on a velocity
        public virtual Vector2 ReflectionNormal(Vector2 heading)
        {
            Vector2 rVector = (collider.Position - heading);
            if (rVector.LengthSquared() > 0)
            {
                rVector.Normalize();
            }
             
            return rVector;
        }

        // Overridable; Determines the normal of reflection based on a velocity
        public virtual Vector2 momentumTransfer(Vector2 heading)
        {
            return (heading + (Velocity * momentumConstant)) / 2;
        }
    }
}
