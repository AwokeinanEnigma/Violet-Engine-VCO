using SFML.Graphics;
using System;

namespace Violet.Graphics
{
    /// <summary>
    /// An interface for a texture within the Violet Engine
    /// </summary>
    public interface IVioletTexture : IDisposable
    {
        /// <summary>
		/// The texture
		/// </summary>
		Texture Image { get; }
    }
}
