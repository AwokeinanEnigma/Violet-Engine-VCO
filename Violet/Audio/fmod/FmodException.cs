using System;

namespace Violet.Audio.fmod
{
    internal sealed class FmodException : Exception
    {
        public FmodException(string message) : base(message)
        {
        }
    }
}
