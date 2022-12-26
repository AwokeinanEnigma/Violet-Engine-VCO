using FMOD;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Violet.Audio.fmod
{
    internal class FmodAudioManager : AudioManager
    {
        private FMOD.System system;
        private Dictionary<int, CHANNEL_CALLBACK> callbacks;
        private int callbackCounter;

        public FmodAudioManager()
        {
            uint version = 0;
            int controlpaneloutputrate = 0;
            int numdrivers = 0;
            SPEAKERMODE controlpanelspeakermode = SPEAKERMODE.STEREO;
            CAPS caps = CAPS.NONE;
            
            FmodAudioManager.ERRCHECKSYSTEM(Factory.System_Create(ref this.system));
            FmodAudioManager.ERRCHECKSYSTEM(this.system.getVersion(ref version));
            FmodAudioManager.ERRCHECKSYSTEM(this.system.getNumDrivers(ref numdrivers));

            RESULT baseResult;
            if (numdrivers == 0)
            {
                baseResult = this.system.setOutput(OUTPUTTYPE.NOSOUND);
                FmodAudioManager.ERRCHECKMISC(baseResult);
            }
            else
            {
                FmodAudioManager.ERRCHECKSYSTEM(this.system.getDriverCaps(0, ref caps, ref controlpaneloutputrate, ref controlpanelspeakermode));
                if ((caps & CAPS.HARDWARE_EMULATED) == CAPS.HARDWARE)
                {
                    RESULT result2 = this.system.setDSPBufferSize(1024U, 10);
                    Debug.LogWarning("Audio hardware acceleration is turned off. Audio performance may be degraded.");
                    FmodAudioManager.ERRCHECKMISC(result2);
                }
                
                StringBuilder name = new StringBuilder(256);
                
                GUID guid = new GUID();
                baseResult = this.system.getDriverInfo(0, name, 256, ref guid);
                
                FmodAudioManager.ERRCHECKMISC(baseResult);
                string audioDriverName = name.ToString();
                
                Debug.LogInfo($"Audio driver name: {audioDriverName}");

                // old ass audio driver from the 2000s

                if (audioDriverName.Contains("SigmaTel"))
                {
                    baseResult = this.system.setSoftwareFormat(48000, SOUND_FORMAT.PCMFLOAT, 0, 0, DSP_RESAMPLER.LINEAR);
                    FmodAudioManager.ERRCHECKMISC(baseResult);

                    Debug.LogDebug("Sigmatel card detected; format changed to PCM floating point.");
                }
            
            }


            this.InitFmodSystem();
            if (baseResult == RESULT.ERR_OUTPUT_CREATEBUFFER)
            {
                FmodAudioManager.ERRCHECKSYSTEM(this.system.setSpeakerMode(SPEAKERMODE.STEREO));
                this.InitFmodSystem();

                Debug.LogWarning("Selected speaker mode is not supported, defaulting to stereo.");
            }
            this.callbacks = new Dictionary<int, CHANNEL_CALLBACK>();
        }

        private unsafe void InitFmodSystem() => FmodAudioManager.ERRCHECKSYSTEM(this.system.init(32, INITFLAGS.NORMAL, (IntPtr)(void*)null));

        public override void SetSpeakerMode(AudioMode mode)
        {
            SPEAKERMODE speakermode;
            switch (mode)
            {
                case AudioMode.Mono:
                    speakermode = SPEAKERMODE.MONO;
                    break;
                default:
                    speakermode = SPEAKERMODE.STEREO;
                    break;
            }
            FmodAudioManager.ERRCHECKSYSTEM(this.system.setSpeakerMode(speakermode));
        }

        public override void Update()
        {
            base.Update();
            int num = (int)this.system.update();
        }

        public override VioletSound Use(string filename, AudioType type, 
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            int hashCode = filename.GetHashCode();
            
            FmodSound fmodSound;

            if (!this.sounds.ContainsKey(hashCode))
            {
                fmodSound = FmodAudioLoader.Instance.LoadSound(ref this.system, filename, 0, this.effectsVolume, type);
                this.instances.Add(hashCode, 1);
                this.sounds.Add(hashCode, fmodSound);
            }
            else
            {
                fmodSound = (FmodSound)this.sounds[hashCode];

                Dictionary<int, int> instances = this.instances;
                instances[hashCode] = instances[hashCode] + 1;
                //Debug.Log($"{instances[hashCode]}");
            }
            return fmodSound;
        }

        public override void Unuse(VioletSound sound)
        {
            int key = 0;

            VioletSound memorySound = null;

            foreach (KeyValuePair<int, VioletSound> currentSound in this.sounds)
            {
                key = currentSound.Key;
                memorySound = currentSound.Value;
                if (memorySound == sound)
                {
                    Dictionary<int, int> instances;
                    int key2;
                    (instances = this.instances)[key2 = key] = instances[key2] - 1; 
                    
                    break;
                }
            }
            
            if (memorySound == null || this.instances[key] > 0)
            {
                return;
            }

            //Console.WriteLine("Cleaning up audio");
            this.instances.Remove(key);
            this.sounds.Remove(key);
            memorySound.Dispose();
        }

        public int AddCallback(CHANNEL_CALLBACK callback)
        {
            this.callbacks.Add(++this.callbackCounter, callback);
            return this.callbackCounter;
        }

        public void RemoveCallback(int index) => this.callbacks.Remove(index);

        public static void ERRCHECKMISC(RESULT miscResult,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (miscResult != RESULT.OK)
            {
                throw new FmodException($"FMOD Channel Error at {callerFilePath}.{callerLineNumber}: {miscResult} - {Error.String(miscResult)}");
            }
        }

        public static void ERRCHECKCHANNEL(RESULT channelResult,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0) {
            if (channelResult != RESULT.OK)
            {
                throw new FmodException($"FMOD Channel Error at {callerFilePath}.{callerLineNumber}: {channelResult} - {Error.String(channelResult)}");
            }
        }

        public static void ERRCHECKSYSTEM(RESULT systemResult,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (systemResult != RESULT.OK)
            {
                throw new FmodException($"FMOD System Error at {callerFilePath}.{callerLineNumber}: {systemResult} - {Error.String(systemResult)}");
            }
        }

        public static void ERRCHECKSOUND(RESULT soundResult,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (soundResult != RESULT.OK)
            {
                throw new FmodException($"FMOD Sound Error at {callerFilePath}.{callerLineNumber}: {soundResult} - {Error.String(soundResult)}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.disposed || this.system == null)
            {
                return;
            }

            FmodAudioManager.ERRCHECKSYSTEM(this.system.close());
            FmodAudioManager.ERRCHECKSYSTEM(this.system.release());
        }
    }
}
