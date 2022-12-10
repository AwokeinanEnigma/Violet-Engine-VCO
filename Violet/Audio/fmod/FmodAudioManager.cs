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
            FmodAudioManager.ERRCHECK(Factory.System_Create(ref this.system));
            FmodAudioManager.ERRCHECK(this.system.getVersion(ref version));
            FmodAudioManager.ERRCHECK(this.system.getNumDrivers(ref numdrivers));
            RESULT result1;
            if (numdrivers == 0)
            {
                result1 = this.system.setOutput(OUTPUTTYPE.NOSOUND);
                FmodAudioManager.ERRCHECK(result1);
            }
            else
            {
                FmodAudioManager.ERRCHECK(this.system.getDriverCaps(0, ref caps, ref controlpaneloutputrate, ref controlpanelspeakermode));
                if ((caps & CAPS.HARDWARE_EMULATED) == CAPS.HARDWARE)
                {
                    RESULT result2 = this.system.setDSPBufferSize(1024U, 10);
                    Debug.LogWarning("Audio hardware acceleration is turned off. Audio performance may be degraded.");
                    FmodAudioManager.ERRCHECK(result2);
                }
                StringBuilder name = new StringBuilder(256);
                GUID guid = new GUID();
                result1 = this.system.getDriverInfo(0, name, 256, ref guid);
                FmodAudioManager.ERRCHECK(result1);
                string str = name.ToString();
                Debug.LogInfo($"Audio driver name: {str}");
                if (str.Contains("SigmaTel"))
                {
                    result1 = this.system.setSoftwareFormat(48000, SOUND_FORMAT.PCMFLOAT, 0, 0, DSP_RESAMPLER.LINEAR);
                    FmodAudioManager.ERRCHECK(result1);
                    Debug.LogDebug("Sigmatel card detected; format changed to PCM floating point.");
                }
            }
            this.InitFmodSystem();
            if (result1 == RESULT.ERR_OUTPUT_CREATEBUFFER)
            {
                FmodAudioManager.ERRCHECK(this.system.setSpeakerMode(SPEAKERMODE.STEREO));
                this.InitFmodSystem();
                Debug.LogWarning("Selected speaker mode is not supported, defaulting to stereo.");
            }
            this.callbacks = new Dictionary<int, CHANNEL_CALLBACK>();
        }

        private unsafe void InitFmodSystem() => FmodAudioManager.ERRCHECK(this.system.init(32, INITFLAGS.NORMAL, (IntPtr)(void*)null));

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
            FmodAudioManager.ERRCHECK(this.system.setSpeakerMode(speakermode));
        }

        public override void Update()
        {
            base.Update();
            int num = (int)this.system.update();
        }

        public override VioletSound Use(string filename, AudioType type, [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            int hashCode = filename.GetHashCode();
            FmodSound fmodSound;
            //Console.WriteLine(string.Format("AUDIO - {0} - LINE: {1} - METHOD - {2}", filename, i, member));
            if (!this.sounds.ContainsKey(hashCode))
            {
                fmodSound = FmodAudioLoader.Instance.LoadSound(ref this.system, filename, 0, this.effectsVolume);
                this.instances.Add(hashCode, 1);
                this.sounds.Add(hashCode, fmodSound);
            }
            else
            {
                fmodSound = (FmodSound)this.sounds[hashCode];
                Dictionary<int, int> instances;
                int key;
                (instances = this.instances)[key = hashCode] = instances[key] + 1;
            }
            return fmodSound;
        }

        public override void Unuse(VioletSound sound)
        {
            int key1 = 0;
            VioletSound VioletSound = null;
            foreach (KeyValuePair<int, VioletSound> sound1 in this.sounds)
            {
                key1 = sound1.Key;
                VioletSound = sound1.Value;
                if (VioletSound == sound)
                {
                    Dictionary<int, int> instances;
                    int key2;
                    (instances = this.instances)[key2 = key1] = instances[key2] - 1;
                    break;
                }
            }
            if (VioletSound == null || this.instances[key1] > 0)
            {
                return;
            }

            //Console.WriteLine("Cleaning up audio");
            this.instances.Remove(key1);
            this.sounds.Remove(key1);
            VioletSound.Dispose();
        }

        public int AddCallback(CHANNEL_CALLBACK callback)
        {
            this.callbacks.Add(++this.callbackCounter, callback);
            return this.callbackCounter;
        }

        public void RemoveCallback(int index) => this.callbacks.Remove(index);

        public static void ERRCHECK(RESULT result)
        {

            if (result != RESULT.OK)
            {
                Console.WriteLine(($"There was an error trying to play a sound! "));
            }
            //throw new FmodException(string.Format("FMOD error: {0} - {1}", (object)result, (object)Error.String(result)));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.disposed || this.system == null)
            {
                return;
            }

            FmodAudioManager.ERRCHECK(this.system.close());
            FmodAudioManager.ERRCHECK(this.system.release());
        }
    }
}
