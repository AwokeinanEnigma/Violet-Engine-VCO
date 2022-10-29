using System.Collections.Generic;

namespace Violet.Utility
{
    public class FrameTimerManager
    {
        public static FrameTimerManager Instance
        {
            get
            {
                if (FrameTimerManager.instance == null)
                {
                    FrameTimerManager.instance = new FrameTimerManager();
                }
                return FrameTimerManager.instance;
            }
        }

        public event FrameTimerManager.OnTimerEndHandler OnTimerEnd;

        private FrameTimerManager()
        {
            this.timers = new List<FrameTimerManager.Timer>();
        }

        public int StartTimer(int duration)
        {
            long frame = Engine.Frame;
            FrameTimerManager.Timer item = new FrameTimerManager.Timer
            {
                End = frame + duration,
                Index = ++this.timerCounter
            };
            this.timers.Add(item);
            return this.timerCounter;
        }

        public void Cancel(int timerIndex)
        {
            for (int i = 0; i < this.timers.Count; i++)
            {
                if (this.timers[i].Index == timerIndex)
                {
                    this.timers.RemoveAt(i);
                    return;
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < this.timers.Count; i++)
            {
                if (this.timers[i].End < Engine.Frame)
                {
                    if (this.OnTimerEnd != null)
                    {
                        this.OnTimerEnd(this.timers[i].Index);
                    }
                    this.timers.RemoveAt(i);
                    i--;
                }
            }
        }

        private static FrameTimerManager instance;

        private List<FrameTimerManager.Timer> timers;

        private int timerCounter;

        private struct Timer
        {
            public long End;

            public int Index;
        }

        public delegate void OnTimerEndHandler(int timerIndex);
    }
}
