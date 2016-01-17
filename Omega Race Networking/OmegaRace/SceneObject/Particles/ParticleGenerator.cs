using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmegaRace
{
    // Generates particles made from slicing up the texture given and spinning and moving it
    public class ParticleGenerator : SceneObject
    {
        // The number of slices to make of the image in the x and y direction
        private const int RECT_NUM_X = 4;
        private const int RECT_NUM_Y = 4;

        // The array of particles
        private Particle[] particles;

        // Time to live before the particle emitter is removed
        private const float TTL = 0.5f;
        private float ttl = TTL;

        // Constructor for the particle generator; creates all the particles from the image given
        public ParticleGenerator(Vector2 _position, Vector2 _velocity, float _rotation, Texture2D _texture)
            : base(_position, _rotation)
        {
            particles = new Particle[RECT_NUM_X * RECT_NUM_Y]; // Create the array
            // And populate it
            for (int i = 0; i < RECT_NUM_X; i++)
            {
                for (int j = 0; j < RECT_NUM_Y; j++)
                {
                    // Create a source rect for the particle by slicing up the image
                    Rectangle rect = new Rectangle(
                        i * _texture.Width / RECT_NUM_X,
                        j * _texture.Height / RECT_NUM_Y,
                        _texture.Width / RECT_NUM_X,
                        _texture.Height / RECT_NUM_Y);
                    // Relocate the particle to be where the src rect would be in game
                    Vector2 mov = new Vector2(
                        (i * _texture.Width / RECT_NUM_X - (_texture.Width / 2)) * (float)(Math.Sin(Rotation)),
                        (j * _texture.Height / RECT_NUM_Y - (_texture.Height / 2)) * (float)(-Math.Cos(Rotation)));
                    // Set the position
                    Vector2 pos = new Vector2(
                        _position.X,
                        _position.Y);
                    pos += mov; // The move it by the offset
                    // Center the origin around the center of the src rect for spinning purposes
                    Vector2 orig = new Vector2(
                        rect.Center.X/2,
                        rect.Center.Y/2
                        );
                    // Create a new particle
                    particles[j + (i * RECT_NUM_Y)] = new Particle(rect,
                        pos, _velocity, orig, _rotation, _texture);
                }
            }
        }

        // Update the ttl timer and the particles
        public override void Update(float deltaTime)
        {
            ttl -= deltaTime; // Reduce the time to live
            for (int i = 0; i < particles.Length; i++) // Update every particle
            {
                particles[i].Update(deltaTime);
            }

            // If time to live is up, remove yourself
            if (ttl <= 0)
            {
                ReadyToRemove = true;
            }
             
        }

        // Draw every partical, unless you are being removed
        public override void Draw()
        {
            if (!ReadyToRemove)
            {
                // Draw every particle
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Draw();
                }
                base.Draw();
            }
             
        }

        // Private inner class Particle
        private class Particle
        {
            // The texture to pull form
            private Texture2D texture;
            
            // Adjust the color so players feel less threatened
            private Color recolor;

            // Speed to rotate at
            private const float ROTATION_SPEED = 3.0f; // Modifier for rotation speed
            private float rotationModifier; // Positive or negative randomness to the rotation

            // The position of the particle
            private Vector2 position;

            // The rotation of the particle in degrees
            private float rotation;

            // The origin to rotate about
            private Vector2 origin;

            // The velocity of the particle
            private Vector2 velocity;

            // The src rect for drawing the particle
            private Rectangle rect;

            // Scale on the random added to velocity
            private const float VELOCITY_SCALE = 5.0f;

            // A static random used to give some randomness to the particles' movements and rotations
            private static Random random;

            // Creates a particle and adds some random to it
            public Particle(Rectangle _rect, Vector2 _position, Vector2 _velocity, Vector2 _origin, float _rotation, Texture2D _texture)
            {
                // Set all the things passed in
                position = _position;
                rotation = _rotation;
                velocity = _velocity;
                origin = _origin;
                texture = _texture;
                rect = _rect;

                recolor = Color.Gray;

                // If you're the first particle, seed the random and create it
                if (random == null)
                {
                    random = new Random(System.DateTime.Now.Millisecond);
                }
                 

                // Modify the rotation with some random
                rotationModifier = (float)random.NextDouble();
                if (random.Next() % 2 == 0) // Also allow negative rotations
                {
                    rotationModifier *= -1;
                }
                 
                // Create some random on the velocity
                float x = (float)random.NextDouble() * VELOCITY_SCALE;
                float y = (float)random.NextDouble() * VELOCITY_SCALE;
                if (random.Next() % 2 == 0) // Also allow negative
                {
                    x *= -1;
                }
                 
                if (random.Next() % 2 == 0) // Also allow negative
                {
                    y *= -1;
                }
                 
                // Update the velocity with the random
                velocity.X += x;
                velocity.Y += y;
            }

            // Move and rotate the particle
            public void Update(float deltaTime)
            {
                position += velocity * deltaTime; // Move the particle
                rotation += deltaTime * rotationModifier * ROTATION_SPEED; // And rotate it
            }

            // Draw the particle rotated about on its origin
            public void Draw()
            {
                SpriteBatch spriteBatch = GameplayScreen.GameInstance.SpriteBatch;
                spriteBatch.Begin();
                spriteBatch.Draw(texture, position, rect, recolor, rotation,
                                    origin, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }
        }
    }
}
