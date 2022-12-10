using MoonSharp.Interpreter;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Lua;
using Violet.Scenes;
using Violet.Scenes.Transitions;
using Violet.Utility;
using Button = Violet.Input.Button;

namespace Violet
{
    /// <summary>
    /// "You must be ahead to quit. Too many people quit when they’re behind instead of attempting to get ahead. Failure!"
    /// </summary>
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

        /// <summary>
        /// How much FPS are we running at?
        /// </summary>
        public static float FPS
        {
            get
            {
                return fps;
            }
        }

        /// <summary>
        /// Out of ten, what frame are we on?
        /// </summary>
        public static long Frame
        {
            get
            {
                return frameIndex;
            }
        }

        /// <summary>
        /// If nothing can be rendered, what color should be rendered in place? Think of out of bound maps.
        /// </summary>
        public static SFML.Graphics.Color ClearColor { get; set; }

        /// <summary>
        /// In seconds, how long has the game been running for?
        /// </summary>
        public static int SessionTime
        {
            get
            {
                return (int)TimeSpan.FromTicks(DateTime.Now.Ticks - startTicks).TotalSeconds;
            }
        }
        #endregion




        private static uint frameBufferScale;
        private static RenderWindow window;
        private static RenderTexture frameBuffer;
        private static RenderStates frameBufferState;
        private static VertexArray frameBufferVertArray;
        private static Random rand;
        private static FontData defaultFont;
        private static Text debugText;
        private static bool quit;


        #region Active game fields
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
        #endregion

        #region Full screen
        // Are we fullscreen?
        private static bool isFullscreen;
        // 
        private static bool switchScreenMode;

        #endregion

        #region  Screen Info

        /// <summary>
        /// Width of the screen in pixels
        /// </summary>
        public static uint SCREEN_WIDTH { get { return screen_width; } }

        /// <summary>
        /// Height of the screen in pixels
        /// </summary>        
        public static uint SCREEN_HEIGHT { get { return screen_height; } }

        public static Vector2f SCREEN_SIZE;

        /// <summary>
        /// This is the SCREEN_SIZE divided by two.
        /// </summary>
        public static Vector2f HALF_SCREEN_SIZE;
        #endregion

        #region Game Data
        /// <summary>
        /// What version of OpenGL is required to run the game?
        /// </summary>
        private static uint required_opengl_version;

        /// <summary>
        /// Width of the screen in pixels
        /// </summary>
        private static uint screen_width;

        /// <summary>
        /// Height of the screen in pixels
        /// </summary>        
        private static uint screen_height;

        /// <summary>
        /// If an icon is set, what's the size of the icon?
        /// </summary>
        private static uint icon_size;

        /// <summary>
        /// Treat this as you
        /// </summary>
        private static uint target_framerate;

        /// <summary>
        /// Data to change specific aspects of the engine such as the resolution and target frame rate.
        /// </summary>
        public struct EngineInitializationData {
            /// <summary>
            /// Width of the screen in pixels
            /// </summary>
            public uint screen_width;

            /// <summary>
            /// Height of the screen in pixels
            /// </summary>        
            public uint screen_height;

            /// <summary>
            /// What's the size of the game's icon?
            /// </summary>
            public uint icon_size;

            /// <summary>
            /// What frame rate *should* the game be running at?
            /// </summary>
            public uint target_framerate;

            /// <summary>
            /// OpenGL version required to run the game. Usually is 2.1 unless you're doing something special.
            /// </summary>
            public uint required_opengl_version;

            /// <summary>
            /// When the game starts, should it start in full screen?
            /// </summary>
            public bool start_fullscreen;

            /// <summary>
            /// Should the game start in Vsync?
            /// </summary>
            public bool start_vsync;

            /// <summary>
            /// Base frame buffer scale
            /// </summary>
            public uint base_frame_buffer_scale;
        }

        #endregion

        /// <summary>
        /// Gets the current OpenGl version
        /// </summary>
        /// <returns>Returns the OpenGL version as a decimal</returns>
        public static uint OpenGLVersion()
        {
            // todo:
            // this causes some weird pinvoke exception
            // not sure why but ok?
            // Debug.LogDebug($"Running OpenGL version major {window.Settings.MajorVersion} minor {window.Settings.MinorVersion}");
            
            return 3;//  decimal.Parse($"{}.{window.Settings.MinorVersion}");
        }

        /// <summary>
        /// Initalizes the engine using EngineInitializationData 
        /// </summary>
        /// <param name="data">Data the game needs to function, like screen width and height, maximum framerate, and other such things.</param>
        public static void Initialize(EngineInitializationData data)
        {
            void PullFromInitializationData() {
                screen_width = data.screen_width;
                screen_height = data.screen_height;
                icon_size = data.icon_size;
                target_framerate = data.target_framerate;
                required_opengl_version = data.required_opengl_version;
                frameBufferScale = data.base_frame_buffer_scale;

                SCREEN_SIZE = new Vector2f(screen_width, screen_height);
                HALF_SCREEN_SIZE = new Vector2f(screen_width / 2, screen_height / 2);
            }
            void SetFrameBuffer()
            {
                frameBuffer = new RenderTexture(screen_width, screen_height);
                frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, frameBuffer.Texture, null);
                frameBufferVertArray = new VertexArray(PrimitiveType.Quads, 4U);
            }
            void SetFrameBuffArray()
            {

                frameBufferVertArray[0U] = new Vertex(new Vector2f(-Engine.HALF_SCREEN_SIZE.X, -HALF_SCREEN_SIZE.Y), new Vector2f(0f, 0f));
                frameBufferVertArray[1U] = new Vertex(new Vector2f(Engine.HALF_SCREEN_SIZE.X, -HALF_SCREEN_SIZE.Y), new Vector2f(screen_width, 0f));
                frameBufferVertArray[2U] = new Vertex(new Vector2f(Engine.HALF_SCREEN_SIZE.X, HALF_SCREEN_SIZE.Y), new Vector2f(screen_width, screen_height));
                frameBufferVertArray[3U] = new Vertex(new Vector2f(-Engine.HALF_SCREEN_SIZE.X, HALF_SCREEN_SIZE.Y), new Vector2f(0f, screen_height));
            }

            frameStopwatch = Stopwatch.StartNew();
            startTicks = DateTime.Now.Ticks;

            PullFromInitializationData();
            SetFrameBuffer();

            // set these to the initialization data.
            bool vsync = data.start_vsync;
            bool goFullscreen = data.start_fullscreen;

            SetWindow(goFullscreen, vsync);
            InputManager.Instance.ButtonPressed += OnButtonPressed;

            SetFrameBuffArray();

            rand = new Random();
            defaultFont = new FontData();
            debugText = new Text(string.Empty, defaultFont.Font, defaultFont.Size);
            debugText.FillColor = SFML.Graphics.Color.Blue;
            //debugText.FillColor.
            ClearColor = SFML.Graphics.Color.Black;


            Debug.Initialize();

            // for now, register empty
            LuaManager.Initialize(Paths.DATA_LUA);
            Script.DefaultOptions.DebugPrint = s => Debug.LogLua(s);

            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());

            decimal openGlV = OpenGLVersion();
            if (openGlV < required_opengl_version)
            {
                string message = $"OpenGL version {required_opengl_version} or higher is required. This system has version {openGlV}.";
                throw new InvalidOperationException(message);
            }
            //Debug.LogD($"OpenGL v{window.Settings.MajorVersion}.{window.Settings.MinorVersion}");
            fpsString = new StringBuilder(32);
            SetCursorTimer(90);
            Running = true;
        }

        /// <summary>
        /// Initalizes the engine, and loads the necessary EngineInitializationData from enginedata.ini
        /// </summary>
        public static void Initialize()
        {
            void SetFrameBuffer()
            {
                frameBuffer = new RenderTexture(screen_width, screen_height);
                frameBufferState = new RenderStates(BlendMode.Alpha, Transform.Identity, frameBuffer.Texture, null);
                frameBufferVertArray = new VertexArray(PrimitiveType.Quads, 4U);
            }
            void SetFrameBuffArray()
            {

                frameBufferVertArray[0U] = new Vertex(new Vector2f(-Engine.HALF_SCREEN_SIZE.X, -HALF_SCREEN_SIZE.Y), new Vector2f(0f, 0f));
                frameBufferVertArray[1U] = new Vertex(new Vector2f(Engine.HALF_SCREEN_SIZE.X, -HALF_SCREEN_SIZE.Y), new Vector2f(screen_width, 0f));
                frameBufferVertArray[2U] = new Vertex(new Vector2f(Engine.HALF_SCREEN_SIZE.X, HALF_SCREEN_SIZE.Y), new Vector2f(screen_width, screen_height));
                frameBufferVertArray[3U] = new Vertex(new Vector2f(-Engine.HALF_SCREEN_SIZE.X, HALF_SCREEN_SIZE.Y), new Vector2f(0f, screen_height));
            }

            frameStopwatch = Stopwatch.StartNew();
            startTicks = DateTime.Now.Ticks;

            Debug.Initialize();

            Debug.LogEngine("Loading from enginedata.ini");


            // create new inifile
            IniFile ini = new IniFile();

            //load enginedata.ini
            ini.Load(Paths.DATA + "enginedata.ini");

            void PullFromEngineDataIniFile()
            {
                //set a buncha values
                screen_width = ini["enginedata"]["screen_width"].ToUInt();
                screen_height = ini["enginedata"]["screen_height"].ToUInt();
                icon_size = ini["enginedata"]["icon_size"].ToUInt();
                target_framerate = ini["enginedata"]["target_framerate"].ToUInt();
                required_opengl_version = ini["enginedata"]["required_opengl_version"].ToUInt();
                frameBufferScale = ini["enginedata"]["base_frame_buffer_scale"].ToUInt();

                // i could just use a get set for these but i'm lazy.
                SCREEN_SIZE = new Vector2f(screen_width, screen_height);
                HALF_SCREEN_SIZE = new Vector2f(screen_width / 2, screen_height / 2);
            }

            PullFromEngineDataIniFile();

            // before we set these fields using the args, set them to the initialization data.
            bool vsync = ini["enginedata"]["start_vsync"].ToBool();
            bool goFullscreen = ini["enginedata"]["start_fullscreen"].ToBool();

            SetFrameBuffer();

            SetWindow(goFullscreen, vsync);
            InputManager.Instance.ButtonPressed += OnButtonPressed;

            SetFrameBuffArray();

            rand = new Random();
            defaultFont = new FontData();
            debugText = new Text(string.Empty, defaultFont.Font, defaultFont.Size);
            debugText.FillColor = SFML.Graphics.Color.Blue;
            ClearColor = SFML.Graphics.Color.Black;

            // for now, register empty
            LuaManager.Initialize(Paths.DATA_LUA);
            Script.DefaultOptions.DebugPrint = s => Debug.LogLua(s);

            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());

            decimal openGlV = OpenGLVersion();
            if (openGlV < required_opengl_version)
            {
                string message = $"OpenGL version {required_opengl_version} or higher is required. This system has version {openGlV}.";
                throw new InvalidOperationException(message);
            }
            //Debug.LogD($"OpenGL v{window.Settings.MajorVersion}.{window.Settings.MinorVersion}");
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

                float fullScreenMin = Math.Min(desktopMode.Width / screen_width, desktopMode.Height / screen_height);
                float num4 = (desktopMode.Width - screen_width * fullScreenMin) / 2f;
                float num5 = (desktopMode.Height - screen_height * fullScreenMin) / 2f;

                int width = (int)(HALF_SCREEN_SIZE.X * fullScreenMin);
                int height = (int)(HALF_SCREEN_SIZE.Y * fullScreenMin);
                frameBufferState.Transform = new Transform(cos * fullScreenMin, sin, num4 + width, -sin, cos * fullScreenMin, num5 + height, 0f, 0f, 1f);
            }
            else
            {

                int halfWidthScale = (int)(HALF_SCREEN_SIZE.X * ScreenScale);
                int halfHeightScale = (int)(HALF_SCREEN_SIZE.Y * ScreenScale);
                style = Styles.Close;
                desktopMode = new VideoMode(screen_width * frameBufferScale, screen_height * frameBufferScale);
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
                //window.SetFramerateLimit(target_framerate);
                window.SetVerticalSyncEnabled(true);
            }
            else
            {
                window.SetFramerateLimit(target_framerate);

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
            Debug.LogInfo("Screenshot saved as \"{0}\"", text);
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
                System.Windows.Forms.Clipboard.SetImage(image2);
            }
            Debug.LogInfo("Screenshot copied to clipboard");
        }

        public static void OnButtonPressed(InputManager sender, Button b)
        {
            switch (b)
            {
                case Button.Escape:
                    if (!isFullscreen)
                    {
                        quit = true;
                        return;
                    }
                    switchScreenMode = true;
                    isFullscreen = !isFullscreen;
                    return;
                case Button.Tilde:
                    Debug.Log("Hey!");
                    debugDisplay = !debugDisplay;
                    return;
                case Button.F1:
                case Button.F2:
                case Button.F3:
                case Button.F6:
                case Button.F7:
                    break;
                case Button.F4:
                    switchScreenMode = true;
                    isFullscreen = !isFullscreen;
                    return;
                case Button.F5:
                    frameBufferScale = frameBufferScale % 5U + 1U;
                    Debug.LogInfo($"frame buffer scale is {frameBufferScale}");
                    switchScreenMode = true;
                    return;
                case Button.F8:
                    {
                        TakeScreenshot();
                        return;
                    }
                case Button.F9:
                    {
                        RenderPNG();
                        return;
                    }
                default:
                    if (b != Button.F12)
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

                // Update cruciel game instances
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


        }

        public static void Render() {




            if (debugDisplay)
            {
                if (frameIndex % 10L == 0L)
                {
                    //megabytesUsed = ;
                    //megabytesUsed *= 0.001;
                    fpsString.Clear();
                    fpsString.AppendFormat("GC: {0:D5} KB\n", GC.GetTotalMemory(false) / 1024L);
                    // fpsString.AppendFormat("FGC: {0:D5} KB\n", GC.GetTotalMemory(true) / 1024L);
                    fpsString.Append($"MGC: {(GC.GetTotalMemory(true) / 1024L) * 0.001}MB\n");
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
                window.SetIcon(icon_size, icon_size, iconFile.GetBytesForSize(32));
            }
        }
    }
}
