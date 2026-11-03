using System;
using System.Drawing;

namespace EngineGDI
{
    public class CLSText
    {
        private string content;
        private STRTransform transform;
        private Color color;
        private int fontSize;
        private string fontPath;

        public string Content { get => content; set => content = value; }
        public STRTransform Transform { get => transform; set => transform = value; }
        public Color Color { get => color; set => color = value; }
        public int FontSize { get => fontSize; set => fontSize = value; }
        public string FontPath { get => fontPath; set => fontPath = value; }

        public CLSText(string content, STRVector2f position, int fontSize = 24, string fontPath = null)
        {
            this.content = content;
            this.transform = new STRTransform(position);
            this.color = Color.White;
            this.fontSize = fontSize;
            this.fontPath = fontPath;
        }

        public void SetFont(string path = null, int size = 24)
        {
            this.fontPath = path;
            this.fontSize = size;
        }

        public void Draw()
        {
            // Se dibuja con la posicion y la escala del transform
            ESTEngine.DrawText(content, transform, color, fontSize, fontPath);
        }
    }
}
