using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // A circle collider
    public class CollisionCircle : CollisionBase
    {
        // The radius and its accessors
        private int radius;
        public int Radius
        {
            get { return radius; }
            set 
            { 
                radius = value;
                radiusSQ = radius * radius;
            }
        }

        // The squared radius for various comparisons
        private int radiusSQ;
        public int RadiusSQ
        {
            get { return radiusSQ; }
        }

        // Creates a collision circle centered on the position and sets the squared radius
        public CollisionCircle(Vector2 _position, int _radius)
        {
            Position = _position;
            radius = _radius;
            radiusSQ = radius * radius;
        }

        // Checks if colliding with the provided collider or not
        public override bool isColliding(CollisionBase collider)
        {
            bool colliding = false;

            // Both must be active
            if (Active && collider.Active)
            {
                // Check the type first
                if (collider is CollisionCircle)
                {
                    // We can save a square root by using the distance squared
                    float distanceSquared = (((Position.X - collider.Position.X) * (Position.X - collider.Position.X)) +
                        ((Position.Y - collider.Position.Y) * (Position.Y - collider.Position.Y)));
                    float collisionDistance = (this.radiusSQ + ((CollisionCircle)collider).RadiusSQ);

                    if (collisionDistance > distanceSquared)
                    {
                        colliding = true;
                    }
                    else
                    {
                        // False
                    }
                }
                // Rough circle to box collision, puts a box bounding on circle
                else if (collider is CollisionRect)
                {
                    // Check top1 < bottom2
                    if ((Position.Y + radius) < (collider.Position.Y - ((CollisionRect)collider).HalfHeight))
                    {
                        // False
                    }
                    // Check bottom1 > top2
                    else if ((Position.Y - radius) > (collider.Position.Y + ((CollisionRect)collider).HalfHeight))
                    {
                        // False
                    }
                    // Check right1 < left2
                    else if ((Position.X + radius) < (collider.Position.X - ((CollisionRect)collider).HalfWidth))
                    {
                        // False
                    }
                    // Check left1 > right2
                    else if ((Position.X - radius) > (collider.Position.X + ((CollisionRect)collider).HalfWidth))
                    {
                        // False;
                    }
                    else
                    {
                        colliding = true;
                    }
                }
            }
            else
            {
                // False
            }

            return colliding;
        }
    }
}
