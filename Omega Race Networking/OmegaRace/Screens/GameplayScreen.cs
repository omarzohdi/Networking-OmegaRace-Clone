#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
#endregion

namespace OmegaRace
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        public static bool RemotePlayerWinConfirm = false;
        public static bool LocalPlayerWinConfirm = false;
        public static NetworkSession networkSession;

        public static ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        Texture2D backgroundTexture;

        float pauseAlpha;

        private const float TimeScale = 1000;
        private SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private static GameplayScreen Game;
        public static GameplayScreen GameInstance
        {
            get { return GameplayScreen.Game; }
        }

        private Player LocalPlayer;
        public Player Localplayer
        {
            get { return LocalPlayer; }
        }
        private Player RemotePlayer;
        public Player Remoteplayer
        {
            get { return RemotePlayer; }
        }

        public Player getPlayer(PlayerIndex index)
        {
            Player rValue = null;

            if (NetworkManager.isApplicationHost())
            {
                if (index == PlayerIndex.One)
                    rValue = Localplayer;
                else
                    rValue = Remoteplayer;
            }
            else
            {
                if (index == PlayerIndex.Two)
                    rValue = Localplayer;
                else
                    rValue = Remoteplayer;
            }

            return rValue;
        }


        public enum sceneEnum
        {
            GameScene,
            MainMenuScene,
            LobbyScene
        };
        private sceneEnum currentSceneEnum;
        private sceneEnum nextSceneEnum;

        private Scene currentScene;
        public Scene CurrentScene
        {
            get { return currentScene; }
        }

         private enum state
        {
            enteringScene, // Scene is brand new
            inScene, // The state the Scene spends most of its time in
            leavingScene // The Scene has just ended / is ending
        };
        private state currentState;

        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            Game = this;
            GameplayScreen.networkSession = networkSession;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Initialize()
        {
            NetworkManager.Initialize();


            if (NetworkManager.isApplicationHost())
            {
                LocalPlayer = new Player(PlayerIndex.One, true);
                RemotePlayer = new Player(PlayerIndex.Two, false);
            }
            else
            {
                LocalPlayer = new Player(PlayerIndex.Two,true);
                RemotePlayer = new Player(PlayerIndex.One,false);
            }

            InputManager.Initialize();
            SoundManager.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");
            backgroundTexture = content.Load<Texture2D>("background");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            Initialize();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            SoundManager.UnInitialize();
            content.Unload();
        }


        #endregion

        #region Update and Draw

        private void switchScene()
        {
            switch (nextSceneEnum)
            {
                case sceneEnum.GameScene:
                    currentScene = new GameScene();
                    break;
            }
            currentSceneEnum = nextSceneEnum;
            currentScene.Initialize();
            currentState = state.enteringScene;
        }

        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            NetworkManager.UpdateNetworkStatus(networkSession);

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (currentScene == null)
                {
                    switchScene();
                }
                else if (currentState == state.enteringScene || currentState == state.inScene) // Updates the scene
                {
                    // All things in milliseconds x the scale
                    currentScene.Update(gameTime.ElapsedGameTime.Milliseconds / TimeScale);
                    currentState = state.inScene; // If it was in entering, now it is in inScene

                    // If the scene is ready to exit, prepare the next scene
                    if (currentScene.ReadyToExit)
                    {
                        currentState = state.leavingScene;
                    }
                     
                }
                else if (currentState == state.leavingScene) // Closes the scene and gets the next one
                {
                    currentScene.Exit(); // Close the scene
                    nextSceneEnum = currentScene.nextScene(); // Get the next scene
                    // Set up the next scene based on the events of the previous screen
                    switchScene();
                }
            }

            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, true, null,
                                       new BackgroundScreen(),
                                       new LobbyScreen(networkSession));
                }
            }

            GetQueueNetworkData();
            NetworkManager.UpdateNetworkStatus(networkSession);
            InputManager.FlushInput();
        }

        public void GetQueueNetworkData()
        {
            PlayerData tRemotePlayer =  NetworkManager.GetPlayerData();
            if (tRemotePlayer != null)
                RemotePlayer.setPlayerData(tRemotePlayer);

            ShipData tRemoteShip = NetworkManager.GetShipData();
            if (tRemoteShip != null)
                RemotePlayer.PlayerShip.setData(tRemoteShip);

            WeaponData tRemoteWeapon = NetworkManager.GetWeaponData();
            
            if (tRemoteWeapon != null)
            {
                if (tRemoteWeapon.isFired)
                    RemotePlayer.PlayerShip.fireLaser();
                if (tRemoteWeapon.isMined)
                    RemotePlayer.PlayerShip.layMine();
            }

            GameData tRemoteGameData = NetworkManager.GetGameData();
            if (tRemoteGameData != null)
            {
                int winnerstatenumber = tRemoteGameData.getGameStateNumber(GameScene.gameState.winner);

                if (tRemoteGameData.gamestate == winnerstatenumber && tRemoteGameData.winConfirm)
                    RemotePlayerWinConfirm = true;
                else
                    RemotePlayerWinConfirm = false;
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                // local players who are participating in the session.
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
            }
        }


        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);
                return false;
            }

            // Otherwise move the player position.
            Vector2 movement = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;

            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;

            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;

            Vector2 thumbstick = gamePadState.ThumbSticks.Left;

            movement.X += thumbstick.X;
            movement.Y -= thumbstick.Y;

            if (movement.Length() > 1)
                movement.Normalize();

            playerPosition += movement * 2;

            return true;
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);


            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            // Our player and enemy are both actually just text strings.

            spriteBatch.Begin();


            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            if (networkSession != null)
            {
                string message = "Players: " + networkSession.AllGamers.Count;
                Vector2 messagePosition = new Vector2(100, 480);
                spriteBatch.DrawString(gameFont, message, messagePosition, Color.White);
            }

            spriteBatch.End();

            if (currentScene != null)
                currentScene.Draw();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            base.Draw(gameTime);
        }


        #endregion
    }
}
