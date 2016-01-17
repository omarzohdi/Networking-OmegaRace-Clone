using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmegaRace
{

    public class PlayerData
    {
        public int Lives;
        public int Death;
        public int Kills;
        public int Score;
        public bool Killed;

        public PlayerData()
        {
        }

        public PlayerData(Player player)
        {
            Lives = player.Lives;
            Death = player.Deaths;
            Kills = player.Kills;
            Score = player.Wins;
            Killed = player.PlayerShip.IsDead;
        }

        public void WriteData(Player player)
        {
            Lives = player.Lives;
            Death = player.Deaths;
            Kills = player.Kills;
            Score = player.Wins;
            Killed = player.PlayerShip.IsDead;
        }
    };

    public class ShipData
    {
        public Vector2 Pos;
        public Vector2 Vel;
        public float Rot;
        public int minesDown;
        public bool Boost;

        public ShipData()
        {

        }

        public ShipData(Ship ship)
        {
            Pos = ship.Position;
            Vel = ship.Velocity;
            Rot = ship.Rotation;
            minesDown = ship.MinesLayed;
            Boost = ship.isBoosting();
        }

        public void WriteData(Ship ship)
        {
            Pos = ship.Position;
            Vel = ship.Velocity;
            Rot = ship.Rotation;
            minesDown = ship.MinesLayed;
            Boost = ship.isBoosting();
        }
    };

    public class WeaponData
    {
        public bool isFired;
        public bool isMined;

         public WeaponData()
         {

         }
        public WeaponData(bool FIRE, bool MINE)
        {
            isFired = FIRE;
            isMined = MINE;
        }
    };

    public class GameData
    {
        public int gamestate;
        public bool winConfirm;

        public GameData()
        {

        }
        public int getGameStateNumber(GameScene.gameState GameState)
        {
            switch(GameState)
            {
                case GameScene.gameState.ready:
                    return 1;
                case GameScene.gameState.game:
                    return 2;
                case GameScene.gameState.winner:
                    return 3;

                default:
                    return 0;
            }
        }
        public GameData(GameScene.gameState GameState, bool ConfirmWin)
        {

            gamestate = getGameStateNumber(GameState);
            winConfirm = ConfirmWin;
        }
    };
}
