using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // A laser that travels along a straight line until it collides
    public class Laser : SceneObject
    {
        private PlayerIndex index; // The player that fired the laser
        public PlayerIndex Index
        {
            get { return index; }
        }

        private const float LASER_SPEED = 300.0f; // The speed the laser travels at

        // Constructor for a laser
        public Laser(Vector2 _position, float rotation, PlayerIndex _index)
            : base(_position, rotation, determineSprite(_index))
        {
            index = _index; // The color for the laser and the player to give points to on a kill
            Velocity = -RotateAboutOrigin(Vector2.UnitY, Vector2.Zero, rotation); // Velocity is determined by the rotation of the laser
            Velocity.Normalize();
            Velocity *= LASER_SPEED; // The speed is normalized then multiplied by the speed factor
            Collider = new CollisionCircle(_position, 3); // Create a collision circle of size three
            damage = 1; // It does do damage
            Collider.Active = false; // Inactive until respawned
        }

        // Called during constructor time; pick the sprite to draw based on the player
        private static String determineSprite(PlayerIndex index)
        {
            String rValue = null;
            switch (index)
            {
                case PlayerIndex.One:
                    rValue = "BlueLaser";
                    break;
                case PlayerIndex.Two:
                    rValue = "GreenLaser";
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            return rValue;
        }

        // Called when a collision has occurred
        public override void onCollide(SceneObject collide)
        {
            ReadyToRemove = true; // Remove yourself on a collide

            SoundManager.playLaserHit(); // Play the hit sound

            Collider.Active = false; // Only collide once

            // Create particles and add them to the scene
            ParticleGenerator gen = new ParticleGenerator(Position, Velocity, Rotation, Sprite);
            GameplayScreen.GameInstance.CurrentScene.addSceneObject(gen);

            // Base method
            base.onCollide(collide);
        }

        // Redo the texture and booleans for being replaced into the world
        public void respawn(Vector2 _position, float _rotation, PlayerIndex _index)
        {
            Position = _position;
            index = _index; // The color for the laser and the player to give points to on a kill
            Rotation = _rotation;
            Velocity = -RotateAboutOrigin(Vector2.UnitY, Vector2.Zero, Rotation); // Velocity is determined by the rotation of the laser
            Velocity.Normalize();
            Velocity *= LASER_SPEED; // The speed is normalized then multiplied by the speed factor
            Velocity = Velocity;
            setSpriteByString(determineSprite(_index)); // Set the sprite to the correct color
            damage = 1; // Remember to do damage
            Collider.Active = true; // Activate the collider
            Collider.Position = _position;
            ReadyToRemove = false; // No longer ready to remove
        }

        // Used for determining the velocity of the laser based on its rotation
        private Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            Vector2 u = point - origin; // Point relative to origin  

            if (u == Vector2.Zero) // Early return
            {
                return point; // Warning: Early return
            }
             

            float a = (float)Math.Atan2(u.Y, u.X); // Angle relative to origin  
            a += rotation; // Rotate  

            // u is now the new point relative to origin  
            u = u.Length() * new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            return (u + origin);
        }

        // Called when removed from the scene
        public override void onRemove()
        {
            WeaponManager.addLaser(this); // Readd yourself to the list of recycled lasers
            base.onRemove(); // Base method
        }

        // Overriden normal method; no one should bounce off a laser
        public override Vector2 ReflectionNormal(Vector2 heading)
        {
            return Vector2.Zero; // So return zero
        }
    }
}
