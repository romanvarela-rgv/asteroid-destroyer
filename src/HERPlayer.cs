using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    public class HERPlayer : ABSCharacter
    {
        // El sprite de la nave mira hacia arriba, pero en el juego angulo 0 = derecha
        private const float SPRITE_ANGLE_OFFSET = 90f;

        private const float INVULNERABLE_TIME = 2.5f;

        private CLSAnimation animation;
        private string lastFrame = "";
        private float invulnerableTimer;

        // Despues de reaparecer no lo pueden dañar por unos segundos
        public bool IsInvulnerable => invulnerableTimer > 0;

        public HERPlayer(STRVector2f position) : base(position, 0.55f)
        {
            // Las turbinas titilan entre los 4 frames
            animation = new CLSAnimation(
                "idle",
                8f,
                new List<string>
                {
                    "assets/textures/Animations/Idle/0.png",
                    "assets/textures/Animations/Idle/1.png",
                    "assets/textures/Animations/Idle/2.png",
                    "assets/textures/Animations/Idle/3.png"
                },
                true
            );

            SetSprite(animation.CurrentFrame);

            STRTransform temp = Transform;
            temp.Origin = new STRVector2f(0.5f, 0.5f);
            temp.Angle = -90f; // arranca apuntando hacia arriba
            Transform = temp;
        }

        // Vuelve al centro, quieto y apuntando hacia arriba, con invulnerabilidad
        public void Respawn(STRVector2f position)
        {
            STRTransform temp = Transform;
            temp.Position = position;
            temp.Angle = -90f;
            Transform = temp;

            Velocity = new STRVector2f(0, 0);
            invulnerableTimer = INVULNERABLE_TIME;
        }

        public override void Update(float deltaTime)
        {
            float rotationSpeed = 200f;
            float acceleration = 300f;

            if (ESTEngine.IsKeyDown(Keys.Left) || ESTEngine.IsKeyDown(Keys.A))
            {
                STRTransform t = Transform;
                t.Angle -= rotationSpeed * deltaTime;
                Transform = t;
            }
            if (ESTEngine.IsKeyDown(Keys.Right) || ESTEngine.IsKeyDown(Keys.D))
            {
                STRTransform t = Transform;
                t.Angle += rotationSpeed * deltaTime;
                Transform = t;
            }

            if (ESTEngine.IsKeyDown(Keys.Up) || ESTEngine.IsKeyDown(Keys.W))
            {
                float angleRad = (float)(Transform.Angle * Math.PI / 180.0f);
                STRVector2f dir = new STRVector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
                Velocity += dir * acceleration * deltaTime;
            }

            Velocity *= 0.99f; // Fricción
            base.Update(deltaTime);

            animation.Update(deltaTime);
            string currentFrame = animation.CurrentFrame;
            if (currentFrame != lastFrame)
            {
                SetSprite(currentFrame);
                lastFrame = currentFrame;
            }

            if (invulnerableTimer > 0)
                invulnerableTimer -= deltaTime;
        }

        public override void Draw()
        {
            // Mientras es invulnerable parpadea (se dibuja un frame si y otro no, 10 veces por segundo)
            if (IsInvulnerable && (int)(invulnerableTimer * 10) % 2 == 0)
                return;

            STRTransform drawTransform = Transform;
            drawTransform.Angle += SPRITE_ANGLE_OFFSET;
            ESTCamera.Draw(sprite, drawTransform);
        }
    }
}
