using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OmegaRace
{
    // Manages input and allows recording of things like isPressed
    // Must be initalized and must be flushed at the end of an Update
    public static class InputManager
    {
        // Max Players (not used)
        private const int MAX_PLAYERS = 2;

        // Keyboard keys
        private static bool[] LocalplayerKeys;
        private static bool[] RemoteplayerKeys;
        private static bool[] player3Keys;
        private static bool[] player4Keys;

        // XBox keys
        private static Dictionary<Buttons, Boolean> XBoxButtons1;
        private static Dictionary<Buttons, Boolean> XBoxButtons2;
        private static Dictionary<Buttons, Boolean> XBoxButtons3;
        private static Dictionary<Buttons, Boolean> XBoxButtons4;

        // Reference region of commented values
        #region GamePad Numbers
        // | operator can be used
        /*
            DPadUp = 1,
            DPadDown = 2,
            DPadLeft = 4,
            DPadRight = 8,
            Start = 16,
            Back = 32,
            LeftStick = 64,
            RightStick = 128,
            LeftShoulder = 256,
            RightShoulder = 512,
            BigButton = 2048,
            A = 4096,
            B = 8192,
            X = 16384,
            Y = 32768,
            LeftThumbstickLeft = 2097152,
            RightTrigger = 4194304,
            LeftTrigger = 8388608,
            RightThumbstickUp = 16777216,
            RightThumbstickDown = 33554432,
            RightThumbstickRight = 67108864,
            RightThumbstickLeft = 134217728,
            LeftThumbstickUp = 268435456,
            LeftThumbstickDown = 536870912,
            LeftThumbstickRight = 1073741824,
        */
        #endregion

        // Methods for getting information on the necessary keyboard keys
        #region Keyboard Keys
        private static bool escKeyReleased;
        public static bool isKeyEscDown() { return (Keyboard.GetState().IsKeyDown(Keys.Escape)); }
        public static bool isKeyEscUp() { return (Keyboard.GetState().IsKeyUp(Keys.Escape)); }
        public static bool isKeyEscPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Escape) && escKeyReleased); }

        private static bool enterKeyReleased;
        public static bool isKeyEnterDown() { return (Keyboard.GetState().IsKeyDown(Keys.Enter)); }
        public static bool isKeyEnterUp() { return (Keyboard.GetState().IsKeyUp(Keys.Enter)); }
        public static bool isKeyEnterPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Enter) && enterKeyReleased); }

        private static bool tabKeyReleased;
        public static bool isKeyTabDown() { return (Keyboard.GetState().IsKeyDown(Keys.Tab)); }
        public static bool isKeyTabUp() { return (Keyboard.GetState().IsKeyUp(Keys.Tab)); }
        public static bool isKeyTabPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Tab) && tabKeyReleased); }

        private static bool backspaceKeyReleased;
        public static bool isKeyBackspaceDown() { return (Keyboard.GetState().IsKeyDown(Keys.Back)); }
        public static bool isKeyBackspaceUp() { return (Keyboard.GetState().IsKeyUp(Keys.Back)); }
        public static bool isKeyBackspacePressed() { return (Keyboard.GetState().IsKeyDown(Keys.Back) && backspaceKeyReleased); }

        private static bool leftShiftKeyReleased;
        public static bool isKeyLeftShiftDown() { return (Keyboard.GetState().IsKeyDown(Keys.LeftShift)); }
        public static bool isKeyLeftShiftUp() { return (Keyboard.GetState().IsKeyUp(Keys.LeftShift)); }
        public static bool isKeyLeftShiftPressed() { return (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && leftShiftKeyReleased); }

        private static bool leftCtrlKeyReleased;
        public static bool isKeyLeftCtrlDown() { return (Keyboard.GetState().IsKeyDown(Keys.LeftControl)); }
        public static bool isKeyLeftCtrlUp() { return (Keyboard.GetState().IsKeyUp(Keys.LeftControl)); }
        public static bool isKeyLeftCtrlPressed() { return (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && leftCtrlKeyReleased); }

        private static bool rightShiftKeyReleased;
        public static bool isKeyRightShiftDown() { return (Keyboard.GetState().IsKeyDown(Keys.RightShift)); }
        public static bool isKeyRightShiftUp() { return (Keyboard.GetState().IsKeyUp(Keys.RightShift)); }
        public static bool isKeyRightShiftPressed() { return (Keyboard.GetState().IsKeyDown(Keys.RightShift) && rightShiftKeyReleased); }

        private static bool rightCtrlKeyReleased;
        public static bool isKeyRightCtrlDown() { return (Keyboard.GetState().IsKeyDown(Keys.RightControl)); }
        public static bool isKeyRightCtrlUp() { return (Keyboard.GetState().IsKeyUp(Keys.RightControl)); }
        public static bool isKeyRightCtrlPressed() { return (Keyboard.GetState().IsKeyDown(Keys.RightControl) && rightCtrlKeyReleased); }

        private static bool spaceKeyReleased;
        public static bool isKeySpaceDown() { return (Keyboard.GetState().IsKeyDown(Keys.Space)); }
        public static bool isKeySpaceUp() { return (Keyboard.GetState().IsKeyUp(Keys.Space)); }
        public static bool isKeySpacePressed() { return (Keyboard.GetState().IsKeyDown(Keys.Space) && spaceKeyReleased); }

        private static bool wKeyReleased;
        public static bool isKeyWDown() { return (Keyboard.GetState().IsKeyDown(Keys.W)); }
        public static bool isKeyWUp() { return (Keyboard.GetState().IsKeyUp(Keys.W)); }
        public static bool isKeyWPressed() { return (Keyboard.GetState().IsKeyDown(Keys.W) && wKeyReleased); }

        private static bool sKeyReleased;
        public static bool isKeySDown() { return (Keyboard.GetState().IsKeyDown(Keys.S)); }
        public static bool isKeySUp() { return (Keyboard.GetState().IsKeyUp(Keys.S)); }
        public static bool isKeySPressed() { return (Keyboard.GetState().IsKeyDown(Keys.S) && sKeyReleased); }

        private static bool aKeyReleased;
        public static bool isKeyADown() { return (Keyboard.GetState().IsKeyDown(Keys.A)); }
        public static bool isKeyAUp() { return (Keyboard.GetState().IsKeyUp(Keys.A)); }
        public static bool isKeyAPressed() { return (Keyboard.GetState().IsKeyDown(Keys.A) && aKeyReleased); }

        private static bool dKeyReleased;
        public static bool isKeyDDown() { return (Keyboard.GetState().IsKeyDown(Keys.D)); }
        public static bool isKeyDUp() { return (Keyboard.GetState().IsKeyUp(Keys.D)); }
        public static bool isKeyDPressed() { return (Keyboard.GetState().IsKeyDown(Keys.D) && dKeyReleased); }

        private static bool upKeyReleased;
        public static bool isKeyUpDown() { return (Keyboard.GetState().IsKeyDown(Keys.Up)); }
        public static bool isKeyUpUp() { return (Keyboard.GetState().IsKeyUp(Keys.Up)); }
        public static bool isKeyUpPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Up) && upKeyReleased); }

        private static bool downKeyReleased;
        public static bool isKeyDownDown() { return (Keyboard.GetState().IsKeyDown(Keys.Down)); }
        public static bool isKeyDownUp() { return (Keyboard.GetState().IsKeyUp(Keys.Down)); }
        public static bool isKeyDownPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Down) && downKeyReleased); }

        private static bool leftKeyReleased;
        public static bool isKeyLeftDown() { return (Keyboard.GetState().IsKeyDown(Keys.Left)); }
        public static bool isKeyLeftUp() { return (Keyboard.GetState().IsKeyUp(Keys.Left)); }
        public static bool isKeyLeftPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Left) && leftKeyReleased); }

        private static bool rightKeyReleased;
        public static bool isKeyRightDown() { return (Keyboard.GetState().IsKeyDown(Keys.Right)); }
        public static bool isKeyRightUp() { return (Keyboard.GetState().IsKeyUp(Keys.Right)); }
        public static bool isKeyRightPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Right) && rightKeyReleased); }

        private static bool yKeyReleased;
        public static bool isKeyYDown() { return (Keyboard.GetState().IsKeyDown(Keys.Y)); }
        public static bool isKeyYUp() { return (Keyboard.GetState().IsKeyUp(Keys.Y)); }
        public static bool isKeyYPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Y) && yKeyReleased); }

        private static bool qKeyReleased;
        public static bool isKeyQDown() { return (Keyboard.GetState().IsKeyDown(Keys.Q)); }
        public static bool isKeyQUp() { return (Keyboard.GetState().IsKeyUp(Keys.Q)); }
        public static bool isKeyQPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Q) && qKeyReleased); }

        private static bool zKeyReleased;
        public static bool isKeyZDown() { return (Keyboard.GetState().IsKeyDown(Keys.Z)); }
        public static bool isKeyZUp() { return (Keyboard.GetState().IsKeyUp(Keys.Z)); }
        public static bool isKeyZPressed() { return (Keyboard.GetState().IsKeyDown(Keys.Z) && zKeyReleased); }

        private static bool xKeyReleased;
        public static bool isKeyXDown() { return (Keyboard.GetState().IsKeyDown(Keys.X)); }
        public static bool isKeyXUp() { return (Keyboard.GetState().IsKeyUp(Keys.X)); }
        public static bool isKeyXPressed() { return (Keyboard.GetState().IsKeyDown(Keys.X) && xKeyReleased); }

        private static bool cKeyReleased;
        public static bool isKeyCDown() { return (Keyboard.GetState().IsKeyDown(Keys.C)); }
        public static bool isKeyCUp() { return (Keyboard.GetState().IsKeyUp(Keys.C)); }
        public static bool isKeyCPressed() { return (Keyboard.GetState().IsKeyDown(Keys.C) && cKeyReleased); }

        private static bool vKeyReleased;
        public static bool isKeyVDown() { return (Keyboard.GetState().IsKeyDown(Keys.V)); }
        public static bool isKeyVUp() { return (Keyboard.GetState().IsKeyUp(Keys.V)); }
        public static bool isKeyVPressed() { return (Keyboard.GetState().IsKeyDown(Keys.V) && vKeyReleased); }

        private static bool lKeyReleased;
        public static bool isKeyLDown() { return (Keyboard.GetState().IsKeyDown(Keys.L)); }
        public static bool isKeyLUp() { return (Keyboard.GetState().IsKeyUp(Keys.L)); }
        public static bool isKeyLPressed() { return (Keyboard.GetState().IsKeyDown(Keys.L) && lKeyReleased); }

        private static bool pKeyReleased;
        public static bool isKeyPDown() { return (Keyboard.GetState().IsKeyDown(Keys.P)); }
        public static bool isKeyPUp() { return (Keyboard.GetState().IsKeyUp(Keys.P)); }
        public static bool isKeyPPressed() { return (Keyboard.GetState().IsKeyDown(Keys.P) && pKeyReleased); }

        private static bool mKeyReleased;
        public static bool isKeyMDown() { return (Keyboard.GetState().IsKeyDown(Keys.M)); }
        public static bool isKeyMUp() { return (Keyboard.GetState().IsKeyUp(Keys.M)); }
        public static bool isKeyMPressed() { return (Keyboard.GetState().IsKeyDown(Keys.M) && mKeyReleased); }

        private static bool bKeyReleased;
        public static bool isKeyBDown() { return (Keyboard.GetState().IsKeyDown(Keys.B)); }
        public static bool isKeyBUp() { return (Keyboard.GetState().IsKeyUp(Keys.B)); }
        public static bool isKeyBPressed() { return (Keyboard.GetState().IsKeyDown(Keys.B) && bKeyReleased); }

        private static bool nKeyReleased;
        public static bool isKeyNDown() { return (Keyboard.GetState().IsKeyDown(Keys.N)); }
        public static bool isKeyNUp() { return (Keyboard.GetState().IsKeyUp(Keys.N)); }
        public static bool isKeyNPressed() { return (Keyboard.GetState().IsKeyDown(Keys.N) && nKeyReleased); }
        #endregion

        // Methods for getting information on the necessary controller keys, needs the PlayerIndex
        #region XBox Buttons
        public static bool isButtonDown(PlayerIndex number, Buttons button) { return (GamePad.GetState(number).IsButtonDown(button)); }
        public static bool isButtonUp(PlayerIndex number, Buttons button) { return (GamePad.GetState(number).IsButtonUp(button)); }
        public static bool isButtonPressed(PlayerIndex number, Buttons button)
        {
            bool value = false;
            switch (button)
            {
                case Buttons.A:
                    value = isAPressed(number);
                    break;
                case Buttons.B:
                    value = isBPressed(number);
                    break;
                case Buttons.X:
                    value = isXPressed(number);
                    break;
                case Buttons.Y:
                    value = isYPressed(number);
                    break;
                case Buttons.BigButton:
                    value = isBigButtonPressed(number);
                    break;
                case Buttons.Start:
                    value = isStartPressed(number);
                    break;
                case Buttons.Back:
                    value = isBackPressed(number);
                    break;
            }
            return value;
        }

        private static bool[] aReleased;
        public static bool isADown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.A == ButtonState.Pressed); }
        public static bool isAUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.A == ButtonState.Released); }
        public static bool isAPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.A == ButtonState.Pressed) && aReleased[(int)number]); }
        public static bool isAPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Pressed) && aReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] bReleased;
        public static bool isBDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.B == ButtonState.Pressed); }
        public static bool isBbUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.B == ButtonState.Released); }
        public static bool isBPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.B == ButtonState.Pressed) && bReleased[(int)number]); }
        public static bool isBPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.B == ButtonState.Pressed) && bReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] xReleased;
        public static bool isXDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.X == ButtonState.Pressed); }
        public static bool isXUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.X == ButtonState.Released); }
        public static bool isXPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.X == ButtonState.Pressed) && xReleased[(int)number]); }
        public static bool isXPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.X == ButtonState.Pressed) && xReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] yReleased;
        public static bool isYDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Y == ButtonState.Pressed); }
        public static bool isYUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Y == ButtonState.Released); }
        public static bool isYPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.Y == ButtonState.Pressed) && yReleased[(int)number]); }
        public static bool isYPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.Y == ButtonState.Pressed) && yReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] bigButtonReleased;
        public static bool isBigButtonDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.BigButton == ButtonState.Pressed); }
        public static bool isBigButtonUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.BigButton == ButtonState.Released); }
        public static bool isBigButtonPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.BigButton == ButtonState.Pressed) && bigButtonReleased[(int)number]); }
        public static bool isBigButtonPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.BigButton == ButtonState.Pressed) && bigButtonReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] startReleased;
        public static bool isStartDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Start == ButtonState.Pressed); }
        public static bool isStartUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Start == ButtonState.Released); }
        public static bool isStartPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.Start == ButtonState.Pressed) && startReleased[(int)number]); }
        public static bool isStartPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed) && startReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] backReleased;
        public static bool isBackDown(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Back == ButtonState.Pressed); }
        public static bool isBackUp(PlayerIndex number) { return (GamePad.GetState(number).Buttons.Back == ButtonState.Released); }
        public static bool isBackPressed(PlayerIndex number) { return ((GamePad.GetState(number).Buttons.Back == ButtonState.Pressed) && backReleased[(int)number]); }
        public static bool isBackPressed()
        {
            bool rValue = false;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if ((GamePad.GetState((PlayerIndex)i).Buttons.Back == ButtonState.Pressed) && backReleased[i])
                {
                    rValue = true;
                    break;
                }
                 
            }
            return rValue;
        }

        private static bool[] ltReleased; // left trigger
        public static bool isLTDown(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Left > 0); }
        public static bool isLTUp(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Left == 0); }
        public static bool isLTPressed(PlayerIndex number) { return ((GamePad.GetState(number).Triggers.Left > 0) && ltReleased[(int)number]); }
        public static float leftTriggerVector(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Left); }

        private static bool[] rtReleased; // right trigger
        public static bool isRTDown(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Right > 0); }
        public static bool isRTUp(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Right == 0); }
        public static bool isRTPressed(PlayerIndex number) { return ((GamePad.GetState(number).Triggers.Right > 0) && rtReleased[(int)number]); }
        public static float rightTriggerVector(PlayerIndex number) { return (GamePad.GetState(number).Triggers.Right); }

        public static float leftStickXVector(PlayerIndex number) { return (GamePad.GetState(number).ThumbSticks.Left.X); }
        public static float leftStickYVector(PlayerIndex number) { return (GamePad.GetState(number).ThumbSticks.Left.Y); }

        public static float rightStickXVector(PlayerIndex number) { return (GamePad.GetState(number).ThumbSticks.Right.X); }
        public static float rightStickYVector(PlayerIndex number) { return (GamePad.GetState(number).ThumbSticks.Right.Y); }

        private static bool[] dPadUpReleased;
        public static bool isDPadUpDown(PlayerIndex number) { return (GamePad.GetState(number).DPad.Up == ButtonState.Pressed); }
        public static bool isDPadUpUp(PlayerIndex number) { return (GamePad.GetState(number).DPad.Up == ButtonState.Released); }
        public static bool isDPadUpPressed(PlayerIndex number) { return ((GamePad.GetState(number).DPad.Up == ButtonState.Pressed) && dPadUpReleased[(int)number]); }

        private static bool[] dPadDownReleased;
        public static bool isDPadDownDown(PlayerIndex number) { return (GamePad.GetState(number).DPad.Down == ButtonState.Pressed); }
        public static bool isDPadDownUp(PlayerIndex number) { return (GamePad.GetState(number).DPad.Down == ButtonState.Released); }
        public static bool isDPadDownPressed(PlayerIndex number) { return ((GamePad.GetState(number).DPad.Down == ButtonState.Pressed) && dPadDownReleased[(int)number]); }

        private static bool[] dPadLeftReleased;
        public static bool isDPadLeftDown(PlayerIndex number) { return (GamePad.GetState(number).DPad.Left == ButtonState.Pressed); }
        public static bool isDPadLeftUp(PlayerIndex number) { return (GamePad.GetState(number).DPad.Left == ButtonState.Released); }
        public static bool isDPadLeftPressed(PlayerIndex number) { return ((GamePad.GetState(number).DPad.Left == ButtonState.Pressed) && dPadLeftReleased[(int)number]); }

        private static bool[] dPadRightReleased;
        public static bool isDPadRightDown(PlayerIndex number) { return (GamePad.GetState(number).DPad.Right == ButtonState.Pressed); }
        public static bool isDPadRightUp(PlayerIndex number) { return (GamePad.GetState(number).DPad.Right == ButtonState.Released); }
        public static bool isDPadRightPressed(PlayerIndex number) { return ((GamePad.GetState(number).DPad.Right == ButtonState.Pressed) && dPadRightReleased[(int)number]); }
        #endregion

        // Methods for getting information on particular combination of keys, ie short-hand methods
        #region Special Buttons
        public static bool isConfirmationKeyPressed()
        {
            return ((isKeyEnterPressed())
                || (isAPressed(PlayerIndex.One)
                || isAPressed(PlayerIndex.Two)
                || isAPressed(PlayerIndex.Three)
                || isAPressed(PlayerIndex.Four))
                || (isStartPressed(PlayerIndex.One)
                || isStartPressed(PlayerIndex.Two)
                || isStartPressed(PlayerIndex.Three)
                || isStartPressed(PlayerIndex.Four)));
        }

        public static bool isQuitKeyPressed()
        {
            return ((isKeyEscPressed())
                || (isBackPressed(PlayerIndex.One)
                || isBackPressed(PlayerIndex.Two)
                || isBackPressed(PlayerIndex.Three)
                || isBackPressed(PlayerIndex.Four)));
        }
        #endregion

        // Sets up the bools for Released which is used for isPressed methods to determine if a key was up and is now down
        public static void Initialize()
        {
            // Initializes every key up value to false
            wKeyReleased = false;
            sKeyReleased = false;
            dKeyReleased = false;
            aKeyReleased = false;
            dKeyReleased = false;
            upKeyReleased = false;
            downKeyReleased = false;
            leftKeyReleased = false;
            rightKeyReleased = false;
            enterKeyReleased = false;
            tabKeyReleased = false;
            escKeyReleased = false;
            backspaceKeyReleased = false;
            leftShiftKeyReleased = false;
            leftCtrlKeyReleased = false;
            rightShiftKeyReleased = false;
            rightCtrlKeyReleased = false;
            spaceKeyReleased = false;
            yKeyReleased = false;
            qKeyReleased = false;
            zKeyReleased = false;
            vKeyReleased = false;
            xKeyReleased = false;
            cKeyReleased = false;
            lKeyReleased = false;
            pKeyReleased = false;
            mKeyReleased = false;
            bKeyReleased = false;
            nKeyReleased = false;
            aReleased = new bool[] { false, false, false, false };
            bReleased = new bool[] { false, false, false, false };
            xReleased = new bool[] { false, false, false, false };
            yReleased = new bool[] { false, false, false, false };
            ltReleased = new bool[] { false, false, false, false };
            rtReleased = new bool[] { false, false, false, false };
            bigButtonReleased = new bool[] { false, false, false, false };
            startReleased = new bool[] { false, false, false, false };
            backReleased = new bool[] { false, false, false, false };
            dPadUpReleased = new bool[] { false, false, false, false };
            dPadDownReleased = new bool[] { false, false, false, false };
            dPadLeftReleased = new bool[] { false, false, false, false };
            dPadRightReleased = new bool[] { false, false, false, false };

            XBoxButtons1 = new Dictionary<Buttons, bool>();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons1.Add(button, false);
            }

            XBoxButtons2 = new Dictionary<Buttons, bool>();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons2.Add(button, false);
            }

            XBoxButtons3 = new Dictionary<Buttons, bool>();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons3.Add(button, false);
            }

            XBoxButtons4 = new Dictionary<Buttons, bool>();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons4.Add(button, false);
            }

#if WINDOWS
            LocalplayerKeys = new bool[sizeof(Keys)];
            for (int i = 0; i < LocalplayerKeys.Length; i++) { LocalplayerKeys[i] = false; }
            RemoteplayerKeys = new bool[sizeof(Keys)];
            for (int i = 0; i < RemoteplayerKeys.Length; i++) { RemoteplayerKeys[i] = false; }
            player3Keys = new bool[sizeof(Keys)];
            for (int i = 0; i < player3Keys.Length; i++) { player3Keys[i] = false; }
            player4Keys = new bool[sizeof(Keys)];
            for (int i = 0; i < player4Keys.Length; i++) { player4Keys[i] = false; }
#endif
        }

        // Checks if the keys are up for purposes of isPressed
        public static void FlushInput()
        {
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                aReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                bReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.B == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                xReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.X == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                yReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.Y == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                ltReleased[i] = (GamePad.GetState((PlayerIndex)i).Triggers.Left == 0);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                rtReleased[i] = (GamePad.GetState((PlayerIndex)i).Triggers.Right == 0);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                bigButtonReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.BigButton == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                startReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                backReleased[i] = (GamePad.GetState((PlayerIndex)i).Buttons.Back == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                dPadUpReleased[i] = (GamePad.GetState((PlayerIndex)i).DPad.Up == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                dPadDownReleased[i] = (GamePad.GetState((PlayerIndex)i).DPad.Down == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                dPadLeftReleased[i] = (GamePad.GetState((PlayerIndex)i).DPad.Left == ButtonState.Released);
            }

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                dPadRightReleased[i] = (GamePad.GetState((PlayerIndex)i).DPad.Right == ButtonState.Released);
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons1[button] = GamePad.GetState(PlayerIndex.One).IsButtonUp(button);
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons2[button] = GamePad.GetState(PlayerIndex.Two).IsButtonUp(button);
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons3[button] = GamePad.GetState(PlayerIndex.Three).IsButtonUp(button);
            }

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                XBoxButtons4[button] = GamePad.GetState(PlayerIndex.Four).IsButtonUp(button);
            }

#if WINDOWS
            wKeyReleased = isKeyWUp();
            sKeyReleased = isKeySUp();
            dKeyReleased = isKeyDUp();
            aKeyReleased = isKeyAUp();
            dKeyReleased = isKeySUp();
            upKeyReleased = isKeyUpUp();
            downKeyReleased = isKeyDownUp();
            leftKeyReleased = isKeyLeftUp();
            rightKeyReleased = isKeyRightUp();
            enterKeyReleased = isKeyEnterUp();
            tabKeyReleased = isKeyTabUp();
            escKeyReleased = isKeyEscUp();
            backspaceKeyReleased = isKeyBackspaceUp();
            leftShiftKeyReleased = isKeyLeftShiftUp();
            leftCtrlKeyReleased = isKeyLeftCtrlUp();
            rightShiftKeyReleased = isKeyRightShiftUp();
            rightCtrlKeyReleased = isKeyRightCtrlUp();
            spaceKeyReleased = isKeySpaceUp();
            yKeyReleased = isKeyYUp();
            qKeyReleased = isKeyQUp();
            zKeyReleased = isKeyZUp();
            vKeyReleased = isKeyVUp();
            xKeyReleased = isKeyXUp();
            cKeyReleased = isKeyCUp();
            lKeyReleased = isKeyLUp();
            pKeyReleased = isKeyPUp();
            mKeyReleased = isKeyMUp();
            bKeyReleased = isKeyBUp();
            nKeyReleased = isKeyNUp();

            for (int i = 0; i < LocalplayerKeys.Length; i++) { LocalplayerKeys[i] = Keyboard.GetState().IsKeyDown((Keys)i); }
            for (int i = 0; i < RemoteplayerKeys.Length; i++) { RemoteplayerKeys[i] = Keyboard.GetState().IsKeyDown((Keys)i); }
            for (int i = 0; i < player3Keys.Length; i++) { player3Keys[i] = Keyboard.GetState().IsKeyDown((Keys)i); }
            for (int i = 0; i < player4Keys.Length; i++) { player4Keys[i] = Keyboard.GetState().IsKeyDown((Keys)i); }
#endif
        }
    }
}
