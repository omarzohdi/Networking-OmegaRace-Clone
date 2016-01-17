using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // Just a class to draw fence posts for bouncing off of
    public class FencePost : SceneObject
    {
        // Constructor, just creates a collider and sets the image
        public FencePost(Vector2 _position)
            : base(_position, 0, "FencePost")
        {
            Collider = new CollisionCircle(Position, 10); // Creates a collider of size 10
        }

        // Nothing to do, just sit there
        public override void Update(float deltaTime)
        {
            
        }
        
        // Just call the base draw
        public override void Draw()
        {
            base.Draw();
        }

        // Nothing to do when collided with
        public override void onCollide(SceneObject collision)
        {
            
        }
    }
}
