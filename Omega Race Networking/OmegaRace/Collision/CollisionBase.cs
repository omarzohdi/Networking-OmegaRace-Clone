using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // Base abstract class for collisions extended by CollisionCircle and CollisionRect
    public abstract class CollisionBase
    {
        // Inactive colliders always return false for isColliding
        private bool active = true;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        // The position for the collider
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // Method for determining if two CollisionBases are colliding
        public abstract bool isColliding(CollisionBase collider);
    }
}
