using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OmegaRaceX.Scenes;

namespace OmegaRaceX.SceneObjects
{
    // The scorebox in the middle of the screen, just draws things
    public class ScoreBox
    {
        // Images to represen the remaining lives
        private Texture2D player1Lives;
        private Texture2D player2Lives;

        // Images to represent the mines placed
        private Texture2D player1Mines;
        private Texture2D player2Mines;

        private Vector2 player1LivesPosition; // The right-most location for player1's lives
        private Vector2 player2LivesPosition; // The left-most location for player2's lives
        private Vector2 player1LivesPositionOffset; // The offset to add to place the next player1 life
        private Vector2 player2LivesPositionOffset; // The offset to add to place the next player2 life

        // The location to write the strings for the kills and deaths
        private Vector2 killsTextPlayer1;
        private Vector2 killsTextPlayer2;
        private Vector2 deathsTextPlayer1;
        private Vector2 deathsTextPlayer2;

        // Offsets for strings with more digits
        private Vector2 killsPositionOffsetP1;
        private Vector2 deathsPositionOffsetP1;

        private Vector2 winsTextPlayer1; // Location to draw player 1's wins
        private Vector2 winsDivider; // Location to draw the : between wins
        private Vector2 winsTextPlayer2; // Location to draw player 2's wins
        private Vector2 winsP1RightAlignOffset; // Offset for strings with more digits

        private Vector2 player1MinesPosition; // The right-most location for player1's remaining mines
        private Vector2 player2MinesPosition; // The left-most location for player2's remaining mines
        private Vector2 player1MinesPositionOffset; // The offset to add to place the next player1 remaining mine
        private Vector2 player2MinesPositionOffset; // The offset to add to place the next player2 remaining mine

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
        private Texture2D player1Wins; // Texture for if player 1 wins
        private Texture2D player2Wins; // Texture for if player 2 wins
        private const float WINNER_BLINK_TIMER = 0.25f; // Timer to blink the winner in and out
        private float winnerBlinkTimer = WINNER_BLINK_TIMER;
        private Vector2 winPosition; // Position for the winner image

        // Pointer back to the GameScene
        private GameScene scene;

        // Constructor, sets up the locations and loads the textures
        public ScoreBox(GameScene _scene)
        {
            // Load the images
            player1Lives = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/BluePlayerShip");
            player2Lives = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/GreenPlayerShip");
            readyTexture = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/readyTexture");
            player1Wins = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/player1Wins");
            player2Wins = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/player2Wins");
            player1Mines = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/BlueMine");
            player2Mines = OmegaRaceXGame.GameInstance.Content.Load<Texture2D>("Textures/GreenMine");

            // Load the font
            font = OmegaRaceXGame.GameInstance.Content.Load<SpriteFont>("Fonts\\DefaultFont");

            // Set all the positions and offsets for the lives and the win and ready texture
            player1LivesPosition = new Vector2(400, 205);
            player2LivesPosition = new Vector2(400, 205);
            player1LivesPositionOffset = new Vector2(35, 0);
            player2LivesPositionOffset = new Vector2(35, 0);
            readyPosition = new Vector2(275, 190);
            winPosition = new Vector2(275, 190);

            // Update the positions
            player1LivesPosition -= player1LivesPositionOffset / 1.5f; // Move the lives over a bit
            player1LivesPosition.X -= player1Lives.Width; // Move the lives over a bit again by the size of the texture
            player2LivesPosition += player2LivesPositionOffset / 1.5f; // Move the lives over a bit

            // Set up the positions for the remaining mines 
            player1MinesPosition = new Vector2(400, 245);
            player2MinesPosition = new Vector2(400, 245);
            player1MinesPositionOffset = new Vector2(15, 0);
            player2MinesPositionOffset = new Vector2(15, 0);

            // Update the positions
            player1MinesPosition -= player1MinesPositionOffset / 3; // Move the mines over a bit 
            player1MinesPosition.X -= player1Mines.Width; // Move the mines over a bit again by the size of the texture
            player2MinesPosition += player1MinesPositionOffset / 3; // Move the mines over a bit

            // Set the position for the kills text
            killsTextPlayer1 = new Vector2(390, 260);
            killsTextPlayer2 = new Vector2(410, 260);

            // Set the position for the deaths text
            deathsTextPlayer1 = new Vector2(390, 275);
            deathsTextPlayer2 = new Vector2(410, 275);

            // Set up the offsets based on a single digit number
            killsPositionOffsetP1 = Vector2.Zero;
            deathsPositionOffsetP1 = Vector2.Zero;
            winsP1RightAlignOffset = Vector2.Zero;

            // Set the text position for the wins and the dividing :
            winsTextPlayer1 = new Vector2(380, 175);
            winsDivider = new Vector2(400, 175); 
            winsTextPlayer2 = new Vector2(415, 175);

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
                else { }
            }
            else { }
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
                else { }
            }
            else { }
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
            else { }
        }

        // Drawn when the game is in Ready state
        public void DrawReady()
        {
            SpriteBatch batch = OmegaRaceXGame.GameInstance.SpriteBatch;

            if (blinkOn) // Draw only when blink is on
            {
                batch.Begin();
                batch.Draw(readyTexture, readyPosition, Color.White);
                batch.End();
            }
            else { }
        }

        // Sets all the positions and draws all the score box information
        // Because it is always left aligned, offsets are made to make things center or right aligned
        public void DrawGame()
        {
            SpriteBatch batch = OmegaRaceXGame.GameInstance.SpriteBatch;

            batch.Begin();

            // Draw each life for player 1, moving left as we do
            for (int i = 0; i < OmegaRaceXGame.GameInstance.Player1.Lives; i++)
            {
                batch.Draw(player1Lives, player1LivesPosition - player1LivesPositionOffset * i, Color.White);
            }

            // Draw each life for player 2, moving right as we do
            for (int i = 0; i < OmegaRaceXGame.GameInstance.Player2.Lives; i++)
            {
                batch.Draw(player2Lives, player2LivesPosition + player2LivesPositionOffset * i, Color.White);
            }

            // Draw each remaining mine for player 1, moving left as we do
            for (int i = 0; i < Ship.MINE_LIMIT - OmegaRaceXGame.GameInstance.Player1.PlayerShip.MinesLayed; i++)
            {
                batch.Draw(player1Mines, player1MinesPosition - player1MinesPositionOffset * i, Color.White);
            }

            // Draw each remaining mine for player 2, moving right as we do
            for (int i = 0; i < Ship.MINE_LIMIT - OmegaRaceXGame.GameInstance.Player2.PlayerShip.MinesLayed; i++)
            {
                batch.Draw(player2Mines, player2MinesPosition + player2MinesPositionOffset * i, Color.White);
            }

            // Set the offset for player 1 kills based on the length of the string
            killsPositionOffsetP1 = font.MeasureString("Kills : " + OmegaRaceXGame.GameInstance.Player1.Kills);
            killsPositionOffsetP1.Y = 0;
            batch.DrawString(font, "Kills : " + OmegaRaceXGame.GameInstance.Player1.Kills,
                killsTextPlayer1-killsPositionOffsetP1,
                Color.White); // and draw it
            // Player 2 won't need an offset, so just draw it
            batch.DrawString(font, OmegaRaceXGame.GameInstance.Player2.Kills + " : Kills", killsTextPlayer2, Color.White);

            // Set the offset for player 1 deaths based on the length of the string
            deathsPositionOffsetP1 = font.MeasureString("Deaths : " + OmegaRaceXGame.GameInstance.Player1.Deaths);
            deathsPositionOffsetP1.Y = 0;
            batch.DrawString(font, "Deaths : " + OmegaRaceXGame.GameInstance.Player1.Deaths, 
                deathsTextPlayer1-deathsPositionOffsetP1,
                Color.White); // and draws it
            // Player 2 won't need an offset
            batch.DrawString(font, OmegaRaceXGame.GameInstance.Player2.Deaths + " : Deaths", deathsTextPlayer2, Color.White);

            // Set the offset for player 1 wins based on the length of the string
            winsP1RightAlignOffset = font.MeasureString("" + OmegaRaceXGame.GameInstance.Player1.Wins);
            winsP1RightAlignOffset.Y = 0;
            batch.DrawString(font, "" + OmegaRaceXGame.GameInstance.Player1.Wins, 
                winsTextPlayer1-winsP1RightAlignOffset,
                Color.White); // and draws it
            // The divider never moves, so we already decided the offset, just draw it
            batch.DrawString(font, ":", winsDivider, Color.White);
            // Player 2 won't need an offset
            batch.DrawString(font, "" + OmegaRaceXGame.GameInstance.Player2.Wins, winsTextPlayer2, Color.White);

            batch.End();
        }

        // Drawn when the game is in Winner state
        public void DrawWinner()
        {
            SpriteBatch batch = OmegaRaceXGame.GameInstance.SpriteBatch;

            if (blinkOnWinner) // Draw only when blink is on
            {
                batch.Begin();
                if (scene.Winner == PlayerIndex.One) // Draw the winner
                {
                    batch.Draw(player1Wins, winPosition, Color.White);
                }
                else
                {
                    batch.Draw(player2Wins, winPosition, Color.White);
                }
                batch.End();
            }
            else { }
        }
    }
}
