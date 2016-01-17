using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmegaRace
{
    // The scorebox in the middle of the screen, just draws things
    public class ScoreBox
    {
        // Images to represen the remaining lives
        private Texture2D LocalplayerLives;
        private Texture2D RemoteplayerLives;

        // Images to represent the mines placed
        private Texture2D LocalplayerMines;
        private Texture2D RemoteplayerMines;

        private Vector2 LocalplayerLivesPosition; // The right-most location for Localplayer's lives
        private Vector2 RemoteplayerLivesPosition; // The left-most location for Remoteplayer's lives
        private Vector2 LocalplayerLivesPositionOffset; // The offset to add to place the next Localplayer life
        private Vector2 RemoteplayerLivesPositionOffset; // The offset to add to place the next Remoteplayer life

        // The location to write the strings for the kills and deaths
        private Vector2 killsTextLocalplayer;
        private Vector2 killsTextRemoteplayer;
        private Vector2 deathsTextLocalplayer;
        private Vector2 deathsTextRemoteplayer;

        // Offsets for strings with more digits
        private Vector2 killsPositionOffsetP1;
        private Vector2 deathsPositionOffsetP1;

        private Vector2 winsTextLocalplayer; // Location to draw player 1's wins
        private Vector2 winsDivider; // Location to draw the : between wins
        private Vector2 winsTextRemoteplayer; // Location to draw player 2's wins
        private Vector2 winsP1RightAlignOffset; // Offset for strings with more digits

        private Vector2 LocalplayerMinesPosition; // The right-most location for Localplayer's remaining mines
        private Vector2 RemoteplayerMinesPosition; // The left-most location for Remoteplayer's remaining mines
        private Vector2 LocalplayerMinesPositionOffset; // The offset to add to place the next Localplayer remaining mine
        private Vector2 RemoteplayerMinesPositionOffset; // The offset to add to place the next Remoteplayer remaining mine

        // The font to draw the strings in
        private SpriteFont font;

        // The state of the scorebox and accessors for winners
        private bool displayingWinner; // Drawing the winner or the score
        public bool DisplayingWinner
        {
            get { return displayingWinner; }
        }
        private int winningPlayer; // Which player is the winner to draw
        public int WinningPlayer
        {
            get { return winningPlayer; }
            set
            {
                winningPlayer = value;
                // Also set that we are now displaying a winner
                displayingWinner = true;
            }
        }

        // Members for the Ready state
        private bool blinkOn = false; // Blink bool for flashing the text
        private Texture2D readyTexture; // Texture for the Ready? image
        private const float BLINK_TIMER = 0.25f; // Time between blinking in and out
        private float blinkTimer = BLINK_TIMER; // Set the timer
        private Vector2 readyPosition; // The position for the Ready? image

        // Members for the Winner state
        private bool blinkOnWinner = false; // Blink bool for flashing the text
        private Texture2D LocalplayerWins; // Texture for if player 1 wins
        private Texture2D RemoteplayerWins; // Texture for if player 2 wins
        private const float WINNER_BLINK_TIMER = 0.25f; // Timer to blink the winner in and out
        private float winnerBlinkTimer = WINNER_BLINK_TIMER;
        private Vector2 winPosition; // Position for the winner image

        // Pointer back to the GameScene
        private GameScene scene;

        // Constructor, sets up the locations and loads the textures
        public ScoreBox(GameScene _scene)
        {
            // Load the images

            if (NetworkManager.isApplicationHost())
            {
                LocalplayerLives = GameplayScreen.content.Load<Texture2D>("BluePlayerShip");
                RemoteplayerLives = GameplayScreen.content.Load<Texture2D>("GreenPlayerShip");
                LocalplayerMines = GameplayScreen.content.Load<Texture2D>("BlueMine");
                RemoteplayerMines = GameplayScreen.content.Load<Texture2D>("GreenMine");
                LocalplayerWins = GameplayScreen.content.Load<Texture2D>("player1Wins");
                RemoteplayerWins = GameplayScreen.content.Load<Texture2D>("player2Wins");
            }
            else
            {
                LocalplayerLives = GameplayScreen.content.Load<Texture2D>("GreenPlayerShip");
                RemoteplayerLives = GameplayScreen.content.Load<Texture2D>("BluePlayerShip");
                LocalplayerMines = GameplayScreen.content.Load<Texture2D>("GreenMine");
                RemoteplayerMines = GameplayScreen.content.Load<Texture2D>("BlueMine");
                LocalplayerWins = GameplayScreen.content.Load<Texture2D>("player2Wins");
                RemoteplayerWins = GameplayScreen.content.Load<Texture2D>("player1Wins");
            }

            readyTexture = GameplayScreen.content.Load<Texture2D>("readyTexture");
            
            // Load the font
            font = GameplayScreen.content.Load < SpriteFont > ("Fonts\\DefaultFont");

            // Set all the positions and offsets for the lives and the win and ready texture
            LocalplayerLivesPosition = new Vector2(400, 205);
            RemoteplayerLivesPosition = new Vector2(400, 205);
            LocalplayerLivesPositionOffset = new Vector2(35, 0);
            RemoteplayerLivesPositionOffset = new Vector2(35, 0);
            readyPosition = new Vector2(275, 190);
            winPosition = new Vector2(275, 190);

            // Update the positions
            LocalplayerLivesPosition -= LocalplayerLivesPositionOffset / 1.5f; // Move the lives over a bit
            LocalplayerLivesPosition.X -= LocalplayerLives.Width; // Move the lives over a bit again by the size of the texture
            RemoteplayerLivesPosition += RemoteplayerLivesPositionOffset / 1.5f; // Move the lives over a bit

            // Set up the positions for the remaining mines 
            LocalplayerMinesPosition = new Vector2(400, 245);
            RemoteplayerMinesPosition = new Vector2(400, 245);
            LocalplayerMinesPositionOffset = new Vector2(15, 0);
            RemoteplayerMinesPositionOffset = new Vector2(15, 0);

            // Update the positions
            LocalplayerMinesPosition -= LocalplayerMinesPositionOffset / 3; // Move the mines over a bit 
            LocalplayerMinesPosition.X -= LocalplayerMines.Width; // Move the mines over a bit again by the size of the texture
            RemoteplayerMinesPosition += LocalplayerMinesPositionOffset / 3; // Move the mines over a bit

            // Set the position for the kills text
            killsTextLocalplayer = new Vector2(390, 260);
            killsTextRemoteplayer = new Vector2(410, 260);

            // Set the position for the deaths text
            deathsTextLocalplayer = new Vector2(390, 275);
            deathsTextRemoteplayer = new Vector2(410, 275);

            // Set up the offsets based on a single digit number
            killsPositionOffsetP1 = Vector2.Zero;
            deathsPositionOffsetP1 = Vector2.Zero;
            winsP1RightAlignOffset = Vector2.Zero;

            // Set the text position for the wins and the dividing :
            winsTextLocalplayer = new Vector2(380, 175);
            winsDivider = new Vector2(400, 175);
            winsTextRemoteplayer = new Vector2(415, 175);

            // Recenter the : for the divisor
            Vector2 winsDividerCenter = font.MeasureString(":");
            winsDividerCenter.Y = 0;
            winsDivider -= winsDividerCenter;

            // Set the scene pointer
            scene = _scene;
        }

        // Call the appropriate update based on the game state
        public void Update(float deltaTime)
        {
            // Check the scene's current state and do the right update
            switch (scene.CurrentState)
            {
                case GameScene.gameState.ready:
                    UpdateReady(deltaTime);
                    break;
                case GameScene.gameState.game:
                    UpdateGame(deltaTime);
                    break;
                case GameScene.gameState.winner:
                    UpdateWinner(deltaTime);
                    break;
            }
        }

        // Called when in Ready state, flashes the Ready? text
        private void UpdateReady(float deltaTime)
        {
            // Update the timer
            blinkTimer -= deltaTime;
            if (blinkTimer <= 0)
            {
                blinkOn = !blinkOn; // Swap the blink drawing
                blinkTimer = BLINK_TIMER; // Reset the timer
                if (blinkOn) // Play the sound when blink if turned on
                {
                    SoundManager.playTextBlink();
                }
                 
            }
             
        }

        // No need to update anything
        private void UpdateGame(float deltaTime)
        {

        }

        // Updates the blinking text, called in Winner state
        private void UpdateWinner(float deltaTime)
        {
            // Update the timer
            winnerBlinkTimer -= deltaTime;
            if (winnerBlinkTimer <= 0)
            {
                blinkOnWinner = !blinkOnWinner; // Swap the blink drawing
                winnerBlinkTimer = WINNER_BLINK_TIMER; // Reset the timer
                if (blinkOnWinner) // Play the sound when blink if turned on
                {
                    SoundManager.playTextBlink();
                }
                 
            }
             
        }

        // Draw based on the current state of the scene
        public void Draw()
        {
            if (!scene.IsPaused) // Draw nothing when paused
            {
                // Switch on the scene's current state
                switch (scene.CurrentState)
                {
                    case GameScene.gameState.ready:
                        DrawReady();
                        break;
                    case GameScene.gameState.game:
                        DrawGame();
                        break;
                    case GameScene.gameState.winner:
                        DrawWinner();
                        break;
                }
            }
             
        }

        // Drawn when the game is in Ready state
        public void DrawReady()
        {
            SpriteBatch batch = GameplayScreen.GameInstance.SpriteBatch;

            if (blinkOn) // Draw only when blink is on
            {
                batch.Begin();
                batch.Draw(readyTexture, readyPosition, Color.White);
                batch.End();
            }
             
        }

        // Sets all the positions and draws all the score box information
        // Because it is always left aligned, offsets are made to make things center or right aligned
        public void DrawGame()
        {
            SpriteBatch batch = GameplayScreen.GameInstance.SpriteBatch;

            batch.Begin();

            // Draw each life for player 1, moving left as we do
            for (int i = 0; i < GameplayScreen.GameInstance.Localplayer.Lives; i++)
            {
                batch.Draw(LocalplayerLives, LocalplayerLivesPosition - LocalplayerLivesPositionOffset * i, Color.White);
            }

            // Draw each life for player 2, moving right as we do
            for (int i = 0; i < GameplayScreen.GameInstance.Remoteplayer.Lives; i++)
            {
                batch.Draw(RemoteplayerLives, RemoteplayerLivesPosition + RemoteplayerLivesPositionOffset * i, Color.White);
            }

            // Draw each remaining mine for player 1, moving left as we do
            for (int i = 0; i < Ship.MINE_LIMIT - GameplayScreen.GameInstance.Localplayer.PlayerShip.MinesLayed; i++)
            {
                batch.Draw(LocalplayerMines, LocalplayerMinesPosition - LocalplayerMinesPositionOffset * i, Color.White);
            }

            // Draw each remaining mine for player 2, moving right as we do
            for (int i = 0; i < Ship.MINE_LIMIT - GameplayScreen.GameInstance.Remoteplayer.PlayerShip.MinesLayed; i++)
            {
                batch.Draw(RemoteplayerMines, RemoteplayerMinesPosition + RemoteplayerMinesPositionOffset * i, Color.White);
            }

            // Set the offset for player 1 kills based on the length of the string
            killsPositionOffsetP1 = font.MeasureString("Kills : " + GameplayScreen.GameInstance.Localplayer.Kills);
            killsPositionOffsetP1.Y = 0;
            batch.DrawString(font, "Kills : " + GameplayScreen.GameInstance.Localplayer.Kills,
                killsTextLocalplayer - killsPositionOffsetP1,
                Color.White); // and draw it
            // Player 2 won't need an offset, so just draw it
            batch.DrawString(font, GameplayScreen.GameInstance.Remoteplayer.Kills + " : Kills", killsTextRemoteplayer, Color.White);

            // Set the offset for player 1 deaths based on the length of the string
            deathsPositionOffsetP1 = font.MeasureString("Deaths : " + GameplayScreen.GameInstance.Localplayer.Deaths);
            deathsPositionOffsetP1.Y = 0;
            batch.DrawString(font, "Deaths : " + GameplayScreen.GameInstance.Localplayer.Deaths,
                deathsTextLocalplayer - deathsPositionOffsetP1,
                Color.White); // and draws it
            // Player 2 won't need an offset
            batch.DrawString(font, GameplayScreen.GameInstance.Remoteplayer.Deaths + " : Deaths", deathsTextRemoteplayer, Color.White);

            // Set the offset for player 1 wins based on the length of the string
            winsP1RightAlignOffset = font.MeasureString("" + GameplayScreen.GameInstance.Localplayer.Wins);
            winsP1RightAlignOffset.Y = 0;
            batch.DrawString(font, "" + GameplayScreen.GameInstance.Localplayer.Wins,
                winsTextLocalplayer - winsP1RightAlignOffset,
                Color.White); // and draws it
            // The divider never moves, so we already decided the offset, just draw it
            batch.DrawString(font, ":", winsDivider, Color.White);
            // Player 2 won't need an offset
            batch.DrawString(font, "" + GameplayScreen.GameInstance.Remoteplayer.Wins, winsTextRemoteplayer, Color.White);

            batch.End();
        }

        // Drawn when the game is in Winner state
        public void DrawWinner()
        {
            SpriteBatch batch = GameplayScreen.GameInstance.SpriteBatch;

            if (blinkOnWinner) // Draw only when blink is on
            {
                batch.Begin();
                if (scene.Winner == PlayerIndex.One) // Draw the winner
                {
                    batch.Draw(LocalplayerWins, winPosition, Color.White);
                }
                else
                {
                    batch.Draw(RemoteplayerWins, winPosition, Color.White);
                }
                batch.End();
            }
             
        }
    }
}
