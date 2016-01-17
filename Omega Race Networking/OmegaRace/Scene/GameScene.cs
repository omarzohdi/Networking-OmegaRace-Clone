using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace OmegaRace
{
    // The game scene where the actual game is played
    public class GameScene : Scene
    {
        // The background image
        private Texture2D backgroundTexture;

        // The box in the middle for drawing various things including score
        private ScoreBox scoreBox;

        // The current state of the game
        public enum gameState
        {
            ready, // Flashes Ready? until the timer is up
            game, // The main game mode
            winner // Displays the winner
        };
        private gameState currentState;
        public gameState CurrentState
        {
            get { return currentState; }
        }

        // The index of the winning player in winner state
        private PlayerIndex winner;
        public PlayerIndex Winner
        {
            get { return winner; }
        }

        // Members for the ready state
        private const float READY_STATE_TIMER = 1.5f; // Timer for how long to stay in the ready state
        private float readyStateTimer = READY_STATE_TIMER;

        // Members for the winner state
        private const float WINNER_STATE_TIMER = 1.0f; // Timer for how long to stay in the winner state
        private float winnerStateTimer = WINNER_STATE_TIMER;

        private CollisionRect innerBounds; // Stay out of
        public bool outOfInnerBounds(CollisionCircle shipSphere) { return (innerBounds.isColliding(shipSphere)); }
        private CollisionRect outerBounds; // Stay in
        public bool inOuterBounds(CollisionCircle shipSphere) { return (outerBounds.isColliding(shipSphere)); }

        // Respawn zones, spawn the player in the opposite zone
        // If top then bottom; if bottom, then top
        private CollisionRect topZone;
        private CollisionRect bottomZone;

        // Respawn points
        private Vector2 topSpawnPoint;
        private float topSpawnRotation;
        private Vector2 bottomSpawnPoint;
        private float bottomSpawnRotation;

        // The position for the volume and its textures
        private Vector2 volumePosition;
        private Texture2D unmutedTexture;
        private Texture2D mutedTexture;

        private const float OUT_OF_BOUNDS_EXTRA = 20.0f;

        // Constructor; loads the background and sets the players to the game state
        public GameScene()
            : base()
        {
            // Load background
            backgroundTexture = GameplayScreen.content.Load<Texture2D>("Background");

            // Set the players' modes
            GameplayScreen.GameInstance.Localplayer.CurrentMode = Player.Mode.Game;
            GameplayScreen.GameInstance.Remoteplayer.CurrentMode = Player.Mode.Game;

            innerBounds = new CollisionRect(new Vector2(235, 172), new Vector2(560, 308));
            outerBounds = new CollisionRect(new Vector2(35, 35), new Vector2(765, 445));

            topZone = new CollisionRect(new Vector2(0, 0), new Vector2(800, 162));
            bottomZone = new CollisionRect(new Vector2(0, 316), new Vector2(800, 480));

            topSpawnPoint = new Vector2(400, 120);
            topSpawnRotation = MathHelper.ToRadians(0);
            bottomSpawnPoint = new Vector2(400, 360);
            bottomSpawnRotation = MathHelper.ToRadians(180);
        }



        // Set up all the scene objects in the game
        public override void Initialize()
        {
            // Base method first
            base.Initialize();

            // Set up the weapon manager's lists
            WeaponManager.Initialize();

            // Create ships for the players
            //bottomSpawnPoint
            //    topSpawnPoint;

            GameplayScreen.GameInstance.Localplayer.PlayerShip = new Ship(GameplayScreen.GameInstance.Localplayer.Number);
            GameplayScreen.GameInstance.Remoteplayer.PlayerShip = new Ship(GameplayScreen.GameInstance.Remoteplayer.Number);
          
            // Player ship updating and drawing is handled by the player so they occur last (so as to draw over everything [particularly mines])

            volumePosition = new Vector2(745, 29);

            // Adds the players' ships to the scene
            addSceneObject(GameplayScreen.GameInstance.Localplayer.PlayerShip);
            addSceneObject(GameplayScreen.GameInstance.Remoteplayer.PlayerShip);

            // Fence Posts
            addSceneObject(new FencePost(new Vector2(17, 17)));
            addSceneObject(new FencePost(new Vector2(240, 17)));
            addSceneObject(new FencePost(new Vector2(560, 17)));
            addSceneObject(new FencePost(new Vector2(783, 17)));
            addSceneObject(new FencePost(new Vector2(17, 240)));
            addSceneObject(new FencePost(new Vector2(783, 240)));
            addSceneObject(new FencePost(new Vector2(17, 463)));
            addSceneObject(new FencePost(new Vector2(240, 463)));
            addSceneObject(new FencePost(new Vector2(560, 463)));
            addSceneObject(new FencePost(new Vector2(783, 463)));
            addSceneObject(new FencePost(new Vector2(240, 170)));
            addSceneObject(new FencePost(new Vector2(560, 170)));
            addSceneObject(new FencePost(new Vector2(560, 310)));
            addSceneObject(new FencePost(new Vector2(240, 310)));

            // Vertical fences
            addSceneObject(new Fence(new Vector2(14, 24), new Vector2(14, 24 + 209), true));
            addSceneObject(new Fence(new Vector2(14, 247), new Vector2(14, 247 + 209), true));
            addSceneObject(new Fence(new Vector2(780, 24), new Vector2(780, 24 + 209), true));
            addSceneObject(new Fence(new Vector2(780, 247), new Vector2(780, 247 + 209), true));
            addSceneObject(new Fence(new Vector2(237, 177), new Vector2(237, 177 + 126), false));
            addSceneObject(new Fence(new Vector2(557, 177), new Vector2(557, 177 + 126), false));

            // Horizontal fences
            addSceneObject(new Fence(new Vector2(24, 14), new Vector2(24 + 209, 14), false));
            addSceneObject(new Fence(new Vector2(24, 460), new Vector2(24 + 209, 460), false));
            addSceneObject(new Fence(new Vector2(567, 14), new Vector2(567 + 209, 14), false));
            addSceneObject(new Fence(new Vector2(567, 460), new Vector2(567 + 209, 460), false));
            addSceneObject(new Fence(new Vector2(247, 14), new Vector2(247 + 309, 14), true));
            addSceneObject(new Fence(new Vector2(247, 460), new Vector2(247 + 309, 460), true));
            addSceneObject(new Fence(new Vector2(247, 167), new Vector2(247 + 309, 167), true));
            addSceneObject(new Fence(new Vector2(247, 307), new Vector2(247 + 309, 307), true));

            // Create a new score box
            scoreBox = new ScoreBox(this);

            // Textures for muting and unmuting in pause
            unmutedTexture = GameplayScreen.content.Load<Texture2D>("soundOn");
            mutedTexture = GameplayScreen.content.Load<Texture2D>("soundOff");
        }

        // Update by calling the right update method based on the current state
        public override void Update(float deltaTime)
        {
            //GameData LocalGameData = new GameData(currentState, false);
            //NetworkManager.AddtoQueue(LocalGameData);

            switch (currentState)
            {
                case gameState.ready:
                    base.Update(deltaTime);
                    UpdateReady(deltaTime);
                    break;
                case gameState.game:
                    base.Update(deltaTime);
                    UpdateGame(deltaTime);
                    break;
                case gameState.winner:
                    UpdateWinner(deltaTime);
                    break;
            }
        }

        // Called during ready state, updates the timer and score box
        public void UpdateReady(float deltaTime)
        {
            // Reduce the time to stay in the state
            readyStateTimer -= deltaTime;
            if (readyStateTimer <= 0) // When the state is complete, move to game state
            {
                currentState = gameState.game;
                readyStateTimer = READY_STATE_TIMER; // Reset the timer
            }
             
            // Update the score box
            scoreBox.Update(deltaTime);
        }

        public void checkPlayerRespawn(float deltaTime)
        {
            PlayerData Remoteplayer = NetworkManager.GetPlayerData();
            if (Remoteplayer != null)
                GameplayScreen.GameInstance.Remoteplayer.setPlayerData(Remoteplayer);

            if (GameplayScreen.GameInstance.Localplayer.PlayerShip.Position.Y > GameplayScreen.GameInstance.ScreenManager.GraphicsDevice.Viewport.Height / 2)
            {
                GameplayScreen.GameInstance.Remoteplayer.setRespawnPoint(topSpawnPoint, topSpawnRotation);
            }
            else
            {
                GameplayScreen.GameInstance.Remoteplayer.setRespawnPoint(bottomSpawnPoint, bottomSpawnRotation);
            }

            if (GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Position.Y > GameplayScreen.GameInstance.ScreenManager.GraphicsDevice.Viewport.Height / 2)
            {
                GameplayScreen.GameInstance.Localplayer.setRespawnPoint(topSpawnPoint, topSpawnRotation);
            }
            else
            {
                GameplayScreen.GameInstance.Localplayer.setRespawnPoint(bottomSpawnPoint, bottomSpawnRotation);
            }
        }

        public void checkPlayerDeath()
        {
            PlayerData Remoteplayer = NetworkManager.GetPlayerData();
            if (Remoteplayer != null)
                GameplayScreen.GameInstance.Remoteplayer.setPlayerData(Remoteplayer);


            if (GameplayScreen.GameInstance.Localplayer.Lives <= 0) // If player 1 is out of lives
            {
                // Player 2 has won
                if (NetworkManager.isApplicationHost())
                    winner = PlayerIndex.Two; // Set them as the winner
                else
                    winner = PlayerIndex.One;

                GameplayScreen.GameInstance.Remoteplayer.increaseWins(); // Increase their wins
                currentState = gameState.winner; // Move to the next state

            }
            else if (GameplayScreen.GameInstance.Remoteplayer.Lives <= 0) // If player 2 is out of lives
            {
                // Player 1 has won
                if (NetworkManager.isApplicationHost())
                    winner = PlayerIndex.One; // Set them as the winner
                else
                    winner = PlayerIndex.Two;

                GameplayScreen.GameInstance.Localplayer.increaseWins(); // Increase their wins
                currentState = gameState.winner; // Move to the next state

            }

            PlayerData Localplayer = new PlayerData(GameplayScreen.GameInstance.Localplayer);
            NetworkManager.AddtoQueue(Localplayer);

            //GameData LocalGameData = new GameData(currentState, false);
            //NetworkManager.AddtoQueue(LocalGameData);
        }

        // Called during game state, updates the players, collisions, and score box
        public void UpdateGame(float deltaTime)
        {
            // Update the players first
            
            GameplayScreen.GameInstance.Localplayer.Update(deltaTime);
            GameplayScreen.GameInstance.Remoteplayer.Update(deltaTime);

            // Update the score box
            scoreBox.Update(deltaTime);

            // Check where the players are in case we need to respawn after colliding
            // We'll use half the screen height to decide
            checkPlayerRespawn(deltaTime);

            // Check everyone's collisions
            checkCollisions();

            // Check the players are in the field
            checkPlayersBounded();

            // Check for dead players
            checkPlayerDeath();
        }

        // Called during winner state, updates the timer and score box
        public void UpdateWinner(float deltaTime)
        {
            // Update the score box
            scoreBox.Update(deltaTime);

            // Decrease the timer
            winnerStateTimer -= deltaTime;
            // Wait until the timer is up
            if (winnerStateTimer <= 0)
            {
                // The state doesn't end until a confirmation key is pressed
                if(InputManager.isConfirmationKeyPressed())
                {
                    GameData LocalGameData = new GameData(currentState, true);
                    NetworkManager.AddtoQueue(LocalGameData);
                    GameplayScreen.LocalPlayerWinConfirm = true;
                }
                if (GameplayScreen.RemotePlayerWinConfirm && GameplayScreen.LocalPlayerWinConfirm)
                {
                    // Move to the ready state
                    currentState = gameState.ready;
                    GameplayScreen.RemotePlayerWinConfirm = false;
                    GameplayScreen.RemotePlayerWinConfirm = false;
                    Reset(); // Reset everything
                }
                 
            }
             
        }

        // Draws the background and the score box
        public override void Draw()
        {
            SpriteBatch spriteBatch = GameplayScreen.GameInstance.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Vector2(), Color.White); // Draw the background
            spriteBatch.End();

            // Draw all the items in the scene
            base.Draw();

            // Draw the score box on top
            scoreBox.Draw();

            // If we're paused, we need to draw the muting graphics
            if (IsPaused)
            {
                SpriteBatch batch = GameplayScreen.GameInstance.SpriteBatch;
                batch.Begin();
                // Draw the correct sound image
                if (SoundManager.IsMuted)
                {
                    batch.Draw(mutedTexture, volumePosition, Color.White);
                }
                else
                {
                    batch.Draw(unmutedTexture, volumePosition, Color.White);
                }
                batch.End();
            }
             
        }

        // Gets the next scene, which is the main menu, but this scene never actually ends
        public override GameplayScreen.sceneEnum nextScene()
        {
            return GameplayScreen.sceneEnum.MainMenuScene;
        }

        // Checks everything's collision against everything else
        public void checkCollisions()
        {
            // Create a copy of the list to compare against
            SceneObject[] copy = new SceneObject[sceneObjects.Count];
            sceneObjects.CopyTo(copy); // Copy everything into it
            // Iterate over every object and look for collisions
            foreach (SceneObject item in sceneObjects) // The real list
            {
                for (int i = 0; i < sceneObjects.Count; i++) // The copy list
                {
                    if (item != copy[i]) // Make sure we're not checking collisions against yourself
                    {
                        if (!((item is Fence && copy[i] is FencePost)
                            || (item is FencePost && copy[i] is Fence))) // Fences and FencePosts should not collide with each other
                        {
                            if (item.isColliding(copy[i])) // Otherwise check for a collision
                            {
                                if (checkShipToLaser(item, copy[i])) // Short-hand method for making sure a ship isn't colliding with its own laser
                                {
                                    // Collide on both items
                                    item.onCollide(copy[i]);
                                    copy[i].onCollide(item);
                                }
                                 
                            }
                             
                        }
                         
                    }
                     
                }
            }
        }

        // A check to make sure the players are in the screen, but not in the box
        private void checkPlayersBounded()
        {
            PlayerData Remoteplayer = NetworkManager.GetPlayerData();
            if (Remoteplayer !=null)
                GameplayScreen.GameInstance.Remoteplayer.setPlayerData(Remoteplayer);

            // Make sure the player is not in the center
            if (GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.isColliding(innerBounds))
            {
                // Move the collider back
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.Position = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition;
                // Find the normal of the collision
                Vector2 rNormal = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition - GameplayScreen.GameInstance.Localplayer.PlayerShip.Position;
                Vector2 position = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition;
                if (rNormal.Length() != 0) // If the normal is not length zero,
                {
                    rNormal.Normalize(); // Normalize it
                    while (innerBounds.isColliding(GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider)) // As long as we are still colliding
                    {
                        position += rNormal; // Move back along the normal
                        GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.Position = position; // Move the collider with
                    }
                }
                 
                position += rNormal * OUT_OF_BOUNDS_EXTRA; // Just to make sure they aren't trapped
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Velocity = Vector2.Zero; // So we don't get trapped
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.Position = position; // And move the collider with
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Position = position;
            }
            // Make sure the player is not outside the screen
            else if (!GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.isColliding(outerBounds))
            {
                Vector2 rNormal = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition - GameplayScreen.GameInstance.Localplayer.PlayerShip.Position;
                Vector2 position = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition;
                if (rNormal.Length() != 0) // If the normal is not length zero,
                {
                    rNormal.Normalize(); // Normalize it
                    while (!GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.isColliding(outerBounds)) // As long as we are still colliding
                    {
                        position += rNormal; // Move back along the normal
                        GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.Position = position; // Move the collider with
                    }
                }
                 
                position += rNormal * OUT_OF_BOUNDS_EXTRA; // Just to make sure they aren't trapped
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Velocity = Vector2.Zero; // So we don't get trapped
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Collider.Position = position; // And move the collider with
                GameplayScreen.GameInstance.Localplayer.PlayerShip.Position = position;
            }
             

            // Make sure the player is not in the center
            if (GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.isColliding(innerBounds))
            {
                // Move the collider back
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.Position = GameplayScreen.GameInstance.Remoteplayer.PlayerShip.LastPosition;
                // Find the normal of the collision
                Vector2 rNormal = GameplayScreen.GameInstance.Remoteplayer.PlayerShip.LastPosition - GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Position;
                Vector2 position = GameplayScreen.GameInstance.Remoteplayer.PlayerShip.LastPosition;
                if (rNormal.Length() != 0) // If the normal is not length zero,
                {
                    rNormal.Normalize(); // Normalize it
                    while (innerBounds.isColliding(GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider)) // As long as we are still colliding
                    {
                        position += rNormal; // Move back along the normal
                        GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.Position = position; // Move the collider with
                    }
                }
                 
                position += rNormal * OUT_OF_BOUNDS_EXTRA; // Just to make sure they aren't trapped
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Velocity = Vector2.Zero; // So we don't get trapped
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.Position = position; // And move the collider with
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Position = position;
            }
            // Make sure the player is not outside the screen
            else if (!GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.isColliding(outerBounds))
            {
                Vector2 rNormal = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition - GameplayScreen.GameInstance.Localplayer.PlayerShip.Position;
                Vector2 position = GameplayScreen.GameInstance.Localplayer.PlayerShip.LastPosition;
                if (rNormal.Length() != 0) // If the normal is not length zero,
                {
                    rNormal.Normalize(); // Normalize it
                    while (!GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.isColliding(outerBounds)) // As long as we are still colliding
                    {
                        position += rNormal; // Move back along the normal
                        GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.Position = position; // Move the collider with
                    }
                }
                 
                position += rNormal * OUT_OF_BOUNDS_EXTRA; // Just to make sure they aren't trapped
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Velocity = Vector2.Zero; // So we don't get trapped
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Collider.Position = position; // And move the collider with
                GameplayScreen.GameInstance.Remoteplayer.PlayerShip.Position = position;
            }
             
        }

        // Short-hand method to check if a laser is colliding with its own ship or not
        private bool checkShipToLaser(SceneObject a, SceneObject b)
        {
            return (!(
                ((a is Laser && b is Ship) &&
                    ((Laser)a).Index == ((Ship)b).Index)
                || ((b is Laser && a is Ship) &&
                    ((Laser)b).Index == ((Ship)a).Index)));
        }

        // Clears all the objects and resets all the numbers and positions
        private void Reset()
        {
            ClearObjects(); // Clear everything off the lists and screen
            // Reset the players
            GameplayScreen.GameInstance.Localplayer.PlayerShip.IsDead = false;
            GameplayScreen.GameInstance.Localplayer.Lives = Player.MAX_LIVES;
            GameplayScreen.GameInstance.Localplayer.Reset();
            GameplayScreen.GameInstance.Remoteplayer.PlayerShip.IsDead = false;
            GameplayScreen.GameInstance.Remoteplayer.Lives = Player.MAX_LIVES;
            GameplayScreen.GameInstance.Remoteplayer.Reset();
            // Reset the timers
            winnerStateTimer = WINNER_STATE_TIMER;
            readyStateTimer = READY_STATE_TIMER;
        }

        // Removes all lasers, mines, and particles from the scene
        private void ClearObjects()
        {
            foreach (SceneObject item in sceneObjects)
            {
                if (item is Laser || item is Mine || item is ParticleGenerator)
                {
                    item.ReadyToRemove = true;
                }
                 
            }
        }
    }
}
