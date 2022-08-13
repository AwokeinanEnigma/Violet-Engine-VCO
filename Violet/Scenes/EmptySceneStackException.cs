using System;

namespace Violet.Scenes
{
    internal class EmptySceneStackException : Exception
    {
        public EmptySceneStackException() : base("The scene stack is empty.")
        {
        }
    }
}
