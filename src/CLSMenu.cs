using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    public class CLSMenu
    {
        private List<CLSText> menuItems = new List<CLSText>();
        private List<float> itemWidths = new List<float>();
        private int selectedIndex;
        private STRTransform transform;
        private Color normalColor = Color.White;
        private Color selectedColor = Color.Cyan;
        private int fontSize;
        private string fontPath;
        private float spacing;

        // Variables para la animación del selector
        private STRVector2f selectorPos;
        private float selectorWidth;

        public event Action<int> OnOptionSelected;

        public STRTransform Transform 
        { 
            get { return transform; } 
            set 
            { 
                transform = value; 
                UpdateItemsPositions();
            } 
        }

        public CLSMenu(List<string> options, float x, float y, int fontSize = 30, string fontPath = null)
        {
            this.transform = new STRTransform(x, y);
            this.fontSize = fontSize;
            this.fontPath = fontPath;
            this.selectedIndex = 0;
            this.spacing = fontSize * 1.8f;

            foreach (var opt in options)
            {
                CLSText item = new CLSText(opt, new STRVector2f(0, 0));
                item.SetFont(fontPath, fontSize);
                                
                STRTransform t = item.Transform;
                t.Origin = new STRVector2f(0.5f, 0.5f);
                item.Transform = t;

                menuItems.Add(item);
                itemWidths.Add(ESTEngine.GetTextWidth(opt, fontSize, fontPath));
            }

            UpdateItemsPositions();

            // Inicializar posición del selector
            if (menuItems.Count > 0)
            {
                selectorPos = menuItems[0].Transform.Position;
                selectorWidth = itemWidths[0];
            }
        }

        private void UpdateItemsPositions()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                STRTransform itemTransform = menuItems[i].Transform;
                itemTransform.Position = transform.Position + new STRVector2f(0, i * spacing);
                menuItems[i].Transform = itemTransform;
            }
        }

        public void Update(float deltaTime)
        {
            if (ESTEngine.OnKeyDown(Keys.Up) || ESTEngine.OnKeyDown(Keys.W))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = menuItems.Count - 1;
            }
            if (ESTEngine.OnKeyDown(Keys.Down) || ESTEngine.OnKeyDown(Keys.S))
            {
                selectedIndex++;
                if (selectedIndex >= menuItems.Count) selectedIndex = 0;
            }
            if (ESTEngine.OnKeyDown(Keys.Enter) || ESTEngine.OnKeyDown(Keys.Space))
            {
                OnOptionSelected?.Invoke(selectedIndex);
            }

            // Animación suave del selector usando Lerp
            if (menuItems.Count > 0)
            {
                STRVector2f targetPos = menuItems[selectedIndex].Transform.Position;
                float targetWidth = itemWidths[selectedIndex] + 20;

                float speed = 15f;
                selectorPos = STRVector2f.Lerp(selectorPos, targetPos, deltaTime * speed);
                selectorWidth = STRVector2f.Lerp(selectorWidth, targetWidth, deltaTime * speed);
            }
        }

        public void Draw()
        {
            // Dibuja el selector en su posicion actual
            ESTEngine.DrawRectangle(
                selectorPos, 
                selectorWidth, 
                fontSize + 10, 
                Color.FromArgb(80, selectedColor), 
                true, 
                0.5f, 0.5f
            );

            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelected = (i == selectedIndex);
                menuItems[i].Color = isSelected ? selectedColor : normalColor;
                
                // Dibuja el texto de cada opcion
                menuItems[i].Draw();
            }
        }
    }
}
