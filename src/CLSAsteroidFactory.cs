using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class CLSAsteroidFactory
    {
        private Random random = new Random();
        private float safeRadius = 200f;

        // Sprites agrupados por resolucion, para no agrandar de mas los de 32px
        private static readonly int[] bigSprites   = { 3, 8, 9, 10, 11 };       // 128px
        private static readonly int[] mediumSprites = { 2, 6, 7, 3, 8, 9 };     // 64px y 128px
        private static readonly int[] smallSprites  = { 1, 4, 5, 2, 6, 7 };     // 32px y 64px

        // Asteroide nuevo lejos del jugador. speedMultiplier sube en cada oleada.
        public HERAsteroid CreateSafeAsteroid(STRVector2f playerPosition, int screenWidth, int screenHeight,
                                           ENUAsteroidSize size = ENUAsteroidSize.Large, float speedMultiplier = 1f)
        {
            STRVector2f spawnPosition = new STRVector2f(0, 0);
            bool validPosition = false;

            while (!validPosition)
            {
                spawnPosition.X = random.Next(0, screenWidth);
                spawnPosition.Y = random.Next(0, screenHeight);

                float distanceToPLayer = STRVector2f.Distance(spawnPosition, playerPosition);

                if (distanceToPLayer > safeRadius)
                {
                    validPosition = true;
                }
            }

            float angle = (float)(random.NextDouble() * Math.PI * 2);
            STRVector2f velocity = new STRVector2f((float)Math.Cos(angle), (float)Math.Sin(angle)) * RandomSpeed(size) * speedMultiplier;

            return Create(spawnPosition, velocity, size);
        }

        // Los dos pedazos que salen al romper un asteroide (vacio si ya era chico)
        public List<HERAsteroid> CreateFragments(HERAsteroid parent)
        {
            var fragments = new List<HERAsteroid>();
            if (parent.Size == ENUAsteroidSize.Small) return fragments;

            ENUAsteroidSize childSize = parent.Size - 1;

            // Salen hacia los costados de la direccion en la que iba el padre
            float parentSpeed = parent.Velocity.Length();
            float baseAngle = parentSpeed > 0.01f
                ? (float)Math.Atan2(parent.Velocity.Y, parent.Velocity.X)
                : (float)(random.NextDouble() * Math.PI * 2);

            foreach (int side in new[] { -1, 1 })
            {
                float spread = (float)((0.4 + random.NextDouble() * 0.5) * side); // entre 23 y 50 grados
                float angle = baseAngle + spread;
                float speed = Math.Max(parentSpeed * 1.3f, RandomSpeed(childSize));
                STRVector2f velocity = new STRVector2f((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

                fragments.Add(Create(parent.Transform.Position, velocity, childSize));
            }
            return fragments;
        }

        private HERAsteroid Create(STRVector2f position, STRVector2f velocity, ENUAsteroidSize size)
        {
            int[] sprites = size == ENUAsteroidSize.Large ? bigSprites
                          : size == ENUAsteroidSize.Medium ? mediumSprites
                          : smallSprites;
            string spritePath = $"assets/textures/Animations/Idle_e/meteorito{sprites[random.Next(sprites.Length)]}.png";

            float angle = (float)(random.NextDouble() * 360);
            float rotationSpeed = (float)(random.NextDouble() * 100 - 50);

            return new HERAsteroid(position, velocity, spritePath, angle, rotationSpeed, size);
        }

        // Los mas chicos son mas rapidos
        private float RandomSpeed(ENUAsteroidSize size)
        {
            switch (size)
            {
                case ENUAsteroidSize.Large:  return 25f + (float)random.NextDouble() * 30f;
                case ENUAsteroidSize.Medium: return 45f + (float)random.NextDouble() * 40f;
                default:                  return 70f + (float)random.NextDouble() * 50f;
            }
        }
    }
}
