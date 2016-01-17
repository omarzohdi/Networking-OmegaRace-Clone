using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmegaRace
{
    // Recycles lasers and mines to reduce the call to new
    public static class WeaponManager
    {
        // Queues of precreated weapons
        private static Queue<Laser> availableLasers;
        private static Queue<Mine> availableMines;

        // Creates the empty Queues
        public static void Initialize()
        {
            if (availableLasers == null)
            {
                availableLasers = new Queue<Laser>();
            }
             
            if (availableMines == null)
            {
                availableMines = new Queue<Mine>();
            }
             
        }

        // Creates a number of initlial lasers and mines to use
        public static void InitializeWeapons(int lasers, int mines)
        {
            for (int i = 0; i < lasers; i++)
            {
                availableLasers.Enqueue(new Laser(Vector2.Zero, 0, PlayerIndex.One));
            }

            for (int i = 0; i < mines; i++)
            {
                availableMines.Enqueue(new Mine(Vector2.Zero, 0, PlayerIndex.One));
            }
        }

        // Spawns a laser if one is waiting on the Queue or creates a new one and applies the information to it
        public static Laser spawnLaser(Vector2 position, float rotation, PlayerIndex index)
        {
            Laser rLaser = null;
            if (availableLasers.Count == 0) // If no queued lasers exist, create one;
            {
                rLaser = new Laser(position, rotation, index);
                rLaser.respawn(position, rotation, index); // and respawn it with the information
            }
            else // otherwise dequeue one
            {
                rLaser = availableLasers.Dequeue();
                rLaser.respawn(position, rotation, index); // and respawn it with the information
            }

            // Adds the laser to the scene and returns a pointer to it
            return ((Laser)GameplayScreen.GameInstance.CurrentScene.addSceneObject(rLaser));
        }

        // Requeues a laser, called mostly by the Laser class upon death
        public static Laser addLaser(Laser laser)
        {
            availableLasers.Enqueue(laser);
            return laser;
        }

        // Spawns a mine if one is waiting on the Queue or creates a new one and applies the information to it
        public static Mine spawnMine(Vector2 position, float rotation, PlayerIndex index)
        {
            Mine rMine = null;
            if (availableMines.Count == 0) // If no queued mines exist, create one;
            {
                rMine = new Mine(position, rotation, index);
                // No need to respawn it
            }
            else
            {
                rMine = availableMines.Dequeue(); // otherwise dequeue one
                rMine.respawn(position, rotation, index); // and respawn it with the information
            }

            // Adds the miine to the scene and returns a pointer to it
            return ((Mine)GameplayScreen.GameInstance.CurrentScene.addSceneObject(rMine));
        }

        // Requeues a mine, called mostly by the Mine class upon death
        public static Mine addMine(Mine mine)
        {
            availableMines.Enqueue(mine);
            return mine;
        }
    }
}
