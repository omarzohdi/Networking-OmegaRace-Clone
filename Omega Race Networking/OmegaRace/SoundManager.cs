using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace OmegaRace
{
    public static class SoundManager
    {
        // Records if the game is muted, its initial value - the one created when no file is found, is false - meaning sound will play
        private static bool isMuted;
        public static bool IsMuted
        {
            get { return SoundManager.isMuted; }
            set
            {
                if (noAudio)
                {
                    value = true;
                }
                else
                {
                    // Sets muted to the passed in value and attempts to save it to file
                    try
                    {
                        byte byteValue;
                        if (value) { byteValue = 0; } // Converts the bool to a byte value for writing
                        else { byteValue = 1; }
                        using (fs = new FileStream("Content/settings.orx", FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.WriteByte(byteValue); // Writes the muted value
                        }
                    }
                    catch (IOException e) // If we have an error, print it
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                isMuted = value;
            }
        }
        public static bool toggleIsMuted() { return (IsMuted = !isMuted); } // Short-hand for toggling

        // The sound effects
        private static SoundEffect firing;
        private static SoundEffect mineLay;
        private static SoundEffect mineArm;
        private static SoundEffect minePop;
        private static SoundEffect shipPop;
        private static SoundEffect fenceHit;
        private static SoundEffect laserHit;
        private static SoundEffect shipBoost;
        private static SoundEffect textBlink;

        // The file for storing the mute state
        private static FileStream fs;

        private static bool noAudio = false;

        // Checks for the file and loads the muted state or defaults it to false
        // Loads all the sounds
        public static void Initialize()
        {
            try
            {
                using (fs = new FileStream("Content/settings.orx", FileMode.OpenOrCreate, FileAccess.Read))
                {
                    isMuted = ((fs.ReadByte()) == 0);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                // Their loss
                isMuted = false;
            }

            try
            {
                firing =    GameplayScreen.content.Load<SoundEffect>("Sounds/firing");
                mineLay =   GameplayScreen.content.Load<SoundEffect>("Sounds/mine_lay");
                mineArm =   GameplayScreen.content.Load<SoundEffect>("Sounds/mine_arm");
                minePop =   GameplayScreen.content.Load<SoundEffect>("Sounds/mine_pop");
                shipPop =   GameplayScreen.content.Load<SoundEffect>("Sounds/ship_pop");
                fenceHit =  GameplayScreen.content.Load<SoundEffect>("Sounds/fence_hit");
                laserHit =  GameplayScreen.content.Load<SoundEffect>("Sounds/laser_hit");
                shipBoost = GameplayScreen.content.Load<SoundEffect>("Sounds/ship_boost");
                textBlink = GameplayScreen.content.Load<SoundEffect>("Sounds/text_blink");
            }
            catch (NoAudioHardwareException e)
            {
                Console.WriteLine(e.ToString());
                isMuted = true;
                noAudio = true;
            }
        }

        // Closes the file
        public static void UnInitialize()
        {
            try
            {
                fs.Close();
                fs.Dispose();
            }
            catch (IOException) { }
        }

        // Plays the laser firing noise
        public static void playFiring()
        {
            if (!isMuted)
            {
                firing.Play();
            }
             
        }

        // Plays the sound for placing a mine
        public static void playMineLay()
        {
            if (!isMuted)
            {
                mineLay.Play();
            }
             
        }

        // Plays when a mine arms and become active
        public static void playMineArm()
        {
            if (!isMuted)
            {
                mineArm.Play();
            }
             
        }

        // Plays when a mine explodes
        public static void playMinePop()
        {
            if (!isMuted)
            {
                minePop.Play();
            }
             
        }

        // Plays when a player dies
        public static void playShipPop()
        {
            if (!isMuted)
            {
                shipPop.Play();
            }
             
        }

        // Plays when the fence plays it animation upon hit
        public static void playFenceHit()
        {
            if (!isMuted)
            {
                fenceHit.Play();
            }
             
        }

        // Played when the laser collides
        public static void playLaserHit()
        {
            if (!isMuted)
            {
                laserHit.Play();
            }
             
        }

        // Plays when the player is boosting
        public static void playShipBoost()
        {
            if (!isMuted)
            {
                shipBoost.Play();
            }
             
        }

        // Plays when the "Ready?" opening flashes
        public static void playTextBlink()
        {
            if (!isMuted)
            {
                textBlink.Play();
            }
             
        }
    }
}
