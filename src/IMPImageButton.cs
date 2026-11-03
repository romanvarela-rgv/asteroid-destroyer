using System;

namespace EngineGDI
{
    public class IMPImageButton : INTButton
    {
        private const string SELECTOR_PATH = "assets/textures/UI/Selector.png";

        private STRTransform transform;
        private bool isSelected;
        private string spritePath;

        public STRTransform Transform { get => transform; set => transform = value; }
        public bool IsSelected   { get => isSelected; set => isSelected = value; }

        public event Action OnPressed;

        // targetWidth: ancho deseado en pixels de pantalla. 0 = tamaño nativo.
        public IMPImageButton(string spritePath, STRVector2f position, float targetWidth = 0f)
        {
            this.spritePath = spritePath;

            STRVector2f size = ESTEngine.GetTextureSize(spritePath);
            STRTransform t = new STRTransform(position);
            t.Size   = size;
            t.Origin = new STRVector2f(0.5f, 0.5f);

            if (targetWidth > 0f)
            {
                float s = targetWidth / size.X;
                t.Scale = new STRVector2f(s, s);
            }

            transform = t;
        }

        // Cambia la imagen manteniendo la posicion y el ancho en pantalla (ej. Sound: On / Off)
        public void SetSprite(string path)
        {
            float width = transform.Size.X * transform.Scale.X;
            STRVector2f size = ESTEngine.GetTextureSize(path);

            spritePath = path;
            transform.Size = size;
            float s = width / size.X;
            transform.Scale = new STRVector2f(s, s);
        }

        public void Press()
        {
            OnPressed?.Invoke();
        }

        public void Draw()
        {
            ESTEngine.Draw(spritePath, transform);

            if (isSelected)
                DrawSelector();
        }

        // Flecha naranja a la izquierda del boton, con un pequeño vaiven
        private void DrawSelector()
        {
            // Tamaño del boton en pantalla (Size es el de la imagen sin escalar)
            float width  = transform.Size.X * transform.Scale.X;
            float height = transform.Size.Y * transform.Scale.Y;

            STRVector2f selectorSize = ESTEngine.GetTextureSize(SELECTOR_PATH);
            float scale = (height * 0.5f) / selectorSize.Y;

            float bob = (float)Math.Sin(Environment.TickCount / 120.0) * 3f;

            STRTransform st = new STRTransform(transform.Position.X - width / 2f - 2f + bob, transform.Position.Y);
            st.Scale  = new STRVector2f(scale, scale);
            st.Origin = new STRVector2f(1f, 0.5f);
            ESTEngine.Draw(SELECTOR_PATH, st);
        }
    }
}
