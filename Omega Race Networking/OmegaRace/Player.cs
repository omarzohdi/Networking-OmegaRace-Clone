using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OmegaRace
{
    // The Player is thought more of as the one holding the controller than the ship,
    // Ship handles all the things pertaining to the ship, but Players exist in between screens
    public class Player
    {
        bool islocal;
        // The number for the player, which determines their ship; as distinct from controller index
        private PlayerIndex number;
        public PlayerIndex Number
        {
            get { return number; }
        }
        // No setter

        public const int MAX_LIVES = 3; // Max lives per match

        // Records the number of kills the player has made across all matches
        private int kills = 0;
        public int Kills
        {
            get { return kills; }
            set { kills = value; }
        }
        public int increaseKills() { return ++kills; }

        // Records the number of deaths the player has suffered across all matches
        private int deaths = 0;
        public int Deaths
        {
            get { return deaths; }
            set { deaths = value; }
        }

        // Records the number of wins the player has accrued across all matches
        private int wins = 0;
        public int Wins
        {
            get { return wins; }
            set { wins = value; }
        }
        public int increaseWins() { return ++wins; }

        // Number of lives remaining in the match and corresponding getters, setters, and modifiers
        private int lives;
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        public int decreaseLives() { return --lives; }
        public int increaseLives() { return ++lives; }

        // The ship that represents the player in-game
        private Ship playerShip;
        public Ship PlayerShip
        {
            get { return playerShip; }
            set { playerShip = value; }
        }

        // The current scene the player is in, determines how the controls, update, and draw will work
        public enum Mode
        {
            Menu,
            Game
        };
        private Mode mode = Mode.Menu;
        public Mode CurrentMode
        {
            get { return mode; }
            set { mode = value; }
        }

        // For use in respawning
        private Vector2 respawnPoint;
        private float respawnRotation;
        public void setRespawnPoint(Vector2 _respawnPoint, float _respawnRotation)
        {
            respawnPoint = _respawnPoint;
            respawnRotation = _respawnRotation;
        }

        // Constructor for the player
        // Mode defaults to Mode.Menu
        public Player(PlayerIndex _number,bool local)
        {
            number = _number;
            lives = MAX_LIVES;

            this.islocal = local;

           
        }

        public void setPlayerData(PlayerData Remoteplayer)
        {
                this.deaths = Remoteplayer.Death;
                this.lives = Remoteplayer.Lives;
                this.kills = Remoteplayer.Kills;
                this.wins = Remoteplayer.Score;
                this.playerShip.IsDead = Remoteplayer.Killed; 
        }

        // Collects input and passes it to the ship and controls ship respawning
        public void Update(float deltaTime)
        {
                if (mode == Mode.Game && playerShip != null && islocal)
                {
                    getPlayerGameInput(deltaTime);

                    PlayerData Localplayer = new PlayerData(this);
                    NetworkManager.AddtoQueue(Localplayer);

                    if (playerShip.IsDead)
                    {
                        deaths++;
                        lives--;

                        if (lives > 0)
                        {
                            playerShip.IsDead = false;
                            playerShip.respawn(respawnPoint, respawnRotation);

                            ShipData LocalShip = new ShipData(playerShip);
                            NetworkManager.AddtoQueue(LocalShip);
                        }
                    }
                }
                else
                {
                    getPlayerGameInput(deltaTime);

                    PlayerData Remoteplayer = NetworkManager.GetPlayerData();
                    if (Remoteplayer != null)
                        this.setPlayerData(Remoteplayer);

                    if (playerShip.IsDead)
                    {
                        deaths++;
                        lives--;

                        if (lives > 0)
                        {
                            playerShip.IsDead = false;
                            playerShip.respawn(respawnPoint, respawnRotation);
                        }
                    }                    
                }
        }

        // Does nothing
        public void Draw()
        {
            
        }

        // Get input from the first controller and WASD and Left Ctrl and Left Shift
        private void getPlayerGameInput(float deltaTime)
        {
            if (this.islocal)
            {
                if (!GameplayScreen.GameInstance.CurrentScene.IsPaused)
                {
                    if (InputManager.isKeyVDown()) // Go for the max
                    {
                        playerShip.boost(Ship.MAX_THROTTLE * Ship.THROTLE_MULTI);
                    }
                    else if (InputManager.isRTDown(number))
                    {
                        playerShip.boost(InputManager.rightTriggerVector(number) * Ship.THROTLE_MULTI);
                    }


                    // Do nothing - No breaking

                    if (InputManager.isKeyADown()) 
                    {
                        playerShip.turn(-Ship.MAX_THROTTLE);
                    }
                    else if (InputManager.leftStickXVector(number) < 0)
                    {
                        playerShip.turn(InputManager.leftStickXVector(number));
                    }


                    if (InputManager.isKeyZDown()) 
                    {
                        playerShip.turn(Ship.MAX_THROTTLE);
                    }
                    else if (InputManager.leftStickXVector(number) > 0)
                    {
                        playerShip.turn(InputManager.leftStickXVector(number));
                    }


                    if (InputManager.isKeyXDown() || InputManager.isAPressed(number))
                    {

                        WeaponData LocalWeapon = new WeaponData(true, false);
                        NetworkManager.AddtoQueue(LocalWeapon);

                        playerShip.fireLaser();
                    }


                    if (InputManager.isKeyCDown() || InputManager.isBPressed(number))
                    {
                        WeaponData LocalWeapon = new WeaponData(false, true);
                        NetworkManager.AddtoQueue(LocalWeapon);

                        playerShip.layMine();
                    }

                }


                if ( (InputManager.isKeyTabPressed() || InputManager.isStartPressed(number)) && GameplayScreen.networkSession == null)
                {
                    GameplayScreen.GameInstance.CurrentScene.IsPaused = !GameplayScreen.GameInstance.CurrentScene.IsPaused;
                }
            }
            else if (GameplayScreen.networkSession == null)
            {
                if (!GameplayScreen.GameInstance.CurrentScene.IsPaused)
                {
                    if (InputManager.isKeyBDown()) 
                    {
                        playerShip.boost(Ship.MAX_THROTTLE * Ship.THROTLE_MULTI);
                    }
                    else if (InputManager.isRTDown(number))
                    {
                        playerShip.boost(InputManager.rightTriggerVector(number) * Ship.THROTLE_MULTI);
                    }


                    // Do nothing - No breaking

                    if (InputManager.isKeyLDown())
                    {
                        playerShip.turn(-Ship.MAX_THROTTLE);
                    }
                    else if (InputManager.leftStickXVector(number) < 0)
                    {
                        playerShip.turn(InputManager.leftStickXVector(number));
                    }


                    if (InputManager.isKeyPDown())
                    {
                        playerShip.turn(Ship.MAX_THROTTLE);
                    }
                    else if (InputManager.leftStickXVector(number) > 0)
                    {
                        playerShip.turn(InputManager.leftStickXVector(number));
                    }


                    if (InputManager.isKeyMDown() || InputManager.isAPressed(number))
                    {
                        playerShip.fireLaser();
                    }


                    if (InputManager.isKeyNDown() || InputManager.isBPressed(number))
                    {
                        playerShip.layMine();
                    }

                }


                if ((InputManager.isKeyEnterPressed() || InputManager.isStartPressed(number)) && GameplayScreen.networkSession == null)
                {
                    GameplayScreen.GameInstance.CurrentScene.IsPaused = !GameplayScreen.GameInstance.CurrentScene.IsPaused;
                }

            }
             
        }

        // Resets the ship
        public void Reset()
        {
           playerShip.Reset();
        }
    }
}
