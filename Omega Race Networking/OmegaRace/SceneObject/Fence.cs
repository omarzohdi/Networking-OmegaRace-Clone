using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmegaRace
{
    // Creates a laser fence which animates when hit
    public class Fence : SceneObject
    {
        // The animation to play when hit
        private AnimatedTextureFiles FenceAnimation;

        // If the animation is playing
        private bool playing = false;

        // If the fence is a horizontal fence or a vertical one
        // Used in collisions
        private bool isVertical;
        
        // Creates a fence based on a min and a max value and if the fence is long or short
        public Fence(Vector2 _min, Vector2 _max, bool isLong)
            : base((((_min/2) + (_max/2))), 0, "FencePost") // Centered between min and max
        {
            damage = 0; // Does no damage when collided with
            FenceAnimation = new AnimatedTextureFiles(Vector2.Zero, 0, 0, 0); // The animation for when hit
            if (_min.Y < _max.Y || _max.Y < _min.Y) // If the fence is vertical, the Ys will be different
            {
                _min.X = _max.X; // Just make sure the Xs are the same
                if (_min.Y < _max.Y) // Set the position to be the minimum value
                {
                    Position = _min;
                }
                else
                {
                    Position = _max;
                }
                // horizontal = false;
                isVertical = true; // Not horizontal, but vertical
                // Move the fence over a bit
                _min.X -= 3;
                _max.X += 3;
                if (isLong) // Select the correct animation
                {
                    FenceAnimation.Load(GameplayScreen.content, "Animations/FenceTall", 7, 12);
                }
                else
                {
                    FenceAnimation.Load(GameplayScreen.content, "Animations/FenceShort", 7, 12);
                }
            }
            else // Otherwise the Xs will be different
            {
                _min.Y = _max.Y; // Just make sure the Ys are the same
                if (_min.X < _max.X) // Set the position to be the minimum value
                {
                    Position = _min;
                }
                else
                {
                    Position = _max;
                }
                // horizontal = true;
                isVertical = false; // Not vertical, but horizontal
                // Move the fence over a bit
                _min.Y -= 3;
                _max.Y += 3;
                if (isLong) // Select the correct animation
                {
                    FenceAnimation.Load(GameplayScreen.content, "Animations/FenceWide", 7, 12);
                }
                else
                {
                    FenceAnimation.Load(GameplayScreen.content, "Animations/FenceThin", 7, 12);
                }
            }
            Collider = new CollisionRect(_min, _max); // Set the collider based on the adjusted min and max
        }

        // Update the animation if it is playing
        public override void Update(float deltaTime)
        {
            if (playing) // If the animation is playing, update it
            {
                FenceAnimation.UpdateFrame(deltaTime);
                // If the animation completes, stop and reset it
                if (FenceAnimation.PlayedThrough)
                {
                    playing = false;
                    FenceAnimation.Reset();
                }
                 
            }
        }

        // Draw the animation
        public override void Draw()
        {
            // Just draw it without rotations or scaling
        }

        // Called when a collision has occurred
        public override void onCollide(SceneObject collide)
        {
            SoundManager.playFenceHit(); // Play the sound
            playing = true; // Set the animation to play
        }

        // Determines the normal for objects bouncing off the fence
        public override Vector2 ReflectionNormal(Vector2 heading)
        {
            Vector2 nVector;

            // The reflection is based on if the fence is vertical or horizontal
            // and if the object has come from the left or the right or the top or the bottom
            if (isVertical)
            {
                if (heading.X < 0) // Came from the right
                {
                    nVector = new Vector2(-1, 0); // Go left
                }
                else // Come from the left
                {
                    nVector = new Vector2(1, 0); // Go right
                }
            }
            else // isHorizontal
            {
                if (heading.Y < 0) // Came from the bottom
                {
                    nVector = new Vector2(0, -1); // Go up
                }
                else // Came from the top
                {
                    nVector = new Vector2(0, 1); // Go down
                }
            }

            return nVector;
        }
    }
}
