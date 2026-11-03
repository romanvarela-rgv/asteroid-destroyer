using System;
using System.Drawing;

namespace EngineGDI
{
    // Opciones: prender/apagar el sonido y ver los controles
    public class IMPOptionsScene : INTScene
    {
        private const string BG_PATH    = "assets/textures/Scenes/GameBackground.png";
        private const string TITLE_PATH = "assets/textures/UI/OptionsTitle.png";
        private const string SOUND_ON   = "assets/textures/Buttons/SoundOnButton.png";
        private const string SOUND_OFF  = "assets/textures/Buttons/SoundOffButton.png";

        private static readonly string[,] controls =
        {
            { "WASD / Arrows",    "Move" },
            { "Space",            "Shoot" },
            { "P / Esc",          "Pause" },
            { "Enter",            "Select" },
            { "F1",               "Debug info" }
        };

        private STRTransform backgroundTransform;
        private STRTransform titleTransform;
        private CLSButtonGroup buttons = new CLSButtonGroup();
        private IMPImageButton btnSound;
        private Func<bool> isSoundEnabled;

        public IMPOptionsScene(Func<bool> isSoundEnabled, Action onToggleSound, Action onBack)
        {
            this.isSoundEnabled = isSoundEnabled;

            STRVector2f bgSize = ESTEngine.GetTextureSize(BG_PATH);
            STRTransform bt = new STRTransform(0, 0);
            bt.Scale = new STRVector2f(ESTProgram.SCREEN_WIDTH / bgSize.X, ESTProgram.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx = ESTProgram.SCREEN_WIDTH / 2f;

            STRVector2f titleSize = ESTEngine.GetTextureSize(TITLE_PATH);
            STRTransform tt = new STRTransform(cx, 90f);
            float titleScale = 340f / titleSize.X;
            tt.Scale  = new STRVector2f(titleScale, titleScale);
            tt.Origin = new STRVector2f(0.5f, 0.5f);
            titleTransform = tt;

            btnSound = new IMPImageButton(isSoundEnabled() ? SOUND_ON : SOUND_OFF, new STRVector2f(cx, 190f), 150f);
            var btnBack = new IMPImageButton("assets/textures/Buttons/BackButton.png", new STRVector2f(930f, 490f), 90f);

            btnSound.OnPressed += onToggleSound;
            btnSound.OnPressed += RefreshSoundButton; // despues de cambiar la opcion, actualiza la imagen
            btnBack.OnPressed  += onBack;

            buttons.Add(btnSound);
            buttons.Add(btnBack);
        }

        private void RefreshSoundButton()
        {
            btnSound.SetSprite(isSoundEnabled() ? SOUND_ON : SOUND_OFF);
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            ESTEngine.Draw(BG_PATH, backgroundTransform);
            ESTEngine.Draw(TITLE_PATH, titleTransform);

            // Panel con el marco de los botones
            ESTPixelPanel.Draw(296, 244, 432, 212);
            ESTPixelText.Draw("CONTROLS", ESTProgram.SCREEN_WIDTH / 2f, 264, 3, ESTPixelText.Orange, 0.5f);
            for (int i = 0; i < controls.GetLength(0); i++)
            {
                float y = 304 + i * 28;
                ESTPixelText.Draw(controls[i, 0], 495, y, 2, Color.White, 1f);
                ESTPixelText.Draw(controls[i, 1], 530, y, 2, ESTPixelText.Dim);
            }

            buttons.Draw();
        }
    }
}
