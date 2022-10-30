namespace Violet.Scenes.Transitions
{
    /// <summary>
    /// Generic interface providing fields and methods for transitions.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Is this transition complete?
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// What's the progress of the transition?
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Can we show the new scene?
        /// </summary>
        bool ShowNewScene { get; }

        /// <summary>
        /// Are we stopping the scenemanger from starting the new scene?
        /// </summary>
        bool Blocking { get; set; }

        /// <summary>
        /// Called every frame
        /// </summary>
        void Update();

        /// <summary>
        /// Draw stuff in this function
        /// </summary>
        void Draw();

        /// <summary>
        /// Transitions are NOT initiated every time they're needed. Instead, they're restarted
        /// Do restart stuff here.
        /// </summary>
        void Reset();

    }
}
