using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Violet.Input
{
    /// <summary>
    /// Handles input.
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// The acting instance of the Input Manager.
        /// </summary>
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputManager();
                }
                return instance;
            }
        }
        private static InputManager instance;


        /// <summary>
        /// Invoked when a button is pressed. Example: Pressing the X key. Provides the key that was pressed.
        /// </summary>
        public event ButtonPressedHandler ButtonPressed;

        /// <summary>
        /// Invoked when a button is released. i.e: Taking your finger off of the X key.
        /// </summary>
        public event ButtonReleasedHandler ButtonReleased;

        /// <summary>
        /// Invoked when a stick on a controller is pressed. 
        /// </summary>
        public event AxisPressedHandler AxisPressed;

        /// <summary>
        /// Invoked when a stick on a controller is released. 
        /// </summary>
        public event AxisReleasedHandler AxisReleased;

        /// <summary>
        /// Allows you to look up whether or not a button is pressed.
        /// </summary>
        public Dictionary<Button, bool> State
        {
            get => this.currentState;
        }

        /// <summary>
        /// Returns the identification info of the controller that is currently connected.
        /// </summary>
        public Joystick.Identification ControllerIdentification
        {
            get => currentControllerIdentification;
        }
        private Joystick.Identification currentControllerIdentification;

        /// <summary>
        /// If true, the Input Manager is receiving inputs and invoking events. If false, it isn't.
        /// </summary>
        public bool Enabled
        {
            get => this.enabled;
            set => this.enabled = value;
        }
        private bool enabled;


        public Vector2f Axis
        {
            get
            {
                float x = Math.Max(-1f, Math.Min(1f, this.xAxis + this.xKeyAxis));
                float y = Math.Max(-1f, Math.Min(1f, this.yAxis + this.yKeyAxis));
                return new Vector2f(x, y);
            }
        }

        #region Controller Fields
        private float xAxis;
        private float yAxis;
        private float xKeyAxis;
        private float yKeyAxis;

        private bool axisZero;
        private bool axisZeroLast;

        #endregion

        #region Keyboard Fields
        private bool leftPress;
        private bool rightPress;
        private bool upPress;
        private bool downPress;
        #endregion

        #region Delegates for the events.
        public delegate void ButtonPressedHandler(InputManager sender, Button b);

        public delegate void ButtonReleasedHandler(InputManager sender, Button b);

        public delegate void AxisPressedHandler(InputManager sender, Vector2f axis);

        public delegate void AxisReleasedHandler(InputManager sender, Vector2f axis);
        #endregion

        private Keyboard.Key upKey = Keyboard.Key.Up;
        private Keyboard.Key downKey = Keyboard.Key.Down;
        private Keyboard.Key leftKey = Keyboard.Key.Left;
        private Keyboard.Key rightKey = Keyboard.Key.Right;

        public void SetMovementKeys(Keyboard.Key up, Keyboard.Key down, Keyboard.Key left, Keyboard.Key right) {
            this.upKey = up;
            this.downKey = down;
            this.leftKey = left;
            this.rightKey = right;
        }

        public void SetKeymap(Dictionary<Keyboard.Key, Button> newKeyMap) {
            this.keyMap = newKeyMap;
        }

        private InputManager()
        {
            this.currentState = new Dictionary<Button, bool>();
            foreach (object obj in Enum.GetValues(typeof(Button)))
            {
                Button key = (Button)obj;
                this.currentState.Add(key, false);
            }
            this.enabled = true;
        }

        #region Window

        /// <summary>
        /// Attaches the Input Manager to a window. This is where it'll get the inputs from.
        /// </summary>
        /// <param name="window">The window to receive inputs from.</param>
        public void AttachToWindow(Window window)
        {
            window.SetKeyRepeatEnabled(false);
            window.JoystickButtonPressed += this.JoystickButtonPressed;
            window.JoystickButtonReleased += this.JoystickButtonReleased;
            window.JoystickMoved += this.JoystickMoved;
            window.JoystickConnected += this.JoystickConnected;
            window.JoystickDisconnected += this.JoystickDisconnected;
            window.KeyPressed += this.KeyPressed;
            window.KeyReleased += this.KeyReleased;
        }

        /// <summary>
        /// Detaches the Input Manager from a window. This means that it will not receive any inputs until it is attached to another window.
        /// </summary>
        /// <param name="window">The window to detach from.</param>
        public void DetachFromWindow(Window window)
        {
            window.JoystickButtonPressed -= this.JoystickButtonPressed;
            window.JoystickButtonReleased -= this.JoystickButtonReleased;
            window.JoystickMoved -= this.JoystickMoved;
            window.JoystickConnected -= this.JoystickConnected;
            window.JoystickDisconnected -= this.JoystickDisconnected;
            window.KeyPressed -= this.KeyPressed;
            window.KeyReleased -= this.KeyReleased;
        }

        #endregion

        #region Keyboard
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (this.keyMap.ContainsKey(e.Code))
            {
                Button button = this.keyMap[e.Code];
                if (this.enabled && !this.currentState[button])
                {
                    // i love nullable!
                    ButtonPressed?.Invoke(this, button);
                }
                this.currentState[button] = true;
                return;
            }

            bool keyDown = false;


            if (e.Code == leftKey)
            {
                this.leftPress = true;
                keyDown = true;
            }
            if (e.Code == rightKey)
            {
                this.rightPress = true;
                keyDown = true;

            }
            if (e.Code == upKey)
            {
                this.upPress = true;
                keyDown = true;
            }
            if (e.Code == downKey)
            {
                this.downPress = true;
                keyDown = true;
            }
            
            // old implementation
            /*switch (e.Code)
            {
                case Keyboard.Key.Left:
                    this.leftPress = true;
                    keyDown = true;
                    break;
                case Keyboard.Key.Right:
                    this.rightPress = true;
                    keyDown = true;
                    break;
                case Keyboard.Key.Up:
                    this.upPress = true;
                    keyDown = true;
                    break;
                case Keyboard.Key.Down:
                    this.downPress = true;
                    keyDown = true;
                    break;
            }*/

            //      this.xKeyAxis
            this.xKeyAxis = (this.leftPress ? -1f : 0f) + (this.rightPress ? 1f : 0f);
            this.yKeyAxis = (this.upPress ? -1f : 0f) + (this.downPress ? 1f : 0f);
            if (this.enabled && keyDown)
            {
                AxisPressed?.Invoke(this, this.Axis);
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (this.keyMap.ContainsKey(e.Code))
            {
                Button button = this.keyMap[e.Code];
                if (this.enabled && this.ButtonReleased != null)
                {
                    this.ButtonReleased(this, button);
                }
                this.currentState[button] = false;
                return;
            }

            bool released = false;

            switch (e.Code)
            {
                case Keyboard.Key.Left:
                    this.leftPress = false;
                    released = true;
                    break;
                case Keyboard.Key.Right:
                    this.rightPress = false;
                    released = true;
                    break;
                case Keyboard.Key.Up:
                    this.upPress = false;
                    released = true;
                    break;
                case Keyboard.Key.Down:
                    this.downPress = false;
                    released = true;
                    break;
            }
            this.xKeyAxis = (this.leftPress ? -1f : 0f) + (this.rightPress ? 1f : 0f);
            this.yKeyAxis = (this.upPress ? -1f : 0f) + (this.downPress ? 1f : 0f);
            if (this.enabled && released)
            {
                AxisReleased?.Invoke(this, this.Axis);
            }
        }
        #endregion

        private const float dead_zone = 0.5f;
        #region Controller
        private void JoystickMoved(object sender, JoystickMoveEventArgs e)
        {
            Joystick.Axis axis = e.Axis;
            switch (axis)
            {
                case Joystick.Axis.X:
                    this.xAxis = Math.Max(-1f, Math.Min(1f, e.Position / 70f));
                    if (this.xAxis > 0f && this.xAxis < dead_zone)
                    {
                        this.xAxis = 0f;
                    }
                    if (this.xAxis < 0f && this.xAxis > -dead_zone)
                    {
                        this.xAxis = 0f;
                    }
                    break;
                case Joystick.Axis.Y:
                    this.yAxis = Math.Max(-1f, Math.Min(1f, e.Position / 70f));
                    if (this.yAxis > 0f && this.yAxis < dead_zone)
                    {
                        this.yAxis = 0f;
                    }
                    if (this.yAxis < 0f && this.yAxis > -dead_zone)
                    {
                        this.yAxis = 0f;
                    }
                    break;
                default:
                    switch (axis)
                    {
                        case Joystick.Axis.PovX:
                            this.xAxis = Math.Max(-1f, Math.Min(1f, e.Position));
                            break;
                        case Joystick.Axis.PovY:
                            this.yAxis = Math.Max(-1f, Math.Min(1f, -e.Position));
                            break;
                    }
                    break;
            }
            this.axisZeroLast = this.axisZero;
            this.axisZero = (this.xAxis == 0f && this.yAxis == 0f);

            bool axisPressed = this.axisZeroLast && !this.axisZero;
            
            if (this.enabled && axisPressed)
            {
                AxisPressed?.Invoke(this, this.Axis);
                return;
            }

            bool axisReleased = !this.axisZeroLast && this.axisZero;
            if (this.enabled && axisReleased)
            {
                AxisReleased?.Invoke(this, this.Axis);
            }
        }

        private void JoystickConnected(object sender, JoystickConnectEventArgs e)
        {
            Joystick.Update();
            this.currentControllerIdentification = Joystick.GetIdentification(e.JoystickId);
            Debug.LogInfo($"Gamepad {e.JoystickId} connected: {currentControllerIdentification.Name} ({currentControllerIdentification.VendorId}, {currentControllerIdentification.ProductId})");
        }

        private void JoystickDisconnected(object sender, JoystickConnectEventArgs e)
        {
            Debug.LogInfo($"Gamepad {e.JoystickId} disconnected: {currentControllerIdentification.Name} ({currentControllerIdentification.VendorId}, {currentControllerIdentification.ProductId})");
            currentControllerIdentification = default;
        }

        private void JoystickButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            if (!this.joyMap.ContainsKey(e.Button))
            {
                return;
            }
            Button button = this.joyMap[e.Button];
            this.currentState[button] = true;
            if (this.enabled)
            {
                ButtonPressed?.Invoke(this, button);
            }
        }

        private void JoystickButtonReleased(object sender, JoystickButtonEventArgs e)
        {
            if (!this.joyMap.ContainsKey(e.Button))
            {
                return;
            }
            Button button = this.joyMap[e.Button];
            this.currentState[button] = false;
            if (this.enabled )
            {
                ButtonReleased?.Invoke(this, button);
            }
            }
        #endregion

        #region Mouse helpers
        /// <summary>
        /// Sets the position of the mouse relative to the game window.
        /// </summary>
        /// <param name="position">The position to set the mouse to.</param>
        public static void SetMousePosition(Vector2f position)
        {
            // This is stupid, let me explain:
            // We want a pixel location of where the mouse is relative to the game's window
            // Here's the problem: The scale of the screen
            float scaleFactor = Engine.ScreenScale;
            if (Engine.Fullscreen)
            {
                VideoMode desktopMode;
                desktopMode = VideoMode.DesktopMode;
                scaleFactor = Math.Min(desktopMode.Width / Engine.SCREEN_WIDTH, desktopMode.Height / Engine.SCREEN_HEIGHT);
            }
            Mouse.SetPosition((Vector2i)(position * scaleFactor));// * scaleFactor;
        }

        /// <summary>
        /// Gets the position of the mouse relative to the game window.
        /// </summary>
        /// <returns>The position of the mouse relative to the game window.</returns>
        public static Vector2f GetMousePosition() {
            // had a really long winded thing written but i'll shorten it
            // the mouse position is not relative to the game's window
            // what is (69, 69) in game space is not the same in monitor space
            // this function is translating monitor space to window space.
            if (Engine.Fullscreen) {
                VideoMode desktopMode;
                desktopMode = VideoMode.DesktopMode;
                float fullScreenMin = Math.Min(desktopMode.Width / Engine.SCREEN_WIDTH, desktopMode.Height / Engine.SCREEN_HEIGHT);
                return (Vector2f)Mouse.GetPosition(Engine.Window) / fullScreenMin;
            }
            return (Vector2f)Mouse.GetPosition(Engine.Window) / Engine.ScreenScale;
        }
        #endregion



        private Dictionary<Keyboard.Key, Button> keyMap = new Dictionary<Keyboard.Key, Button>
        {
            {
                Keyboard.Key.Z,
                Button.A
            },
            {
                Keyboard.Key.X,
                Button.B
            },
            {
                Keyboard.Key.S,
                Button.X
            },
            {
                Keyboard.Key.D,
                Button.Y
            },
            {
                Keyboard.Key.A,
                Button.L
            },
            {
                Keyboard.Key.F,
                Button.R
            },
            {
                Keyboard.Key.Enter,
                Button.Start
            },
            {
                Keyboard.Key.Backspace,
                Button.Select
            },
            {
                Keyboard.Key.Escape,
                Button.Escape
            },
            {
                Keyboard.Key.Tilde,
                Button.Tilde
            },
            {
                Keyboard.Key.F1,
                Button.F1
            },
            {
                Keyboard.Key.F2,
                Button.F2
            },
            {
                Keyboard.Key.F3,
                Button.F3
            },
            {
                Keyboard.Key.F4,
                Button.F4
            },
            {
                Keyboard.Key.F5,
                Button.F5
            },
            {
                Keyboard.Key.F6,
                Button.F6
            },
            {
                Keyboard.Key.F7,
                Button.F7
            },
            {
                Keyboard.Key.F8,
                Button.F8
            },
            {
                Keyboard.Key.F9,
                Button.F9
            },
            {
                Keyboard.Key.F10,
                Button.F10
            },
            {
                Keyboard.Key.F11,
                Button.F11
            },
            {
                Keyboard.Key.F12,
                Button.F12
            },
            {
                Keyboard.Key.Num0,
                Button.Zero
            },
            {
                Keyboard.Key.Num1,
                Button.One
            },
            {
                Keyboard.Key.Num2,
                Button.Two
            },
            {
                Keyboard.Key.Num3,
                Button.Three
            },
            {
                Keyboard.Key.Num4,
                Button.Four
            },
            {
                Keyboard.Key.Num5,
                Button.Five
            },
            {
                Keyboard.Key.Num6,
                Button.Six
            },
            {
                Keyboard.Key.Num7,
                Button.Seven
            },
            {
                Keyboard.Key.Num8,
                Button.Eight
            },
            {
                Keyboard.Key.Num9,
                Button.Nine
            }
        };

        private Dictionary<uint, Button> joyMap = new Dictionary<uint, Button>
        {
            {
                0U,
                Button.A
            },
            {
                1U,
                Button.B
            },
            {
                2U,
                Button.X
            },
            {
                3U,
                Button.Y
            },
            {
                4U,
                Button.L
            },
            {
                5U,
                Button.R
            },
            {
                6U,
                Button.Select
            },
            {
                7U,
                Button.Start
            },
            {
                8U,
                Button.Tilde
            }
        };

        private Dictionary<Button, bool> currentState;
    }
}
