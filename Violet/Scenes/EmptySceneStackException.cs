using System;

namespace Violet.Scenes
{
    /// <summary>
    /// Generic exception used if the scene stack is empty.
    /// </summary>
    internal class EmptySceneStackException : Exception
    {
        public EmptySceneStackException() : base("The scene stack is empty!")
        {
        }
    }
}
