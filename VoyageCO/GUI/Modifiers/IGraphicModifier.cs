using Violet.Graphics;

namespace VCO.GUI.Modifiers
{
    internal interface IGraphicModifier
    {
        bool Done { get; }

        Graphic Graphic { get; }

        void Update();
    }
}
