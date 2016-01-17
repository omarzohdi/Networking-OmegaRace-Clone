#region File Description
//-----------------------------------------------------------------------------
// AnimatedTexture.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OmegaRace
{
    // Allows for animated textures of either a series of images or an image sheet
    // AnimatedTexture is an abstract base class extended by AnimatedTextureSheet and AnimatedTextureFiles
    // Load needs to be called to get the texture
    public abstract class AnimatedTexture
    {
        protected Texture2D myTexture; // The texture for drawing
        protected float TimePerFrame; // The time to spend on each frame
        protected float TotalElapsed; // A counter of time to ensure all frames move at exactly their frame length
        protected int framecount; // The number of frames in the animation

        private bool Paused; // Whether the animation is paused or not
        public bool IsPaused
        {
            get { return Paused; }
            set { Paused = value; }
        }

        protected bool playedThrough; // Whether or not the animation has completed a loop
        public bool PlayedThrough
        {
            get { return playedThrough; }
            set { playedThrough = value; }
        }

        protected int Frame; // The current frame
        public int CurrentFrame
        {
            get { return Frame; }
        }

        // Floats for modifying the animation
        public float Rotation, Scale, Depth;
        public Vector2 Origin; // The origin to orbit around when rotating

        // Base class constructor
        public AnimatedTexture(Vector2 origin, float rotation,
            float scale, float depth)
        {
            this.Origin = origin;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Depth = depth;
            this.playedThrough = false;
        }

        // Loads the image needed
        public virtual void Load(ContentManager content, string asset,
            int frameCount, int framesPerSec)
        {
            framecount = frameCount;
            myTexture = content.Load<Texture2D>(asset);
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        // Updates the current frame and tracks if a loop has been completed
        public virtual void UpdateFrame(float elapsed)
        {
            // Don't update if paused
            if (!Paused)
            {
                // Ensures that the frames move fluidly
                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    // Increase the frame
                    Frame++;
                    if (Frame >= framecount)
                    {
                        playedThrough = true; // A loop has been completed
                    }
                     
                    // Keep the Frame between 0 and the total frames, minus one
                    Frame = Frame % framecount;
                    // Subtract the time of one frame per frame
                    TotalElapsed -= TimePerFrame;
                }
            }
             
        }

        // Calls the method to draw the current frame
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, Frame, screenPos);
        }

        // Draws the frameth frame with all it modifiers
        public virtual void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            if (myTexture != null && batch != null && screenPos != null)
            {
                int FrameWidth = myTexture.Width / framecount;
                Rectangle sourcerect = new Rectangle(FrameWidth * frame, 0,
                    FrameWidth, myTexture.Height);
                batch.Begin();
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
                batch.End();
            }
             
        }

        // Resets the animation in frame, time, and is completed
        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
            playedThrough = false;
        }

        // Pauses and resets the animation
        public void Stop()
        {
            Pause();
            Reset();
        }

        // Unpauses the animation
        public void Play()
        {
            Paused = false;
        }

        // Pauses the animation
        public void Pause()
        {
            Paused = true;
        }

    }

    // Uses all the base class methods and constructor
    public class AnimatedTextureSheet : AnimatedTexture
    {
        public AnimatedTextureSheet(Vector2 origin, float rotation,
            float scale, float depth)
            : base(origin, rotation, scale, depth)
        {

        }

    }

    // Uses multiple files rather than a sprite sheet to animate
    public class AnimatedTextureFiles : AnimatedTexture
    {
        // The array of textures
        private Texture2D[] myTextures;

        // Uses the base constructor
        public AnimatedTextureFiles(Vector2 origin, float rotation,
            float scale, float depth)
            : base(origin, rotation, scale, depth)
        {

        }

        // Loads the array of files by appending a number from 1 to frameCount to the file name
        public override void Load(ContentManager content, string asset,
            int frameCount, int framesPerSec)
        {
            framecount = frameCount;
            myTextures = new Texture2D[frameCount];
            for (int i = 0; i < framecount; i++) // Load each image by creating the file name and loading it
            {
                myTextures[i] = content.Load<Texture2D>(asset+(i+1));
            }
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            IsPaused = false;
            myTexture = myTextures[0]; // Set the initial texture
        }

        // Updates the frame based on the time elapsed
        public override void UpdateFrame(float elapsed)
        {
            if (!IsPaused)
            {
                // Keeps an accurate count of the time passed between the last frame
                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    Frame++; // Update the frame
                    if (CurrentFrame >= framecount) // Check for a complete loop
                    {
                        PlayedThrough = true;
                    }
                     
                    // Keep the Frame between 0 and the total frames, minus one
                    Frame = CurrentFrame % framecount;
                    TotalElapsed -= TimePerFrame;
                }
                // Set the current texture to the Frameth texture in the array
                myTexture = myTextures[CurrentFrame];
            }
             
        }

        // Draws the frame without modifications like rotation or scale
        public void DrawFrameSimple(SpriteBatch batch, Vector2 screenPos)
        {
            if (myTexture != null && batch != null && screenPos != null)
            {
                batch.Begin();
                batch.Draw(myTexture, screenPos, Color.White);
                batch.End();
            }
             
        }

        // Draws the frame with rotation and scale
        public override void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            if (myTexture != null && batch != null && screenPos != null)
            {
                Rectangle sourcerect = new Rectangle(0, 0,
                    myTexture.Width, myTexture.Height);
                // Orbit around the center
                Vector2 origin = new Vector2(myTexture.Width/2, myTexture.Height/2); 
                batch.Begin();
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, origin, Scale, SpriteEffects.None, Depth);
                batch.End();
            }
             
        }
    }
}
