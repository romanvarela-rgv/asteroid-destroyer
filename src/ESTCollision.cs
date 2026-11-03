using System;

namespace EngineGDI
{
    public static class ESTCollision
    {
        // Colision entre dos rectangulos (AABB), usa el Size y la escala de cada transform
        public static bool CheckAABB(STRTransform t1, STRTransform t2)
        {
            // Tamaño real con la escala
            float w1 = t1.Size.X * t1.Scale.X;
            float h1 = t1.Size.Y * t1.Scale.Y;
            float w2 = t2.Size.X * t2.Scale.X;
            float h2 = t2.Size.Y * t2.Scale.Y;

            // Bordes de cada rectangulo segun el pivote
            float left1 = t1.Position.X - (t1.Origin.X * w1);
            float right1 = left1 + w1;
            float top1 = t1.Position.Y - (t1.Origin.Y * h1);
            float bottom1 = top1 + h1;

            float left2 = t2.Position.X - (t2.Origin.X * w2);
            float right2 = left2 + w2;
            float top2 = t2.Position.Y - (t2.Origin.Y * h2);
            float bottom2 = top2 + h2;

            // Chocan si se superponen en X y en Y
            return right1 > left2 &&
                   left1 < right2 &&
                   bottom1 > top2 &&
                   top1 < bottom2;
        }

        // Colision circular por distancia. Si no se pasa radio se calcula con el Size
        public static bool CheckCircle(STRTransform t1, STRTransform t2)
        {
            float r1 = (t1.Size.X / 2f) * t1.Scale.X;
            float r2 = (t2.Size.X / 2f) * t2.Scale.X;
            float dist = STRVector2f.Distance(t1.Position, t2.Position);
            return dist < (r1 + r2);
        }
    }
}
