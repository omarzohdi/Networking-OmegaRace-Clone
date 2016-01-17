using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // A placeable mine
    // Arms after a few seconds, but explodes if left too long
    public class Mine : SceneObject
    {
        private PlayerIndex index; // The player that layed the mine
        public PlayerIndex Index
        {
            get { return index; }
        }

        // Time to live - how long before exploding after placed
        private const float TTL = 5.0f;
        private float ttl = TTL;

        // Time to arm - how long before a newly placed mine becomes active and deadly
        private const float TTA = 1.0f;
        private float tta = TTA;

        // Adjustment for the animation speed
        private const float ANIMATION_SPEED_SCALE = 3.0f;

        // The animation for an armed mine
        private AnimatedTexture mineAnimation;

        // Bool to ensure the armed sound is only played once
        private bool playSound;

        // Constructor to set up a mine
        public Mine(Vector2 _position, float _rotation, PlayerIndex _index)
            : base(_position, _rotation, determineSprite(_index))
        {
            index = _index; // The color for the mine and the player to give points to on a kill
            Collider = new CollisionCircle(_position, 1); // Create a collision circle
            damage = 1; // It does do damage
            Collider.Active = false; // Turn off the collider until ready
            playSound = false;
            mineAnimation = new AnimatedTextureFiles(Position, Rotation, 1.0f, 0); // Create an animation

            switch (index) // Load the right texture
            {
                case PlayerIndex.One:
                    mineAnimation.Load(GameplayScreen.content, "Animations/BlueMine", 2, 1);
                    break;
                case PlayerIndex.Two:
                    mineAnimation.Load(GameplayScreen.content, "Animations/GreenMine", 2, 1);
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            mineAnimation.Rotation = Rotation;
            mineAnimation.Origin = Position;
        }

        // Called during constructor time; pick the sprite to draw based on the player
        private static String determineSprite(PlayerIndex index)
        {
            String rValue = null;
            switch (index)
            {
                case PlayerIndex.One:
                    rValue = "BlueMine";
                    break;
                case PlayerIndex.Two:
                    rValue = "GreenMine";
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            return rValue;
        }

        // Count down the timers and play the sound once and update the animation
        public override void Update(float deltaTime)
        {
            // Update time to live
            ttl -= deltaTime;
            if (ttl <= 0) // If you've lived too long, pop
            {
                onCollide(this);
            }
             

            // Update the time to arm
            tta -= deltaTime;
            if (tta <= 0) // If you're armed
            {
                if (!playSound) // If you haven't played the sound yet
                {
                    SoundManager.playMineArm(); // Play it 
                    playSound = true; // but only once
                }
                 
                Collider.Active = true; // Activate the collider
                mineAnimation.UpdateFrame(deltaTime * ANIMATION_SPEED_SCALE); // Update the blinking animation
            }
             
            
            // Base scene object update
            base.Update(deltaTime);
        }

        public override void Draw()
        {
            if (tta <= 0) // If you're armed
            {
                // Play the armed animation; don't draw the sprite
                mineAnimation.DrawFrame(GameplayScreen.GameInstance.ScreenManager.SpriteBatch, Position);
            }
            else 
            {
                // Otherwise draw the sprite; not the animation
                base.Draw();
            }
        }

        // Redo the texture and timers and booleans for being replaced into the world
        public void respawn(Vector2 _position, float _rotation, PlayerIndex _index)
        {
            index = _index; // The color for the mine and the player to give points to on a kill
            Position = _position;
            Rotation = _rotation;
            mineAnimation = new AnimatedTextureFiles(Position, Rotation, 1.0f, 0); // The animation needs to be remade
            playSound = false; // Ready to play the sound again
            switch (index) // Load the right animation
            {
                case PlayerIndex.One:
                    mineAnimation.Load(GameplayScreen.content, "Animations/BlueMine", 2, 1);
                    break;
                case PlayerIndex.Two:
                    mineAnimation.Load(GameplayScreen.content, "Animations/GreenMine", 2, 1);
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            mineAnimation.Rotation = Rotation;
            // Reset the timers
            tta = TTA;
            ttl = TTL;
            setSpriteByString(determineSprite(_index)); // Load the right sprite
            damage = 1; // Remember to do damage
            Collider.Active = false; // Reset the collider to off
            ReadyToRemove = false; // No longer ready to remove
        }

        // Called when a collision has occurred or the ttl has run out
        public override void onCollide(SceneObject collide)
        {
            ReadyToRemove = true; // Be prepped to be removed

            Collider.Active = false; // Only collide once

            // Create particles of your image
            ParticleGenerator gen = new ParticleGenerator(Position, Velocity, Rotation, Sprite);
            GameplayScreen.GameInstance.CurrentScene.addSceneObject(gen); // And add it to the scene

            SoundManager.playMinePop(); // Play the sound

            // Base method
            base.onCollide(collide);
        }

        // Called when removed from the scene
        public override void onRemove()
        {
            // Update the score box and player to allow for more mines
            GameplayScreen.GameInstance.getPlayer(index).PlayerShip.decreaseMinesLayed();
            WeaponManager.addMine(this); // Readd yourself to the list of recycled mines
            base.onRemove(); // Base method
        }

        // Overriden normal method; no one should bounce off a mine
        public override Vector2 ReflectionNormal(Vector2 heading)
        {
            return Vector2.Zero; // So return zero
        }
    }
}
