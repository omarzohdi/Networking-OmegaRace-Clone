using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // A player ship
    public class Ship : SceneObject
    {
        // The color and controller for the ship for the color, starting position, controller index, etc
        private PlayerIndex index;
        public PlayerIndex Index
        {
            get { return index; }
        }

        // Top speed
        private const float MAX_VELOCITY = 100.0f;

        // Modifiers for forward movement controls
        public const float MAX_THROTTLE = 1.0f; // For PC
        public const float THROTLE_MULTI = 1.0f;

        // Modifier for the turning speed
        private const float ROTATION_SCALE = 0.1f;

        // How fast the ship drags to a stop
        private const float DRAG_COEFFICIENT = 5.0f;

        // Timer between when a laser is available for firing
        private const float LASER_TIMER = 0.5f;
        private float laserTimer;

        // Timer between when a mine is available for laying
        private const float MINE_TIMER = 0.1f;
        private float mineTimer;

        // Returns if the ship is dead and needs respawning
        private bool isDead;
        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        // Timer during which the ship blinks and is invincible
        private float respawnTimer;
        private const float RESPAWN_TIMER = 1.0f;
        private float blinkTimer;
        // Timer to blink the ship in and out while invincible
        private const float BLINK_TIMER = RESPAWN_TIMER / 9;
        private bool draw = true;

        // Respawn location
        private Vector2 originalPosition;
        private float originalRotation;

        // Used for controlling the boosting animation
        public bool boostUpdate; // For updating
        public bool boostDraw; // For drawing

        // Controls the speed of the boosting animation
        private float ANIMATION_SPEED_UP = 5.0f;

        // The thrust animation
        private AnimatedTextureFiles thrustAnimation;

        // Timer between playing the boosting sound
        private const float SOUND_TIMER = 0.25f;
        private float soundTimer;

        // Counter for the mines currently layed
        private int minesLayed = 0;
        public int MinesLayed
        {
            get { return minesLayed; }
            set { minesLayed = value; }
        }
        public int increaseMinesLayed() { return ++minesLayed; }
        public int decreaseMinesLayed() { return --minesLayed; }

        // Maximum amount of mines the ship can lay at a time
        public const int MINE_LIMIT = 5;

        // This bool is applied after in a collision to avoid
        // unbalanced collisions during momentum transferring
        private bool lockVelocity;

        // Override the velocity to cap our speed
        public override Vector2 Velocity
        {
            set
            {
                if (value.Length() > MAX_VELOCITY) // If it exceeds max velocity
                {
                    // Bring it back to the normal vector and multiple it by the max
                    value.Normalize();
                    value *= MAX_VELOCITY;
                }
                 

                // Use the base setter to prevent loops
                base.Velocity = value;
            }
        }

        // Constructor for the ship, the rotation and sprite are determined by the player index
        // The position is the respawn spawn point
        public Ship(PlayerIndex _index)
            : base(determineRotation(_index), determineSprite(_index))
        {
            if (_index == PlayerIndex.One)
            {
                this.Position = new Vector2(400, 120);
                originalPosition = new Vector2(400, 120);
            }
            else if (_index == PlayerIndex.Two)
            {
                this.Position = new Vector2(400, 360);
                originalPosition = new Vector2(400, 360);
            }

            originalRotation = Rotation;
            index = _index;
            boostDraw = false;
            boostUpdate = false;
            laserTimer = 0;
            mineTimer = 0;
            Collider = new CollisionCircle(originalPosition, 15); // Creates a collision circle of size 15
            damage = 0;
            soundTimer = SOUND_TIMER;
            Collider.Active = true;
            isDead = false;
            draw = true;
            lockVelocity = false;
            thrustAnimation = new AnimatedTextureFiles(Position, Rotation, 1.0f, 0);
            switch (index) // Only One or Two are acceptable indecies
            {
                case PlayerIndex.One:
                    thrustAnimation.Load(GameplayScreen.content, "Animations/BluePlayerShipThrust", 2, 1);
                    break;
                case PlayerIndex.Two:
                    thrustAnimation.Load(GameplayScreen.content, "Animations/GreenPlayerShipThrust", 2, 1);
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }

            //ShipData LocalShip = new ShipData(this);
            //NetworkManager.AddtoQueue(LocalShip);
        }

        // Picks the sprite during construction time based on the index
        private static String determineSprite(PlayerIndex index)
        {
            String rValue = null;
            switch (index)
            {
                case PlayerIndex.One:
                    rValue = "BluePlayerShip";
                    break;
                case PlayerIndex.Two:
                    rValue = "GreenPlayerShip";
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            return rValue;
        }

        // Determines the spawn rotation during construction time based on the index
        private static float determineRotation(PlayerIndex index)
        {
            float rValue = 0;
            switch (index)
            {
                case PlayerIndex.One:
                    rValue = MathHelper.ToRadians(0);
                    break;
                case PlayerIndex.Two:
                    rValue = MathHelper.ToRadians(180);
                    break;
                case PlayerIndex.Three:
                case PlayerIndex.Four:
                    // Crash!
                    break;
            }
            return rValue;
        }


        public bool isBoosting()
        {
            if (this.boostUpdate)
                return true;

            return false;
        }

        public void setData(ShipData RemoteShip)
        {
            this.minesLayed = RemoteShip.minesDown;
            this.Position = RemoteShip.Pos;
            this.Velocity = RemoteShip.Vel;
            this.Rotation = RemoteShip.Rot;

            if (RemoteShip.Boost)
            {
                this.boostUpdate = true;
                this.boostDraw = true;
            }
        }
        // Updates the position, the thrust animation, the blink animation, and the thrust sounds
        public override void Update(float deltaTime)
        {
            NetworkManager.UpdateNetworkStatus(GameplayScreen.networkSession);

            if ((this.index == PlayerIndex.Two && NetworkManager.isApplicationHost()) || (this.index == PlayerIndex.One && !NetworkManager.isApplicationHost()))
            {
                WeaponData RemoteWeapon = NetworkManager.GetWeaponData();

                if (RemoteWeapon != null)
                {
                    if (RemoteWeapon.isFired)
                        this.fireLaser();
                    if (RemoteWeapon.isMined)
                        this.layMine();
                }

                ShipData RemoteShip = NetworkManager.GetShipData();
                if (RemoteShip != null)
                    this.setData(RemoteShip);
            }
            else if ((this.index == PlayerIndex.Two && !NetworkManager.isApplicationHost()) || (this.index == PlayerIndex.One && NetworkManager.isApplicationHost()))
            {
                ShipData LocalShip = new ShipData(this);
                NetworkManager.AddtoQueue(LocalShip);
            }

            // Timer for blinking during respawn
            blinkTimer -= deltaTime;
            if (blinkTimer <= 0)
            {
                draw = !draw; // Swap drawing when the timer is up
                blinkTimer = BLINK_TIMER;
            }
             
            respawnTimer -= deltaTime; // Total invincibilty timer when respawning
            if (respawnTimer <= 0)
            {
                // Reset all the timers and drawing
                draw = true;
                blinkTimer = 0;
                respawnTimer = 0;
            }
             
            drag(deltaTime); // Update the drag

            // Reduce the timers to allow for weapons to be used
            laserTimer -= deltaTime;
            mineTimer -= deltaTime;

            // Reset the lock
            lockVelocity = false;

            // Rotate the animation to the ship
            thrustAnimation.Rotation = Rotation;
            thrustAnimation.Origin = new Vector2(
                        (Sprite.Width / 2) * (float)(Math.Sin(Rotation)),
                        (Sprite.Width / 2) * (float)(-Math.Cos(Rotation)));

            // Update the boosting sounds and animation
            soundTimer -= deltaTime;
            if (boostUpdate) // If the last layer of the drawing guard is up, update the animation
            {
                thrustAnimation.UpdateFrame(deltaTime * ANIMATION_SPEED_UP);
                boostUpdate = false; // Reset boost set until the next thrust
                if (soundTimer <= 0) // If it's time to play the sound again, do so
                {
                    SoundManager.playShipBoost();
                    soundTimer = SOUND_TIMER;
                }
            }
            else // Otherwise reset the animation
            {
                thrustAnimation.Reset();
            }

            // Scene Object Update
            base.Update(deltaTime);

            NetworkManager.UpdateNetworkStatus(GameplayScreen.networkSession);
        }

        // Draws the ship and the thrust animation if on
        public override void Draw()
        {
            if (draw && !isDead) // If it's active and now blinked out while respawning
            {
                base.Draw(); // Draw the ship
                // If it is boosting, draw the thrust animation, too
                if (boostDraw)
                {
                    thrustAnimation.DrawFrame(GameplayScreen.GameInstance.ScreenManager.SpriteBatch, Position - thrustAnimation.Origin);
                }
                 
            }
             
            boostDraw = false; // Reset the bool until the next thrust
        }

        // Called when the player presses forward
        public void boost(float degrees) // How much throttle they give; Note - PC is always max
        {
            // Set the boost animation to on
            boostUpdate = true;
            boostDraw = true;
            // Add thrust to the ship in the thrust direction
            Velocity = Velocity + ((degrees * THROTLE_MULTI) * (new Vector2((float)(Math.Sin(Rotation)), (float)(-Math.Cos(Rotation)))));
        }

        // We drag to a complete stop
        private void drag(float deltaTime)
        {
            // Check if dragging would bring us to the negative direction
            if (Velocity.Length() < DRAG_COEFFICIENT * deltaTime)
            {
                Velocity = Vector2.Zero; // If so, stop
            }
            else
            {
                // Find the inverse, normalize it, and multiple by the drag strength
                Vector2 inverse = Velocity;
                inverse.Normalize();
                inverse *= -1;
                inverse *= DRAG_COEFFICIENT * deltaTime;
                Velocity = Velocity + inverse; // Then add it and set it as the new velocity
            }
        }

        // Turns the ship by positive or negative degrees
        public void turn(float degrees) // How much throttle they give; Note - PC is always max: positive or negative
        {
            Rotation += (degrees * ROTATION_SCALE) % 360; // Multiply by the speed and mod it
        }

        // When the ship collides, this method is called
        public override void onCollide(SceneObject collidee)
        {
            // Gets the damage from the collidee, either 1 or 0
            int damage = collidee.Damage;
            if (respawnTimer <= 0 && damage > 0) // If not invulnerable, and the damage isn't 0,
            {
                // Destroy the ship
                SoundManager.playShipPop(); // Play the sound
                ParticleGenerator gen = new ParticleGenerator(Position, Velocity, Rotation, Sprite); // Make the particles
                GameplayScreen.GameInstance.CurrentScene.addSceneObject(gen); // Add the particle generator to the scene
                isDead = true; // Tell the player the ship has died

                // If the collidee was a laser or a mine and not the player's, the other player has scored a kill
                if (collidee is Laser)
                {
                    if (((Laser)collidee).Index != index) // Make sure it wasn't our own
                    {
                        GameplayScreen.GameInstance.getPlayer(((Laser)collidee).Index).increaseKills(); // The other player has scored a kill
                    }
                     
                }
                else if (collidee is Mine)
                {
                    if (((Mine)collidee).Index != index) // Make sure it wasn't our own
                    {
                        GameplayScreen.GameInstance.getPlayer(((Mine)collidee).Index).increaseKills(); // The other player has scored a kill
                    }
                     
                }
                 
            }
             

            // Bounce the ship (Warning: has some problems when velocity is zero)
            if (collidee is Fence || collidee is FencePost) // The only two static things we can bounce off of
            {
                Velocity = Vector2.Reflect(Velocity, collidee.ReflectionNormal(Velocity));
            }
            else if (!lockVelocity && collidee is Ship) // The last thing we can bounce off of
            {
                // Grab their speeds
                float ship1Speed = Velocity.Length();
                float ship2Speed = collidee.Velocity.Length();

                // And their directions
                Vector2 ship1vel = Velocity;
                Vector2 ship2vel = collidee.Velocity;

                // Check to make sure we're not going to divide by zero
                if (ship2vel.LengthSquared() > 0.0f && ship1Speed > 0.0f)
                {
                    ship2vel.Normalize();
                    Velocity = ship2vel * ship1Speed; // Travel in the direction of the collidee at this speed
                }
                else
                {
                    if (ship2Speed > 0.0f) // Swap speeds, too
                    {
                        Velocity = Vector2.Normalize(ship2vel) * ship2Speed; // Was stationary
                    }
                    else 
                    {
                        Velocity = Vector2.Zero; // Was moving
                    }
                }

                // Check to make sure we're not going to divide by zero
                if (ship1vel.LengthSquared() > 0.0f && ship2Speed > 0.0f)
                {
                    ship1vel.Normalize();
                    collidee.Velocity = ship1vel * ship2Speed; // Travel in the direction of this at the collidee's speed
                }
                else
                {
                    if (ship1Speed > 0.0f)  // Swap speeds, too
                    {
                        collidee.Velocity = Vector2.Normalize(ship1vel) * ship1Speed; // Was stationary
                    }
                    else 
                    {
                        collidee.Velocity = Vector2.Zero; // Was moving
                    }
                }

                // Lock the velocities so this swap only happens once
                lockVelocity = true;
                ((Ship)collidee).lockVelocity = true;
            }
             

            // Scene Object collide
            base.onCollide(collidee);
        }

        // Fires a laser if the timer is up
        public void fireLaser()
        {
            if (laserTimer <= 0) // Make sure you can only fire as fast as the timer
            {
                // Play the sound, update the timer, and spawn a laser
                SoundManager.playFiring();
                WeaponManager.spawnLaser(Position, Rotation, index);
                laserTimer = LASER_TIMER;
            }
             
        }

        // Lays a mine if the timer is up
        public void layMine()
        {
            if (mineTimer <= 0 && minesLayed < MINE_LIMIT) // Make sure you can only lay a mine as fast as the timer and only if you have mines remaining
            {
                // Play the sound, update the timer, and spawn a laser
                SoundManager.playMineLay();
                minesLayed++; // Also increase the mines layed
                WeaponManager.spawnMine(Position, Rotation, index);
                mineTimer = MINE_TIMER;
            }
             
        }

        // Respawns the ship and sets all the drawing and invulnerable timers
        public void respawn(Vector2 spawnPoint, float spawnRotation)
        {
            respawnTimer = RESPAWN_TIMER;
            blinkTimer = BLINK_TIMER;
            soundTimer = SOUND_TIMER;
            draw = false;
            Position = spawnPoint;
            Velocity = Vector2.Zero;
            Rotation = spawnRotation;
        }

        // Resets the ship's position, velocity, and rotation to the spawn values
        public void Reset()
        {
            Position = originalPosition;
            Velocity = Vector2.Zero;
            Rotation = originalRotation;
        }
    }
}
