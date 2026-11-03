using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    // Lista de botones navegable con flechas/WASD y Enter/Espacio (la usan todas las escenas)
    public class CLSButtonGroup
    {
        private List<INTButton> buttons = new List<INTButton>();
        private int selectedIndex = 0;

        public void Add(INTButton button)
        {
            buttons.Add(button);
            button.IsSelected = buttons.Count - 1 == selectedIndex;
        }

        // Vuelve a seleccionar el primer boton
        public void ResetSelection()
        {
            Select(0);
        }

        public void Update()
        {
            if (buttons.Count == 0) return;

            if (ESTEngine.OnKeyDown(Keys.Up) || ESTEngine.OnKeyDown(Keys.W))
                Select((selectedIndex - 1 + buttons.Count) % buttons.Count);

            if (ESTEngine.OnKeyDown(Keys.Down) || ESTEngine.OnKeyDown(Keys.S))
                Select((selectedIndex + 1) % buttons.Count);

            if (ESTEngine.OnKeyDown(Keys.Enter) || ESTEngine.OnKeyDown(Keys.Space))
                buttons[selectedIndex].Press();
        }

        public void Draw()
        {
            foreach (var btn in buttons)
                btn.Draw();
        }

        private void Select(int index)
        {
            if (buttons.Count == 0) return;

            buttons[selectedIndex].IsSelected = false;
            selectedIndex = index;
            buttons[selectedIndex].IsSelected = true;
        }
    }
}
