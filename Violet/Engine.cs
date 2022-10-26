using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Scenes;
using Violet.Scenes.Transitions;
using Violet.Utility;

namespace Violet
{
    public static class Engine
    {
        #region Static outward fields 
        public static RenderWindow Window
        {
            get
            {
                return window;
            }
        }
        public static RenderTexture FrameBuffer
        {
            get
            {
                return frameBuffer;
            }
        }
        public static Random Random
        {
            get
            {
                return rand;
            }
        }
        public static FontData DefaultFont
        {
            get
            {
                return defaultFont;
            }
        }

        public static bool Running { get; private set; }

        /// <summary>
        /// Multiply this by the SCREEN_WIDTH and SCREEN_HEIGHT.
        /// I.
        /// </summary>
        public static uint ScreenScale
        {
            get
            {
                return frameBufferScale;
            }
            set
            {
                frameBufferScale = Math.Max(0U, value);
                switchScreenMode = true;
            }
        }

        /// <summary>
        /// Are we fullscreen?
        /// </summary>
        public static bool Fullscreen
        {
            get
            {
                return isFullscreen;
            }
            set
            {
                isFullscreen = value;
                switchScreenMode = true;
            }
        }
        
        public static float FPS
        {
            get
            {
                return fps;
            }
        }
        
        public static long Frame
        {
            get
            {
                return frameIndex;
            }
        }
        
        public static SFML.Graphics.Color ClearColor { get; set; }
        
        public static int SessionTime
        {
            get
            {
                return (int)TimeSpan.FromTicks(DateTime.Now.Ticks - startTicks).TotalSeconds;
            }
        }
        #endregion


        public const string CAPTION = "Voyage Carpe Omnia";

        private const decimal REQUIRED_OGL_VERSION = 2.1m;

        private static uint frameBufferScale = 2U;
        private static RenderWindow window;
        private static RenderTexture frameBuffer;
        private static RenderStates frameBufferState;
        private static VertexArray frameBufferVertArray;
        private static Random rand;
        private static FontData defaultFont;
        private static Text debugText;
        private static bool quit;

        private static float fps;
        private static float fpsAverage;
        private static long frameIndex;
        private static long startTicks;
        private static long cursorTimer;
        private static bool showCursor;
        private static long clickFrame = long.MinValue;
        private static IconFile iconFile;
        private static StringBuilder fpsString;
        private static float screenAngle = 0f;
        public static bool debugDisplay;
        private static Stopwatch frameStopwatch;

        #region Full screen
        // Are we fullscreen?
        private static bool isFullscreen;
        // 
        private static bool switchScreenMode;

        #endregion

        #region  Screen Info
        // Width of the screen in pixels
        public const uint SCREEN_WIDTH = 320U;

        // Height of the screen in pixels
        public const uint SCREEN_HEIGHT = 180U;
        public const uint HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
        public const uint HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;
        private const int CURSOR_TIMEOUT = 90;
        private const int ICON_SIZE = 32;
        private const int DOUBLE_CLICK_TIME = 20;
        public static readonly Vector2f SCREEN_SIZE = new Vector2f(320f, 180f);
        public static readonly Vector2f HALF_SCREEN_SIZE = SCREEN_SIZE / 2f;

        public const int TARGET_FRAMERATE = 60;
        #endregion

        public const decimal REQUIRED_OPENGL_VERSION = 2.1m;

        /// <summary>
        /// Gets the current OpenGl version
        /// </summary>
        /// <returns>Returns the OpenGL version as a decimal</returns>
        public static decimal OpenGLVersion()
        {
            return decimal.Parse($"{window.Settings.MajorVersion}.{window.Settings.MinorVersion}");
        }

        /// <summary>
        /// Initalizes the engine
        /// </summary>
        /// <param name="args">Parameters that the game was initialized with.</param>
        public static void Initialize(string[] args)
        {
            frameStopwatch = Stopwatch.StartNew();
            startTicks = DateTime.Now.Ticks;

            bool vsync = false;
            bool goFullscreen = true;

            void SetScreenMode()
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string a;
                    if ((a = args[i]) != null)
                    {
                        if (!(a == "-fullscreen"))
                        {
                            if (!(a == "-vsync"))
                            {
                                if (a == "-scale")
                                {
                                    if (uint.TryParse(args[++i], out uint screenScale))
                                    {
                                        ScreenScale = screenScale;
                                    }
                                }
                            }
                            else
                            {
                                vsync = true;
                            }
                        }
                        else
                        {
                            goFullscreen = true;
                        }
                    }
                }
            }
            void SetFrameBuffer()
            {
                frameBuffer = new RenderTexture(SCREEN_WIDTH, SCREEN_HEIGHT);
                frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, frameBuffer.Texture, null);
                frameBufferVertArray = new VertexArray(PrimitiveType.Quads, 4U);
            }
            void SetFrameBuffArray()
            {

                frameBufferVertArray[0U] = new Vertex(new Vector2f(-HALF_SCREEN_WIDTH, -HALF_SCREEN_HEIGHT), new Vector2f(0f, 0f));
                frameBufferVertArray[1U] = new Vertex(new Vector2f(HALF_SCREEN_WIDTH, -HALF_SCREEN_HEIGHT), new Vector2f(SCREEN_WIDTH, 0f));
                frameBufferVertArray[2U] = new Vertex(new Vector2f(HALF_SCREEN_WIDTH, HALF_SCREEN_HEIGHT), new Vector2f(SCREEN_WIDTH, SCREEN_HEIGHT));
                frameBufferVertArray[3U] = new Vertex(new Vector2f(-HALF_SCREEN_WIDTH, HALF_SCREEN_HEIGHT), new Vector2f(0f, SCREEN_HEIGHT));
            }


            SetScreenMode();
            SetFrameBuffer();

            SetWindow(goFullscreen, vsync);
            InputManager.Instance.ButtonPressed += OnButtonPressed;

            SetFrameBuffArray();

            rand = new Random();
            defaultFont = new FontData();
            debugText = new Text(string.Empty, defaultFont.Font, defaultFont.Size);
            ClearColor = SFML.Graphics.Color.Black;
          
            Debug.Initialize();

            decimal openGlV = OpenGLVersion();
            if (openGlV < REQUIRED_OPENGL_VERSION)
            {
                string message = $"OpenGL version {REQUIRED_OPENGL_VERSION} or higher is required. This system has version {openGlV}.";
                throw new InvalidOperationException(message);
            }
            Debug.LDebug($"OpenGL v{window.Settings.MajorVersion}.{window.Settings.MinorVersion}");
            fpsString = new StringBuilder(32);
            SetCursorTimer(90);
            Running = true;
        }

        public static void StartSession()
        {
            startTicks = DateTime.Now.Ticks;
        }
        private static void SetCursorTimer(int duration)
        {
            cursorTimer = frameIndex + duration;
        }
        private static void SetWindow(bool goFullscreen, bool vsync)
        {
            // Kill our current window so we can create a new one
            if (window != null)
            {
                // Dettach everything from the current window
                window.Closed -= OnWindowClose;
                window.MouseMoved -= MouseMoved;
                InputManager.Instance.DetachFromWindow(window);
                
                // Kill it!
                window.Close();
                window.Dispose();
            }

            float cos = (float)Math.Cos(screenAngle);
            float sin = (float)Math.Sin(screenAngle);
            Styles style;
            VideoMode desktopMode;

            if (goFullscreen)
            {
                style = Styles.Fullscreen;
                desktopMode = VideoMode.DesktopMode;
               
                float fullScreenMin = Math.Min(desktopMode.Width / SCREEN_WIDTH, desktopMode.Height / SCREEN_HEIGHT);
                float num4 = (desktopMode.Width - SCREEN_WIDTH * fullScreenMin) / 2f;
                float num5 = (desktopMode.Height - SCREEN_HEIGHT * fullScreenMin) / 2f;
             
                int width = (int)(160f * fullScreenMin);
                int height = (int)(90f * fullScreenMin);
                frameBufferState.Transform = new Transform(cos * fullScreenMin, sin, num4 + width, -sin, cos * fullScreenMin, num5 + height, 0f, 0f, 1f);
            }
            else
            {

                int halfWidthScale = (int)(HALF_SCREEN_WIDTH * ScreenScale);
                int halfHeightScale = (int)(HALF_SCREEN_HEIGHT * ScreenScale);
                style = Styles.Close;
                desktopMode = new VideoMode(SCREEN_WIDTH * frameBufferScale, SCREEN_HEIGHT * frameBufferScale);
                frameBufferState.Transform = new Transform(cos * frameBufferScale, sin * frameBufferScale, halfWidthScale, -sin * frameBufferScale, cos * frameBufferScale, halfHeightScale, 0f, 0f, 1f);
            }

           
            window = new RenderWindow(desktopMode, "Voyage: Carpe Omnia", style);
            window.Closed += OnWindowClose;
            window.MouseMoved += MouseMoved;
            window.MouseButtonPressed += MouseButtonPressed;
            InputManager.Instance.AttachToWindow(window);
            window.SetMouseCursorVisible(!goFullscreen);

            if (vsync || goFullscreen)
            {
                window.SetFramerateLimit(TARGET_FRAMERATE);

                //window.SetVerticalSyncEnabled(true);
            }
            else
            {
                window.SetFramerateLimit(TARGET_FRAMERATE);
            
            }
            
            if (iconFile != null)
            {
                window.SetIcon(32U, 32U, iconFile.GetBytesForSize(32));
            }
        }
        private static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                showCursor = true;
                window.SetMouseCursorVisible(showCursor);
                SetCursorTimer(90);
                if (frameIndex < clickFrame + 20L)
                {
                    switchScreenMode = true;
                    isFullscreen = !isFullscreen;
                    clickFrame = long.MinValue;
                    return;
                }
                clickFrame = frameIndex;
            }
        }
        private static void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (!showCursor)
            {
                showCursor = true;
                window.SetMouseCursorVisible(showCursor);
            }
            SetCursorTimer(90);
        }
        public static void OnWindowClose(object sender, EventArgs e)
        {
            RenderWindow renderWindow = (RenderWindow)sender;
            renderWindow.Close();
            quit = true;
        }
        
        public static void RenderPNG()
        {
            SFML.Graphics.Image image3 = frameBuffer.Texture.CopyToImage();
            string text = string.Format("screenshot{0}.png", Directory.GetFiles("./", "screenshot*.png").Length);
            image3.SaveToFile(text);
            Debug.LInfo("Screenshot saved as \"{0}\"", text);
        }
         
        public static unsafe void TakeScreenshot()
        {
            SFML.Graphics.Image image = frameBuffer.Texture.CopyToImage();
            byte[] array = new byte[image.Pixels.Length];
            fixed (byte* pixels = image.Pixels, ptr = array)
            {
                for (int i = 0; i < array.Length; i += 4)
                {
                    ptr[i] = pixels[i + 2];
                    ptr[i + 1] = pixels[i + 1];
                    ptr[i + 2] = pixels[i];
                    ptr[i + 3] = pixels[i + 3];
                }

                IntPtr scan = new IntPtr(ptr);
                Bitmap image2 = new Bitmap((int)image.Size.X, (int)image.Size.Y, (int)(4U * image.Size.X), PixelFormat.Format32bppArgb, scan);
                Clipboard.SetImage(image2);
            }
            Debug.LInfo("Screenshot copied to clipboard");
        }

        public static void OnButtonPressed(InputManager sender, Violet.Input.Button b)
        {
            switch (b)
            {
                case Violet.Input.Button.Escape:
                    if (!isFullscreen)
                    {
                        quit = true;
                        return;
                    }
                    switchScreenMode = true;
                    isFullscreen = !isFullscreen;
                    return;
                case Violet.Input.Button.Tilde:
                    debugDisplay = !debugDisplay;
                    return;
                case Violet.Input.Button.F1:
                case Violet.Input.Button.F2:
                case Violet.Input.Button.F3:
                case Violet.Input.Button.F6:
                case Violet.Input.Button.F7:
                    break;
                case Violet.Input.Button.F4:
                    switchScreenMode = true;
                    isFullscreen = !isFullscreen;
                    return;
                case Violet.Input.Button.F5:
                    frameBufferScale = frameBufferScale % 5U + 1U;
                    Debug.LInfo($"frame buffer scale is {frameBufferScale}");
                    switchScreenMode = true;
                    return;
                case Violet.Input.Button.F8:
                {
                    TakeScreenshot();
                    return;
                }
                case Violet.Input.Button.F9:
                {
                    RenderPNG();
                    return;
                }
                default:
                    if (b != Violet.Input.Button.F12)
                    {
                        return;
                    }
                    if (!SceneManager.Instance.IsTransitioning)
                    {
                        SceneManager.Instance.Transition = new InstantTransition();
                        SceneManager.Instance.Pop();
                    }
                    break;
            }
        }

        public static void UpdateInstances()
        {
            SceneManager.Instance.Update();
            FrameTimerManager.Instance.Update();
            ViewManager.Instance.Update();
            ViewManager.Instance.UseView();
        }

        public static void Update()
        {
            frameStopwatch.Restart();
            if (switchScreenMode)
            {
                SetWindow(isFullscreen, false);
                switchScreenMode = false;
            }
            if (frameIndex > cursorTimer)
            {
                showCursor = false;
                window.SetMouseCursorVisible(showCursor);
                cursorTimer = long.MaxValue;
            }

            // This is wrapped in a try catch statement to detect errors and such.
			try
			{
                // Update our audio as soon as possible
                AudioManager.Instance.Update();
                
                // This makes input and other events from the window work
                window.DispatchEvents();

                // Update cruciel game classes
                UpdateInstances();

                // OpenGL shit, we have to clear our frame buffer before we can draw to it
                frameBuffer.Clear(ClearColor);

                //Finally, draw our scene.
                SceneManager.Instance.Draw();
            }
            // If we catch an empty stack exception, we just quit. This is because there's no next scene to go to, the game is finished!
			catch (EmptySceneStackException)
			{
				quit = true;
			}
            // If the exception is NOT an empty scene stack exception, we'll go to the error scene.
			catch (Exception ex)
			{
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new InstantTransition();
                SceneManager.Instance.Push(new ErrorScene(ex));
            }
            ViewManager.Instance.UseDefault();
            if (debugDisplay)
            {
                if (frameIndex % 10L == 0L)
                {
                    megabytesUsed = (GC.GetTotalMemory(true) / 1024L);
                    megabytesUsed *= 0.001;
                    fpsString.Clear();
                    fpsString.AppendFormat("GC: {0:D5} KB\n", GC.GetTotalMemory(false) / 1024L);
                   // fpsString.AppendFormat("FGC: {0:D5} KB\n", GC.GetTotalMemory(true) / 1024L);
                    fpsString.Append($"MGC: {megabytesUsed}MB\n");
                    fpsString.AppendFormat("FPS: {0:F1}", fpsAverage);
                    debugText.DisplayedString = fpsString.ToString();
                }
                frameBuffer.Draw(debugText);
            }
            frameBuffer.Display();
            window.Clear(SFML.Graphics.Color.Black);
            window.Draw(frameBufferVertArray, frameBufferState);
            window.Display();
            Running = (!SceneManager.Instance.IsEmpty && !quit);
            frameStopwatch.Stop();
            fps = 1f / frameStopwatch.ElapsedTicks * Stopwatch.Frequency;
            fpsAverage = (fpsAverage + fps) / 2f;
            frameIndex += 1L;
        }
        public static double megabytesUsed;
        public static void SetWindowIcon(string file)
        {
            if (File.Exists(file))
            {
                iconFile = new IconFile(file);
                window.SetIcon(32U, 32U, iconFile.GetBytesForSize(32));
            }
        }
    }
}
