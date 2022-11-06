using System;

namespace Violet.Audio.Stub
{
    internal class StubAudioManager : AudioManager
    {
        public StubAudioManager()
        {
            Console.WriteLine("STUBBED AUDIO MANAGER");
        }

        public override void SetSpeakerMode(AudioMode mode)
        {
        }

        public override void Update()
        {
        }

        public override VioletSound Use(string filename, AudioType type)
        {
            return new StubSound(type, 0U, 0U, 0, 0f, 0f);
        }

        public override void Unuse(VioletSound sound)
        {
        }
    }
}
