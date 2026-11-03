using System;

namespace EngineGDI
{
    public class IMPMainMenuScene : INTScene
    {
        private const string BG_PATH = "assets/textures/Scenes/MainMenu.png";

       

        private STRTransform backgroundTransform;
        private CLSButtonGroup buttons = new CLSButtonGroup();

        public IMPMainMenuScene(Action onPlay, Action onOptions, Action onQuit)
        {
            STRVector2f bgSize = ESTEngine.GetTextureSize(BG_PATH);
            STRTransform bt = new STRTransform(0, 0);
            bt.Scale = new STRVector2f(ESTProgram.SCREEN_WIDTH / bgSize.X, ESTProgram.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx      = ESTProgram.SCREEN_WIDTH / 2f;   // 512
            float startY  = 290f;
            float spacing = 60f;

            var btnPlay    = new IMPImageButton("assets/textures/Buttons/PlayButton.png",    new STRVector2f(cx, startY),              130f);
            var btnOptions = new IMPImageButton("assets/textures/Buttons/OptionsButton.png", new STRVector2f(cx, startY + spacing),    130f);
            var btnQuit    = new IMPImageButton("assets/textures/Buttons/QuitButton.png",    new STRVector2f(cx, startY + spacing * 2), 130f);

            btnPlay.OnPressed    += onPlay;
            btnOptions.OnPressed += onOptions;
            btnQuit.OnPressed    += onQuit;

            // Agrega los botones a la lista

            buttons.Add(btnPlay);
            buttons.Add(btnOptions);
            buttons.Add(btnQuit);
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
