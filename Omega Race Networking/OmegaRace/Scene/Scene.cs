using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace OmegaRace
{
    // Base class for scenes, extended by GameScene and MainMainScene
    public abstract class Scene
    {
        protected bool readyToExit = false; // Set when a scene is ready to end and proceed to the next scene
        public bool ReadyToExit
        {
            get { return readyToExit; }
        }
        // No setter

        // Pausing (used only in GameScene)
        private bool isPaused = false;
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }
        public bool Pause() { isPaused = true; return isPaused; }
        public bool UnPause() { isPaused = false; return isPaused; }
        // The texture for the paused game
        private Texture2D pauseTexture;

        // List of scene objects to use in the scene
        protected List<SceneObject> sceneObjects; // Current objects in the scene, will be updated and drawn
        private List<SceneObject> addList; // Items waiting to be added to the scene, will be added in Update
        private List<SceneObject> removeList; // Items waiting to be removed from the scene, will be removed in Update
        public SceneObject addSceneObject(SceneObject add) // Adds an object to the add list to be added in Update
        {
            if (!addList.Contains(add) && !sceneObjects.Contains(add)) // If it's not already in the game or the add list
            {
                addList.Add(add); // Set it to be added
            }
             
            return add;
        }
        public SceneObject removeSceneObject(SceneObject remove)  // Adds an object to the remove list to be removed in Update
        {
            if (!removeList.Contains(remove) && sceneObjects.Contains(remove)) // Make sure it isn't already on the remove list, but it is in the game
            {
                removeList.Add(remove); // Set it to be removed
            }
             
            return remove;
        }

        // Empty base constructor
        protected Scene()
        {

        }

        // Creates the list and initializes the pause texture
        public virtual void Initialize()
        {
            sceneObjects = new List<SceneObject>();
            addList = new List<SceneObject>();
            removeList = new List<SceneObject>();

            pauseTexture = GameplayScreen.content.Load<Texture2D>("pausedTexture");
        }

        // Updates the lists and every scene object on the sceneObjects list
        public virtual void Update(float deltaTime)
        {
            if (!isPaused) // If paused, do nothing
            {
                // Add every item on the to add list to the scene object list
                foreach (SceneObject item in addList)
                {
                    sceneObjects.Add(item);
                }
                addList.Clear(); // Clear out the used up list

                // Update every scene object
                foreach (SceneObject item in sceneObjects)
                {
                    item.Update(deltaTime);
                    if (item.ReadyToRemove) // If an item is dead, queue it for removal
                    {
                        removeList.Add(item);
                    }
                     
                }

                // Remove dead objects from the list
                foreach (SceneObject item in removeList)
                {
                    sceneObjects.Remove(item);
                    item.onRemove(); // Make sure to call its on remove method
                }
                removeList.Clear(); // Clear out the used up list
            }
             
        }

        // Draw all scene objects on the list
        public virtual void Draw()
        {
            foreach (SceneObject draw in sceneObjects)
            {
                if (!draw.ReadyToRemove) // Don't draw dead objects
                {
                    draw.Draw();
                }
                 
            }

            // If the game is paused, draw the paused texture
            if (isPaused)
            {
                SpriteBatch spriteBatch = GameplayScreen.GameInstance.SpriteBatch;
                spriteBatch.Begin();
                spriteBatch.Draw(pauseTexture, Vector2.Zero, null, Color.White);
                spriteBatch.End();
            }
        }

        // Called when a scene ends
        public virtual void Exit()
        {

        }

        // Says what scene is next when a scene ends
       public abstract GameplayScreen.sceneEnum nextScene();

    }
}
