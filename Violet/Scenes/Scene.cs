using System;

namespace Violet.Scenes
{
    public abstract class Scene : IDisposable
    {
        #region Properties
        public bool DrawBehind
        {
            get
            {
                return this.drawBehind;
            }
            set
            {
                this.drawBehind = value;
            }
        }
        #endregion

        #region Booleans
        protected bool disposed;
        private bool drawBehind;
        #endregion

        ~Scene()
        {
            this.Dispose(false);
        }

        #region Methods

        /// <summary>
        /// Called when the scene is first loaded.
        /// </summary>
        public virtual void Focus()
        {
        }

        /// <summary>
        /// Called when the scene manager is composite mode and this is the scene that's being drawed over. Don't use this as the place to dispose of resources!
        /// </summary>
        public virtual void Unfocus()
        {
        }

        /// <summary>
        /// Called when the scene is being unloaded and the game is transitioning to another scene.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// This is where you can draw graphics. 
        /// </summary>
        public virtual void Draw()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
            }
            this.disposed = true;
        }
        #endregion
    }
}
