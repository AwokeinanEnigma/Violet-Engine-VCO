using System;
using System.Collections.Generic;
using Violet.Audio;
using Violet.Utility;

namespace VCO.Overworld
{
    internal class FootstepPlayer : IDisposable
    {
        // (get) Token: 0x060005D1 RID: 1489 RVA: 0x00022B7D File Offset: 0x00020D7D
        // (set) Token: 0x060005D2 RID: 1490 RVA: 0x00022B85 File Offset: 0x00020D85
        public TerrainType Terrain
        {
            get => this.terrainType;
            set => this.terrainType = value;
        }
        public FootstepPlayer()
        {
            this.stepCount = 0;
            this.footstepMap = new Dictionary<TerrainType, VioletSound[]>();
            this.terrainType = TerrainType.Tile;
            this.isPaused = false;
            this.timerIndex = -1;
            FrameTimerManager.Instance.OnTimerEnd += this.TimerEnd;
            VioletSound[] value = new VioletSound[]
            {
                this.Load("stepGrass1"),
                this.Load("stepGrass2")
            };
            this.footstepMap.Add(TerrainType.Grass, value);
            this.footstepMap.Add(TerrainType.Moss, value);
            VioletSound[] value2 = new VioletSound[]
            {
                this.Load("stepTile1"),
                this.Load("stepTile2")
            };
            this.footstepMap.Add(TerrainType.Tile, value2);
            this.footstepMap.Add(TerrainType.Stone, value2);
        }
        ~FootstepPlayer()
        {
            this.Dispose(false);
        }
        private VioletSound Load(string name)
        {
            return AudioManager.Instance.Use(DataHandler.instance.Load(name + ".wav"), AudioType.Sound);
        }
        private void TimerEnd(int timerIndex)
        {
            if (this.timerIndex == timerIndex)
            {
                this.Play();
                this.timerIndex = FrameTimerManager.Instance.StartTimer(12);
            }
        }
        private void Play()
        {
            if (!this.disposed && !this.isPaused && this.footstepMap.TryGetValue(this.terrainType, out VioletSound[] array))
            {
                this.lastSound = array[this.stepCount % array.Length];
                this.lastSound.Play();
                this.stepCount++;
            }
        }
        public void Start()
        {
            if (!this.disposed)
            {
                if (!this.isPaused)
                {
                    this.timerIndex = FrameTimerManager.Instance.StartTimer(0);
                }
                this.isPaused = false;
            }
        }
        public void Resume()
        {
            if (!this.disposed)
            {
                this.isPaused = false;
            }
        }
        public void Pause()
        {
            if (!this.disposed)
            {
                this.isPaused = true;
                if (this.lastSound != null)
                {
                    this.lastSound.Stop();
                }
            }
        }
        public void Stop()
        {
            if (!this.disposed && this.lastSound != null)
            {
                this.lastSound.Stop();
                this.lastSound = null;
                FrameTimerManager.Instance.Cancel(this.timerIndex);
                this.timerIndex = -1;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                foreach (VioletSound[] array in this.footstepMap.Values)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        AudioManager.Instance.Unuse(array[i]);
                    }
                }
                FrameTimerManager.Instance.OnTimerEnd -= this.TimerEnd;
            }
            this.disposed = true;
        }
        private const string EXTENSION = ".wav";
        private const int FOOTSTEP_TIMER_DURATION = 12;
        private bool disposed;
        private readonly Dictionary<TerrainType, VioletSound[]> footstepMap;
        private int stepCount;
        private VioletSound lastSound;
        private TerrainType terrainType;
        private int timerIndex;
        private bool isPaused;
    }
}
