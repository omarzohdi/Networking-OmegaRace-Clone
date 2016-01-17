using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Net;

namespace OmegaRace
{
    public static class NetworkManager
    {
        private static PacketWriter packetWriter;
        private static PacketReader packetReader;
        private static bool isHost;

        private static System.Collections.Generic.Queue<PlayerData> playerQueue_sen;
        private static System.Collections.Generic.Queue<ShipData> shipQueue_sen;
        private static System.Collections.Generic.Queue<WeaponData> weaponQueue_sen;
        private static System.Collections.Generic.Queue<GameData> gameQueue_sen;

        private static System.Collections.Generic.Queue<PlayerData> playerQueue_rec;
        private static System.Collections.Generic.Queue<ShipData> shipQueue_rec;
        private static System.Collections.Generic.Queue<WeaponData> weaponQueue_rec;
        private static System.Collections.Generic.Queue<GameData> gameQueue_rec;


        public static void Initialize()
        {
            if (GameplayScreen.networkSession != null)
                isHost = GameplayScreen.networkSession.IsHost;
            else
                isHost = true;

            playerQueue_sen = new System.Collections.Generic.Queue<PlayerData>();
            shipQueue_sen = new System.Collections.Generic.Queue<ShipData>();
            weaponQueue_sen = new System.Collections.Generic.Queue<WeaponData>();
            gameQueue_sen = new System.Collections.Generic.Queue<GameData>();

            playerQueue_rec = new System.Collections.Generic.Queue<PlayerData>();
            shipQueue_rec = new System.Collections.Generic.Queue<ShipData>();
            weaponQueue_rec = new System.Collections.Generic.Queue<WeaponData>();
            gameQueue_rec = new System.Collections.Generic.Queue<GameData>();

            packetWriter = new PacketWriter();
            packetReader = new PacketReader();
        }

        private static void writePlayerData()
        {
            int playerCount = playerQueue_sen.Count;

            packetWriter.Write(playerCount);

            for (int i = 0; i < playerCount; i++)
            {
                PlayerData tempPlayerData = playerQueue_sen.Dequeue();

                packetWriter.Write(tempPlayerData.Lives);
                packetWriter.Write(tempPlayerData.Death);
                packetWriter.Write(tempPlayerData.Kills);
                packetWriter.Write(tempPlayerData.Score);
                packetWriter.Write(tempPlayerData.Killed);
            }
        }
        private static void writeShipData()
        {
            int shipCount = shipQueue_sen.Count;

            packetWriter.Write(shipCount);

            for (int i = 0; i < shipCount; i++)
            {

                ShipData tempShipData = shipQueue_sen.Dequeue();

                packetWriter.Write(tempShipData.Pos);
                packetWriter.Write(tempShipData.Vel);
                packetWriter.Write(tempShipData.Rot);
                packetWriter.Write(tempShipData.minesDown);
                packetWriter.Write(tempShipData.Boost);
            }
        }

        private static void writeWeaponData()
        {
            int laserCount = weaponQueue_sen.Count;

            packetWriter.Write(laserCount);

            for (int i = 0; i < laserCount; i++)
            {
                WeaponData tempLaserData = weaponQueue_sen.Dequeue();

                packetWriter.Write(tempLaserData.isFired);
                packetWriter.Write(tempLaserData.isMined);
            }
        }
        private static void writeGameData()
        {
            int gameCount = gameQueue_sen.Count;

            packetWriter.Write(gameCount);

            for (int i = 0; i < gameCount; i++)
            {
                GameData tempGameData = gameQueue_sen.Dequeue();

                packetWriter.Write(tempGameData.gamestate);
                packetWriter.Write(tempGameData.winConfirm);

            }
        }
        private static void WriteData()
        {
            writeGameData();
            writePlayerData();
            writeShipData();
            writeWeaponData();
        }

        private static bool SendData(NetworkSession networkSession)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ///Write Player Data, Ship Position, Velocity, Rotation, Mines down, Is dead flag.

                WriteData();

                //if (!networkSession.IsHost)
                if (!gamer.IsDisposed)
                    gamer.SendData(packetWriter, SendDataOptions.None);
            }

            return false;
        }

        private static void ReadPlayerData()
        {
            int playerCount = packetReader.ReadInt32();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerData tempPlayerData = new PlayerData();

                tempPlayerData.Lives = packetReader.ReadInt32();
                tempPlayerData.Death = packetReader.ReadInt32();
                tempPlayerData.Kills = packetReader.ReadInt32();
                tempPlayerData.Score = packetReader.ReadInt32();
                tempPlayerData.Killed = packetReader.ReadBoolean();

                playerQueue_rec.Enqueue(tempPlayerData);
            }
        }
        private static void ReadShipData()
        {
            int shipCount = packetReader.ReadInt32();

            for (int i = 0; i < shipCount; i++)
            {
                ShipData tempShipData = new ShipData();

                tempShipData.Pos = packetReader.ReadVector2();
                tempShipData.Vel = packetReader.ReadVector2();
                tempShipData.Rot = packetReader.ReadSingle();
                tempShipData.minesDown = packetReader.ReadInt32();
                tempShipData.Boost = packetReader.ReadBoolean();

                shipQueue_rec.Enqueue(tempShipData);
            }
        }

        private static void ReadWeaponData()
        {
            int laserCount = packetReader.ReadInt32();

            for (int i = 0; i < laserCount; i++)
            {
                WeaponData tempLaserData = new WeaponData();

                tempLaserData.isFired = packetReader.ReadBoolean();
                tempLaserData.isMined = packetReader.ReadBoolean();

                weaponQueue_rec.Enqueue(tempLaserData);
            }
        }
        private static void ReadGameData()
        {
            int gameCount = packetReader.ReadInt32();

            for (int i = 0; i < gameCount; i++)
            {
                GameData tempGameData = new GameData();

                tempGameData.gamestate = packetReader.ReadInt32();
                tempGameData.winConfirm = packetReader.ReadBoolean();

                gameQueue_rec.Enqueue(tempGameData);
            }
        }

        private static void ReadData()
        {
            ReadGameData();
            ReadPlayerData();
            ReadShipData();
            ReadWeaponData();
        }
        private static bool ReceiveData(NetworkSession networkSession)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                // Keep reading while packets are available.
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;
                    // Read a single packet.
                    gamer.ReceiveData(packetReader, out sender);

                    if (!sender.IsLocal)
                    {
                        ReadData();
                    }
                }
            }

            return false;
        }

        public static bool isApplicationHost()
        {
            return isHost;
        }

        public static PlayerData GetPlayerData()
        {
            PlayerData Pdata = new PlayerData();

            if (playerQueue_rec.Count > 0)
            {
                Pdata = playerQueue_rec.Dequeue();
                return Pdata;
            }

            return null;
        }

        public static ShipData GetShipData()
        {
            ShipData Pdata = new ShipData();

            if (shipQueue_rec.Count > 0)
            {
                Pdata = shipQueue_rec.Dequeue();
                return Pdata;
            }

            return null;
        }

        public static WeaponData GetWeaponData()
        {
            WeaponData Wdata = new WeaponData();

            if (weaponQueue_rec.Count > 0)
            {
                Wdata = weaponQueue_rec.Dequeue();
                return Wdata;
            }

            return null;
        }

        public static GameData GetGameData()
        {
            GameData Gdata = new GameData();

            if (gameQueue_rec.Count > 0)
            {
                Gdata = gameQueue_rec.Dequeue();
                return Gdata;
            }

            return null;
        }


        public static void AddtoQueue(PlayerData data)
        {
            playerQueue_sen.Enqueue(data);

        }
        public static void AddtoQueue(ShipData data)
        {
            shipQueue_sen.Enqueue(data);

        }
        public static void AddtoQueue(WeaponData data)
        {
            weaponQueue_sen.Enqueue(data);

        }
        public static void AddtoQueue(GameData data)
        {
            gameQueue_sen.Enqueue(data);

        }

        public static void PurgeQueues()
        {

            playerQueue_sen.Clear();
            shipQueue_sen.Clear();
            weaponQueue_sen.Clear();
            gameQueue_sen.Clear();

            playerQueue_rec.Clear();
            shipQueue_rec.Clear();
            weaponQueue_rec.Clear();
            gameQueue_rec.Clear();

        }

        public static void UpdateNetworkStatus(NetworkSession networkSession)
        {

            if (networkSession != null)
            {
                SendData(networkSession);
                ReceiveData(networkSession);

            }
        }
    }
}