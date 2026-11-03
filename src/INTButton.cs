using System;

namespace EngineGDI
{
    public interface INTButton
    {
        STRTransform Transform { get; set; }
        bool IsSelected { get; set; }
        event Action OnPressed;
        void Press();
        void Draw();
    }
}
