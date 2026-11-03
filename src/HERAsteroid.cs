using System;

namespace EngineGDI
{
    public class HERAsteroid : ABSCharacter
    {
        public event Action<HERAsteroid> OnDestroyed;

        private float rotationSpeed;

        public ENUAsteroidSize Size { get; private set; }

        // Los mas chicos dan mas puntos porque cuesta mas pegarles
        public int Points => GetPoints(Size);

        public HERAsteroid(STRVector2f position, STRVector2f velocity, string spritePath, float angle, float rotationSpeed, ENUAsteroidSize size)
            : base(position)
        {
            this.Size = size;
            this.Velocity = velocity;
            this.rotationSpeed = rotationSpeed;

            SetSprite(spritePath);

            // Los sprites son de 32, 64 o 128px, se escalan para que todos midan lo mismo segun Size
            float scale = GetDiameter(size) / Transform.Size.X;

            STRTransform temp = Transform;
            temp.Angle  = angle;
            temp.Scale  = new STRVector2f(scale, scale);
            temp.Origin = new STRVector2f(0.5f, 0.5f);
            this.Transform = temp;
        }

        // Tamaño en pantalla, en pixeles
        public static float GetDiameter(ENUAsteroidSize size)
        {
            switch (size)
            {
                case ENUAsteroidSize.Large:  return 100f;
                case ENUAsteroidSize.Medium: return 60f;
                default:                  return 34f;
            }
        }

        public static int GetPoints(ENUAsteroidSize size)
        {
            switch (size)
            {
                case ENUAsteroidSize.Large:  return 20;
                case ENUAsteroidSize.Medium: return 50;
                default:                  return 100;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            STRTransform temp = Transform;
            temp.Angle += rotationSpeed * deltaTime;
            this.Transform = temp;
        }

        public override void Draw()
        {
            ESTCamera.Draw(sprite, Transform);
        }

        public void Destroy() // avisa con el evento OnDestroyed
        {
            OnDestroyed?.Invoke(this);
        }
    }
}
