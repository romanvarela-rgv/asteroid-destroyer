using System;

namespace EngineGDI
{
    public class IMPPlayMenuScene : INTScene
    {
        private const string BG_PATH = "assets/textures/Scenes/PlayMenu.png";


        private STRTransform backgroundTransform;
        private CLSButtonGroup buttons = new CLSButtonGroup();

        public IMPPlayMenuScene(Action onNewGame, Action onLoadGame, Action onBack)
        {
            STRVector2f bgSize = ESTEngine.GetTextureSize(BG_PATH);
            STRTransform bt = new STRTransform(0, 0);
            bt.Scale = new STRVector2f(ESTProgram.SCREEN_WIDTH / bgSize.X, ESTProgram.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx      = ESTProgram.SCREEN_WIDTH / 2f;   // 512
            float startY  = 290f;
            float spacing = 60f;

            var btnNew  = new IMPImageButton("assets/textures/Buttons/NewGameButton.png",  new STRVector2f(cx, startY),           130f);
            var btnLoad = new IMPImageButton("assets/textures/Buttons/LoadGameButton.png", new STRVector2f(cx, startY + spacing), 130f);
            var btnBack = new IMPImageButton("assets/textures/Buttons/BackButton.png",     new STRVector2f(930f, 490f),            90f);

            btnNew.OnPressed  += onNewGame;
            btnLoad.OnPressed += onLoadGame;
            btnBack.OnPressed += onBack;

            buttons.Add(btnNew);
            buttons.Add(btnLoad);
            buttons.Add(btnBack);
        }

        // Seleccion y update de los botones
        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            ESTEngine.Draw(BG_PATH, backgroundTransform);
            buttons.Draw();
        }
    }
}
