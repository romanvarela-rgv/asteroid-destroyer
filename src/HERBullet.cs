using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class HERBullet : ABSCharacter
    {
        public Action<HERBullet> OnDeactivate; // avisa al pool cuando la bala se desactiva
                                           
        private float lifeTime = 2.0f;
        private CLSAnimation animation;
        private string lastFrame = "";

        public float LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }

        public bool Active { get; private set; }

        // Constructor
        public HERBullet() : base(new STRVector2f(0,0), 1.0f)
        {
            this.Active = false; // las balas empiezan inactivas
            // Animacion de la bala
            animation = new CLSAnimation(
                "bullet",
                15f,
                new List<string>
                {
                    "assets/textures/Animations/1.png",
                    "assets/textures/Animations/2.png",
                    "assets/textures/Animations/3.png",
                    "assets/textures/Animations/4.png"
                },
                true
            );

        }

        public HERBullet (STRVector2f position, float angle, STRVector2f shipVelocity) : base(position, 1.0f)
        {
            this.Active = true;

            animation = new CLSAnimation(
                "bullet",
                15f,
                new List<string>
                {
                    "assets/textures/Animations/1.png",
                    "assets/textures/Animations/2.png",
                    "assets/textures/Animations/3.png",
                    "assets/textures/Animations/4.png"
                },
                true
            );
            Init(position, angle, shipVelocity);
        }

        public void Init(STRVector2f position, float angle, STRVector2f shipVelocity)
        {
            this.lifeTime = 2.0f;
            this.Active = true;

            animation.Reset();
            SetSprite(animation.CurrentFrame);

            STRTransform temp = Transform;
            temp.Position = position;
            temp.Origin = new STRVector2f(0.5f, 0.5f); 
            Transform = temp;

            float speed = 400.0f;
            float angleRad = (float)(angle * Math.PI / 180.0f);

            this.Velocity = new STRVector2f(
                (float)Math.Cos(angleRad),
                (float)Math.Sin(angleRad)
            ) * speed + shipVelocity;
        }

        // Update
        public override void Update(float deltaTime)
        {
            if (!Active) return;

            STRTransform t = Transform;
            t.Position += Velocity * deltaTime;
            Transform = t;

            animation.Update(deltaTime);
            string currentFrame = animation.CurrentFrame;

            if (currentFrame != lastFrame)
            {
                SetSprite(currentFrame);
                lastFrame = currentFrame;
            }

            // Lifetime
            lifeTime -= deltaTime;

            if (lifeTime <= 0 || IsOffScreen())
            {
                Deactivate();
            }
        }

        // Draw
        public override void Draw()
        {
            if (!Active) return;

            ESTCamera.Draw(sprite, Transform);
        }

        // Desactiva la bala y la devuelve al pool
        public void Deactivate()
        {
            if (!Active) return;

            Active = false;
            OnDeactivate?.Invoke(this);
        }

        // Se fija si la bala salio de la pantalla
        private bool IsOffScreen()
        {
            float margin = 50f;

            return Transform.Position.X < -margin ||
                   Transform.Position.X > ESTProgram.SCREEN_WIDTH + margin ||
                   Transform.Position.Y < -margin ||
                   Transform.Position.Y > ESTProgram.SCREEN_HEIGHT + margin;
        }
    }
}