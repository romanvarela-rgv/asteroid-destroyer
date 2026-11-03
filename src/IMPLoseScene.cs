using System;
using System.Drawing;

namespace EngineGDI
{
    public class IMPLoseScene : INTScene
    {
        private const string BG_PATH = "assets/textures/Scenes/LoseScene.png";
        private const string NEW_RECORD_PATH = "assets/textures/UI/NewRecordTitle.png";

        

        private STRTransform backgroundTransform;
        private CLSButtonGroup buttons = new CLSButtonGroup();
        private CLSGameManager gameManager;

        public IMPLoseScene(CLSGameManager gameManager, Action onRetry, Action onBack)
        {
            this.gameManager = gameManager;

            STRVector2f bgSize = ESTEngine.GetTextureSize(BG_PATH);
            STRTransform bt = new STRTransform(0, 0);
            bt.Scale = new STRVector2f(ESTProgram.SCREEN_WIDTH / bgSize.X, ESTProgram.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            var btnRetry = new IMPImageButton("assets/textures/Buttons/RetryButton.png", new STRVector2f(512f, 330f), 130f);
            var btnBack  = new IMPImageButton("assets/textures/Buttons/BackButton.png",  new STRVector2f(930f, 490f),  90f);

            btnRetry.OnPressed += onRetry;
            btnBack.OnPressed  += onBack;

            buttons.Add(btnRetry);
            buttons.Add(btnBack);
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            ESTEngine.Draw(BG_PATH, backgroundTransform);
            DrawFinalScore();

            buttons.Draw();
        }

        // Puntaje centrado debajo del boton principal
        private void DrawFinalScore()
        {
            float cx = ESTProgram.SCREEN_WIDTH / 2f;
            ESTPixelText.Draw($"SCORE {gameManager.Score}", cx, 388, 4, Color.White, 0.5f);
            ESTPixelText.Draw($"BEST {gameManager.HighScore}", cx, 432, 2, ESTPixelText.Dim, 0.5f);

            if (gameManager.IsNewRecord)
            {
                STRVector2f size = ESTEngine.GetTextureSize(NEW_RECORD_PATH);
                STRTransform t = new STRTransform(cx, 485);
                float s = 220f / size.X;
                t.Scale  = new STRVector2f(s, s);
                t.Origin = new STRVector2f(0.5f, 0.5f);
                ESTEngine.Draw(NEW_RECORD_PATH, t);
            }
        }
    }
}
