using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    static class ESTProgram
    {
        private static ENUGameState currentState = ENUGameState.Menu;
        public static float deltaTime;
        static DateTime lastFrameTime = DateTime.Now;
        public static bool showDebug = false; // F1 lo activa

        public static int SCREEN_WIDTH  = 1024;
        public static int SCREEN_HEIGHT = 544;

        // Entidades del juego
        public static HERPlayer           p1;
        public static List<HERAsteroid>   asteroids  = new List<HERAsteroid>();
        public static CLSPoolObject<HERBullet> bulletPool;
        public static List<HERExplosion>  explosions = new List<HERExplosion>();

        private static CLSScreenShake screenShake = new CLSScreenShake();

        // Oleadas: cada una trae mas asteroides grandes y mas rapidos. Al limpiar la ultima se gana.
        private const int MAX_WAVES = 3;
        private const float NEXT_WAVE_DELAY = 1.5f;
        private const float WAVE_BANNER_TIME = 2f;
        private static int currentWave;
        private static float nextWaveTimer;
        private static float waveBannerTimer;

        // Al perder una vida la nave reaparece despues de un momento
        private const float RESPAWN_DELAY = 1.2f;
        private static float respawnTimer;

        // Al ganar o perder se espera un poco antes de cambiar de pantalla para ver la explosion
        private const float END_DELAY = 1.2f;
        private static float endTimer;

        // Fondo de la partida (generado con tools/generate-ui-assets.ps1)
        private const string GAME_BG_PATH = "assets/textures/Scenes/GameBackground.png";
        private static STRTransform gameBackgroundTransform;

        private const string LIFE_ICON_PATH = "assets/textures/Animations/Idle/0.png";

        // Escenas
        private static INTScene mainMenuScene;
        private static INTScene playMenuScene;
        private static INTScene optionsScene;
        private static INTScene winScene;
        private static INTScene loseScene;
        private static IMPPauseScene pauseScene; // tipo concreto para poder llamar a Open()

        private static CLSAsteroidFactory asteroidFactory = new CLSAsteroidFactory();

        private static STRVector2f ScreenCenter => new STRVector2f(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

        // La nave esta en juego (no exploto ni esta esperando para reaparecer)
        private static bool PlayerActive => !CLSGameManager.Instance.IsGameOver && respawnTimer <= 0;

        [STAThread]
        static void Main()
        {
            ESTEngine.Initialize(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids GDI+");

            // Record, opciones y partida guardada desde %AppData%\AsteroidDestroyer\save.txt
            CLSGameManager.Instance.UseSaveStore(new IMPFileSaveStore());

            // La partida se crea al elegir New Game o Load Game, si no pisaria la partida guardada
            InitializeMenu();

            while (ESTEngine.IsWindowOpen)
            {
                ESTEngine.UpdateWindow();
                CalcDeltaTime();
                Update();
                Draw();
                ESTEngine.Render();
            }

            // Por si se cierra en medio de una partida con un record nuevo
            CLSGameManager.Instance.Save();
        }


        // Update segun el estado del juego

        private static void Update()
        {
            switch (currentState)
            {
                case ENUGameState.Menu:
                    mainMenuScene.Update(deltaTime);
                    break;

                case ENUGameState.PlayMenu:
                    playMenuScene.Update(deltaTime);
                    break;

                case ENUGameState.Options:
                    optionsScene.Update(deltaTime);
                    break;

                case ENUGameState.Playing:
                    if (ESTEngine.OnKeyDown(Keys.P) || ESTEngine.OnKeyDown(Keys.Escape))
                    {
                        pauseScene.Open();
                        currentState = ENUGameState.Paused;
                        break;
                    }

                    UpdateGame(deltaTime);
                    UpdateWaveProgress(deltaTime);
                    break;

                case ENUGameState.Paused:
                    if (ESTEngine.OnKeyDown(Keys.P) || ESTEngine.OnKeyDown(Keys.Escape))
                        currentState = ENUGameState.Playing;
                    else
                        pauseScene.Update(deltaTime);
                    break;

                case ENUGameState.Victory:
                    winScene.Update(deltaTime);
                    break;

                case ENUGameState.GameOver:
                    loseScene.Update(deltaTime);
                    break;
            }
        }


        // Draw segun el estado del juego

        private static void Draw()
        {
            switch (currentState)
            {
                case ENUGameState.Menu:
                    mainMenuScene.Draw();
                    break;

                case ENUGameState.PlayMenu:
                    playMenuScene.Draw();
                    break;

                case ENUGameState.Options:
                    optionsScene.Draw();
                    break;

                case ENUGameState.Playing:
                    DrawGame();
                    break;

                case ENUGameState.Paused:
                    DrawGame();
                    pauseScene.Draw();
                    break;

                case ENUGameState.Victory:
                    winScene.Draw();
                    break;

                case ENUGameState.GameOver:
                    loseScene.Draw();
                    break;
            }
        }


        // Inicializacion


        // Crea las escenas y les pasa lo que hacen sus botones
        public static void InitializeMenu()
        {
            var gm = CLSGameManager.Instance;

            mainMenuScene = new IMPMainMenuScene(
                onPlay:    () => currentState = ENUGameState.PlayMenu,
                onOptions: () => currentState = ENUGameState.Options,
                onQuit:    () => Application.Exit()
            );

            playMenuScene = new IMPPlayMenuScene(
                onNewGame:  () => RestartGame(),
                onLoadGame: () => LoadGame(),
                onBack:     () => currentState = ENUGameState.Menu
            );

            optionsScene = new IMPOptionsScene(
                isSoundEnabled: () => gm.SoundEnabled,
                onToggleSound:  () => gm.ToggleSound(),
                onBack:         () => currentState = ENUGameState.Menu
            );

            pauseScene = new IMPPauseScene(
                onResume:  () => currentState = ENUGameState.Playing,
                onRestart: () => RestartGame(),
                onMenu:    () => currentState = ENUGameState.Menu
            );

            STRVector2f bgSize = ESTEngine.GetTextureSize(GAME_BG_PATH);
            STRTransform bt = new STRTransform(0, 0);
            bt.Scale = new STRVector2f(SCREEN_WIDTH / bgSize.X, SCREEN_HEIGHT / bgSize.Y);
            gameBackgroundTransform = bt;

            winScene = new IMPWinScene(
                gameManager: gm,
                onContinue: () => RestartGame(),
                onBack:     () => currentState = ENUGameState.Menu
            );

            loseScene = new IMPLoseScene(
                gameManager: gm,
                onRetry: () => RestartGame(),
                onBack:  () => currentState = ENUGameState.Menu
            );
        }

        public static void InitializeGame(int startWave)
        {
            p1         = new HERPlayer(ScreenCenter);
            asteroids.Clear();
            explosions.Clear();
            bulletPool = new CLSPoolObject<HERBullet>(100);
            screenShake.Reset();
            endTimer      = END_DELAY;
            respawnTimer  = 0;

            StartWave(startWave);
        }

        private static void RestartGame()
        {
            CLSGameManager.Instance.ResetGame();
            InitializeGame(1);
            currentState = ENUGameState.Playing;
        }

        // Sigue desde el inicio de la ultima oleada guardada (si no hay, arranca una nueva)
        private static void LoadGame()
        {
            var gm = CLSGameManager.Instance;
            if (!gm.HasSavedGame)
            {
                RestartGame();
                return;
            }

            gm.LoadSavedGame();
            InitializeGame(gm.SavedWave);
            currentState = ENUGameState.Playing;
        }


        // Oleadas

        private static void StartWave(int wave)
        {
            currentWave     = wave;
            nextWaveTimer   = NEXT_WAVE_DELAY;
            waveBannerTimer = WAVE_BANNER_TIME;

            int count = 2 + wave;                     // 3, 4, 5 asteroides grandes
            float speed = 1f + 0.2f * (wave - 1);     // cada oleada un 20% mas rapida

            for (int i = 0; i < count; i++)
                AddAsteroid(asteroidFactory.CreateSafeAsteroid(p1.Transform.Position, SCREEN_WIDTH, SCREEN_HEIGHT,
                                                              ENUAsteroidSize.Large, speed));

            CLSGameManager.Instance.SaveCheckpoint(wave);
        }

        private static void UpdateWaveProgress(float dt)
        {
            var gm = CLSGameManager.Instance;

            if (gm.IsGameOver)
            {
                endTimer -= dt;
                if (endTimer <= 0)
                    currentState = ENUGameState.GameOver;
                return;
            }

            if (asteroids.Count > 0) return;

            if (currentWave < MAX_WAVES)
            {
                nextWaveTimer -= dt;
                if (nextWaveTimer <= 0)
                    StartWave(currentWave + 1);
            }
            else
            {
                endTimer -= dt;
                if (endTimer <= 0)
                {
                    gm.FinishGame();
                    currentState = ENUGameState.Victory;
                }
            }
        }

        private static void AddAsteroid(HERAsteroid asteroid)
        {
            asteroid.OnDestroyed += OnAsteroidExploded;
            asteroids.Add(asteroid);
        }


        // Logica del juego

        public static void UpdateGame(float dt)
        {
            if (ESTEngine.OnKeyDown(Keys.F1))
                showDebug = !showDebug;

            if (respawnTimer > 0)
            {
                respawnTimer -= dt;
                if (respawnTimer <= 0)
                    p1.Respawn(ScreenCenter);
            }

            if (PlayerActive)
            {
                if (ESTEngine.OnKeyDown(Keys.Space))
                {
                    bulletPool.GetObject().Init(p1.Transform.Position, p1.Transform.Angle, p1.Velocity);
                    PlaySound("assets/sounds/laser-zap-90575.wav");
                }

                p1.Update(dt);
            }

            foreach (var asteroid in asteroids)
                asteroid.Update(dt);

            UpdateBullets(dt);

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(dt);
                if (explosions[i].IsFinished)
                    explosions.RemoveAt(i);
            }

            screenShake.Update(dt);

            if (waveBannerTimer > 0)
                waveBannerTimer -= dt;

            CheckCollisions();

            if (showDebug)
            {
                ESTEngine.DebugLog($"FPS {Math.Round(1f / Math.Max(dt, 0.0001f))}");
                ESTEngine.DebugLog($"Wave {currentWave}  Asteroids {asteroids.Count}  Lives {CLSGameManager.Instance.Lives}");
                ESTEngine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}  angle {Math.Round(p1.Transform.Angle)}");
                ESTEngine.DebugLog($"Bullets active {bulletPool.ActiveObjects.Count}  pooled {bulletPool.AvailableObjects.Count}");
            }
        }

        // Las balas desactivadas vuelven al pool para que no sigan chocando asteroides
        private static void UpdateBullets(float dt)
        {
            var active = bulletPool.ActiveObjects;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                active[i].Update(dt);
                if (!active[i].Active)
                    bulletPool.RealeaseObject(active[i]);
            }
        }

        public static void DrawGame()
        {
            // El fondo queda quieto; el shake mueve solo lo que esta en el espacio
            ESTEngine.Draw(GAME_BG_PATH, gameBackgroundTransform);

            ESTCamera.Offset = screenShake.Offset;

            foreach (var asteroid in asteroids)
                asteroid.Draw();

            foreach (var bullet in bulletPool.ActiveObjects)
                bullet.Draw();

            if (PlayerActive)
                p1.Draw();

            foreach (var explosion in explosions)
                explosion.Draw();

            ESTCamera.Offset = new STRVector2f(0, 0);

            DrawHud();
        }

        // HUD: puntaje y vidas arriba a la derecha, oleada abajo a la izquierda y pausa abajo a la derecha
        private static void DrawHud()
        {
            var gm = CLSGameManager.Instance;
            ESTPixelText.Draw($"SCORE {gm.Score}", SCREEN_WIDTH - 20, 14, 3, Color.White, 1f);
            ESTPixelText.Draw($"BEST {gm.HighScore}", SCREEN_WIDTH - 20, 44, 2, ESTPixelText.Dim, 1f);
            DrawLives(gm.Lives);

            ESTPixelText.Draw($"WAVE {currentWave}/{MAX_WAVES}", 20, SCREEN_HEIGHT - 32, 2, Color.White);
            ESTPixelText.Draw("P  PAUSE", SCREEN_WIDTH - 20, SCREEN_HEIGHT - 32, 2, ESTPixelText.Dim, 1f);

            // Cartel "WAVE N" al empezar cada oleada (no se muestra en pausa)
            if (waveBannerTimer > 0 && currentState == ENUGameState.Playing)
            {
                string bannerPath = $"assets/textures/UI/Wave{currentWave}Title.png";
                STRVector2f size = ESTEngine.GetTextureSize(bannerPath);
                STRTransform bt = new STRTransform(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f - 90f);
                float s = 240f / size.X;
                bt.Scale  = new STRVector2f(s, s);
                bt.Origin = new STRVector2f(0.5f, 0.5f);
                ESTEngine.Draw(bannerPath, bt);
            }
        }

        // Una navecita por cada vida, alineadas a la derecha debajo del record
        private static void DrawLives(int lives)
        {
            STRVector2f iconSize = ESTEngine.GetTextureSize(LIFE_ICON_PATH);
            float scale = 22f / iconSize.Y;
            float iconWidth = iconSize.X * scale;

            for (int i = 0; i < lives; i++)
            {
                STRTransform t = new STRTransform(SCREEN_WIDTH - 20 - i * (iconWidth + 6), 68f);
                t.Scale  = new STRVector2f(scale, scale);
                t.Origin = new STRVector2f(1f, 0f);
                ESTEngine.Draw(LIFE_ICON_PATH, t);
            }
        }

        private static void CheckCollisions()
        {
            var activeBullets = bulletPool.ActiveObjects;

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = asteroids[i];

                for (int j = activeBullets.Count - 1; j >= 0; j--)
                {
                    var bullet = activeBullets[j];

                    if (ESTCollision.CheckAABB(asteroid.Transform, bullet.Transform))
                    {
                        // Se saca de la lista antes del evento porque los fragmentos se agregan al final
                        asteroids.RemoveAt(i);
                        asteroid.Destroy();
                        bullet.Deactivate();
                        bulletPool.RealeaseObject(bullet);

                        CLSGameManager.Instance.AddScore(asteroid.Points);
                        break;
                    }
                }
            }

            if (!PlayerActive || p1.IsInvulnerable) return;

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = asteroids[i];
                if (ESTCollision.CheckAABB(p1.Transform, asteroid.Transform))
                {
                    asteroids.RemoveAt(i);
                    asteroid.Destroy();
                    OnPlayerHit();
                    break;
                }
            }
        }

        private static void OnPlayerHit()
        {
            // La nave explota mas grande y sacude mas fuerte
            explosions.Add(new HERExplosion(p1.Transform.Position, p1.Velocity, 140f));
            screenShake.Shake(0.5f, 14f);

            bool livesLeft = CLSGameManager.Instance.LoseLife();
            if (livesLeft)
                respawnTimer = RESPAWN_DELAY;
        }

        private static void OnAsteroidExploded(HERAsteroid asteroid)
        {
            PlaySound("assets/sounds/explosion-42132.wav");

            // Explosion y sacudon proporcionales al tamaño del asteroide
            float diameter = HERAsteroid.GetDiameter(asteroid.Size);
            explosions.Add(new HERExplosion(asteroid.Transform.Position, asteroid.Velocity, diameter * 1.6f));
            screenShake.Shake(0.1f + diameter / 600f, diameter / 20f);

            // Se parte en dos asteroides mas chicos
            foreach (var fragment in asteroidFactory.CreateFragments(asteroid))
                AddAsteroid(fragment);
        }

        // Respeta la opcion de sonido del menu de opciones
        private static void PlaySound(string path)
        {
            if (CLSGameManager.Instance.SoundEnabled)
                ESTEngine.PlaySound(path);
        }

        static void CalcDeltaTime()
        {
            TimeSpan span = DateTime.Now - lastFrameTime;
            deltaTime    = (float)span.TotalSeconds;
            lastFrameTime = DateTime.Now;
        }
    }
}
