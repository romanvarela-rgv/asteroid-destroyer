using System;
using System.Drawing;

namespace EngineGDI
{
    // Menu de pausa: se dibuja encima del juego congelado
    public class IMPPauseScene : INTScene
    {
        private const string TITLE_PATH = "assets/textures/UI/PausedTitle.png";

        private STRTransform titleTransform;
        private CLSButtonGroup buttons = new CLSButtonGroup();

        public IMPPauseScene(Action onResume, Action onRestart, Action onMenu)
        {
            float cx = ESTProgram.SCREEN_WIDTH / 2f;

            STRVector2f titleSize = ESTEngine.GetTextureSize(TITLE_PATH);
            STRTransform tt = new STRTransform(cx, 150f);
            float titleScale = 300f / titleSize.X;
            tt.Scale  = new STRVector2f(titleScale, titleScale);
            tt.Origin = new STRVector2f(0.5f, 0.5f);
            titleTransform = tt;

            float startY  = 270f;
            float spacing = 65f;

            var btnResume  = new IMPImageButton("assets/textures/Buttons/ResumeButton.png",  new STRVector2f(cx, startY),               130f);
            var btnRestart = new IMPImageButton("assets/textures/Buttons/RestartButton.png", new STRVector2f(cx, startY + spacing),     130f);
            var btnMenu    = new IMPImageButton("assets/textures/Buttons/MenuButton.png",    new STRVector2f(cx, startY + spacing * 2), 130f);

            btnResume.OnPressed  += onResume;
            btnRestart.OnPressed += onRestart;
            btnMenu.OnPressed    += onMenu;

            buttons.Add(btnResume);
            buttons.Add(btnRestart);
            buttons.Add(btnMenu);
        }

        // Al abrir la pausa siempre queda marcado "Resume"
        public void Open()
        {
            buttons.ResetSelection();
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            // Oscurece el juego que queda de fondo
            ESTEngine.DrawRectangle(0, 0, ESTProgram.SCREEN_WIDTH, ESTProgram.SCREEN_HEIGHT, Color.FromArgb(170, 0, 0, 0));

            ESTEngine.Draw(TITLE_PATH, titleTransform);
            buttons.Draw();

            ESTPixelText.Draw("P / Esc to resume", ESTProgram.SCREEN_WIDTH / 2f, 468f, 2, ESTPixelText.Dim, 0.5f);
        }
    }
}
