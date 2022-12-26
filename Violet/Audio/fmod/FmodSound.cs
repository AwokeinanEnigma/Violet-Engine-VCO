using FMOD;
using System;

namespace Violet.Audio.fmod
{
    internal sealed class FmodSound : VioletSound
    {
        private Sound sound;
        private FMOD.System system;
        private Channel channel;
        private int callbackIndex;

        public override uint Position
        {
            get
            {
                if (this.channel == null)
                {
                    return 0;
                }

                FmodAudioManager.ERRCHECKCHANNEL(this.channel.getPosition(ref this.position, TIMEUNIT.MS));
                return this.position;
            }
            set
            {
                this.position = value;
                FmodAudioManager.ERRCHECKCHANNEL(this.channel.setPosition(this.position, TIMEUNIT.MS));
            }
        }

        public override bool IsPaused
        {
            get
            {
                if (this.channel == null)
                {
                    return false;
                }

                bool paused = false;
                FmodAudioManager.ERRCHECKCHANNEL(this.channel.getPaused(ref paused));
                return paused;
            }
        }

        public override float Volume
        {
            get => this.volume;
            set
            {
                this.volume = (this.type == AudioType.Stream ? AudioManager.Instance.MusicVolume : AudioManager.Instance.EffectsVolume) * value;
                if (this.channel == null)
                {
                    return;
                }

                FmodAudioManager.ERRCHECKCHANNEL(this.channel.setVolume(this.volume));
            }
        }

        public override int LoopCount
        {
            get
            {
                int loopcount = 0;
                FmodAudioManager.ERRCHECKSOUND(this.sound.getLoopCount(ref loopcount));
                return loopcount;
            }
            set => FmodAudioManager.ERRCHECKSOUND(this.sound.setLoopCount(value));
        }

        public FmodSound(
          ref FMOD.System system,
          string filename,
          AudioType type,
          uint loopBegin,
          uint loopEnd,
          int loopCount,
          float volume)
          : base(type, loopBegin, loopEnd, loopCount, volume, 1f)
        {
            this.system = system;
            this.name = filename;
            this.sound = new Sound();
            switch (this.type)
            {
                case AudioType.Sound:
                    FmodAudioManager.ERRCHECKSYSTEM(system.createSound(filename, MODE.LOOP_NORMAL | MODE._2D | MODE.SOFTWARE, ref this.sound));
                    FmodAudioManager.ERRCHECKSOUND(this.sound.setLoopCount(0));
                    break;
                case AudioType.Stream:
                    FmodAudioManager.ERRCHECKSYSTEM(system.createSound(filename, MODE.LOOP_NORMAL | MODE._2D | MODE.SOFTWARE | MODE.CREATESTREAM, ref this.sound));
                    this.LoopCount = -1;
                    break;
                case AudioType.Sound3d:
                    FmodAudioManager.ERRCHECKSYSTEM(system.createSound(filename, MODE.LOOP_NORMAL | MODE._3D | MODE.SOFTWARE, ref this.sound));
                    FmodAudioManager.ERRCHECKSOUND(this.sound.setLoopCount(0));
                    break;
            }
        }

        public override void Play()
        {
            bool isplaying = false;
            if (this.type == AudioType.Stream && this.channel != null)
            {
                this.channel.isPlaying(ref isplaying);
            }

            if (isplaying)
            {
                return;
            }

            FmodAudioManager.ERRCHECKCHANNEL(this.system.playSound(CHANNELINDEX.FREE, this.sound, true, ref this.channel));
            CHANNEL_CALLBACK callback = (CHANNEL_CALLBACK)Delegate.CreateDelegate(typeof(CHANNEL_CALLBACK), this, "ChannelCallback");
            
            this.callbackIndex = ((FmodAudioManager)AudioManager.Instance).AddCallback(callback);
            
            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setCallback(callback));
            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setVolume(this.volume));
           
            float frequency = 0.0f;
           
            FmodAudioManager.ERRCHECKCHANNEL(this.channel.getFrequency(ref frequency));
            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setFrequency(frequency * this.pitch));
           
            if (this.loopEnd > this.loopBegin)
            {
                FmodAudioManager.ERRCHECKCHANNEL(this.channel.setLoopPoints(this.loopBegin, TIMEUNIT.MS, this.loopEnd, TIMEUNIT.MS));
            }

            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setPaused(false));
        }

        private RESULT ChannelCallback(
          IntPtr channelraw,
          CHANNEL_CALLBACKTYPE type,
          IntPtr commanddata1,
          IntPtr commanddata2)
        {
            if (type == CHANNEL_CALLBACKTYPE.END)
            {
                if (this.channel.getRaw() == channelraw)
                {
                    this.channel = null;
                }

                this.HandleSoundCompletion();
            }
            return RESULT.OK;
        }

        public override void Pause()
        {
            if (this.channel == null)
            {
                return;
            }

            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setPaused(true));
        }

        public override void Resume()
        {
            if (this.channel == null)
            {
                return;
            }

            FmodAudioManager.ERRCHECKCHANNEL(this.channel.setPaused(false));
        }

        public override void Stop()
        {
            if (this.channel == null)
            {
                return;
            }

            bool isplaying = false;
            int num = (int)this.channel.isPlaying(ref isplaying);
            if (!isplaying)
            {
                return;
            }

            FmodAudioManager.ERRCHECKCHANNEL(this.channel.stop());
            this.channel = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                int num = disposing ? 1 : 0;
                FmodAudioManager.ERRCHECKSOUND(this.sound.release());
                ((FmodAudioManager)AudioManager.Instance).RemoveCallback(this.callbackIndex);
            }
            this.disposed = true;
        }
    }
}
