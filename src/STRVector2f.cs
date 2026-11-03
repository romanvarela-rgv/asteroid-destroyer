using System;

namespace EngineGDI
{
    public struct STRVector2f 
    {
        public float X;
        public float Y;

        public STRVector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Operadores de suma
        public static STRVector2f operator +(STRVector2f a, STRVector2f b) => new STRVector2f(a.X + b.X, a.Y + b.Y);
        public static STRVector2f operator +(STRVector2f a, float b) => new STRVector2f(a.X + b, a.Y + b);

        // Operadores de resta
        public static STRVector2f operator -(STRVector2f a, STRVector2f b) => new STRVector2f(a.X - b.X, a.Y - b.Y);
        public static STRVector2f operator -(STRVector2f a, float b) => new STRVector2f(a.X - b, a.Y - b);

        // Operadores de multiplicación
        public static STRVector2f operator *(STRVector2f a, float b) => new STRVector2f(a.X * b, a.Y * b);
        public static STRVector2f operator *(float a, STRVector2f b) => new STRVector2f(a * b.X, a * b.Y);

        // Operadores de división
        public static STRVector2f operator /(STRVector2f a, float b) => new STRVector2f(a.X / b, a.Y / b);

        // Métodos de utilidad
        public float Length() => (float)Math.Sqrt(X * X + Y * Y);

        public STRVector2f Normalize()
        {
            float len = Length();
            return len > 0 ? this / len : new STRVector2f(0, 0);
        }

        public static float Distance(STRVector2f a, STRVector2f b) => (a - b).Length();

        public static STRVector2f Lerp(STRVector2f a, STRVector2f b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return a + (b - a) * t;
        }

        public static float Lerp(float a, float b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return a + (b - a) * t;
        }

        public override string ToString() => $"({X}, {Y})";
    }
}
