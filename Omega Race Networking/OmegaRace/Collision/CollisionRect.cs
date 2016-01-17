using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // An axis-aligned collision rectangle
    public class CollisionRect : CollisionBase
    {
        // The width and all its accessors
        private int width;

        public int Width
        {
            get { return width; }
            set 
            { 
                width = value;
                halfWidth = width / 2;
            }
        }
        private int halfWidth;
        public int HalfWidth
        {
            get { return halfWidth; }
        }

        // The height and all its accessors
        private int height;
        public int Height
        {
            get { return height; }
            set 
            { 
                height = value;
                halfHeight = height / 2;
            }
        }
        private int halfHeight;
        public int HalfHeight
        {
            get { return halfHeight; }
        }

        // Creates a collision rect centered between two values, min must be <= than max for x, y, and z
        public CollisionRect(Vector2 _min, Vector2 _max)
        {
            width = (int)(_max.X - _min.X);
            height = (int)(_max.Y - _min.Y);
            halfWidth = width / 2;
            halfHeight = height / 2;
            Position = new Vector2(_min.X + halfWidth, _min.Y + halfHeight);
        }

        // Creates a collision rect centered at the provided position with the provided width and height
        public CollisionRect(Vector2 _position, int _width, int _height)
        {
            Position = _position;
            width = _width;
            height = _height;
            halfWidth = width / 2;
            halfHeight = height / 2;
        }

        // Checks if colliding with the provided collider or not
        public override bool isColliding(CollisionBase collider)
        {
            bool colliding = false;

            // Both must be active
            if (Active && collider.Active)
            {
                // Check the type first
                if (collider is CollisionRect)
                {
                    // Check top1 < bottom2
                    if ((Position.Y + halfHeight) <
                        (collider.Position.Y - ((CollisionRect)collider).HalfHeight))
                    {
                        // False
                    }
                    // Check bottom1 > top2
                    else if ((Position.Y - halfHeight) >
                        (collider.Position.Y + ((CollisionRect)collider).HalfHeight))
                    {
                        // False
                    }
                    // Check right1 < left2
                    else if ((Position.X + halfWidth) < (collider.Position.X -
                           ((CollisionRect)collider).HalfWidth))
                    {
                        // False
                    }
                    // Check left1 > right2
                    else if ((Position.X - halfWidth) > (collider.Position.X +
                           ((CollisionRect)collider).HalfWidth))
                    {
                        // False
                    }
                    else
                    {
                        colliding = true;
                    }
                }
                // Rough circle to box collision, puts a box bounding on circle
                else if (collider is CollisionCircle)
                {
                    // Check top1 < bottom2
                    if ((Position.Y + halfHeight) < (collider.Position.Y -
                            ((CollisionCircle)collider).Radius))
                    {
                        // False
                    }
                    // Check bottom1 > top2
                    else if ((Position.Y - halfHeight) > (collider.Position.Y +
                           ((CollisionCircle)collider).Radius))
                    {
                        // False
                    }
                    // Check right1 < left2
                    else if ((Position.X + halfWidth) < (collider.Position.X -
                           ((CollisionCircle)collider).Radius))
                    {
                        // False
                    }
                    // Check left1 > right2
                    else if ((Position.X - halfWidth) >
                       (collider.Position.X + ((CollisionCircle)collider).Radius))
                    {
                        // False
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
