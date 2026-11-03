namespace EngineGDI
{
    public struct STRTransform
    {
        public STRVector2f Position;
        public STRVector2f Scale;
        public float Angle;
        public STRVector2f Origin;
        public STRVector2f Size; // Tamaño base del objeto (sin escalar)

        public STRTransform(float x, float y, float scale = 1.0f)
        {
            Position = new STRVector2f(x, y);
            Scale = new STRVector2f(scale, scale);
            Angle = 0;
            Origin = new STRVector2f(0, 0);
            Size = new STRVector2f(0, 0);
        }

        public STRTransform(STRVector2f position, float scale = 1.0f)
        {
            Position = position;
            Scale = new STRVector2f(scale, scale);
            Angle = 0;
            Origin = new STRVector2f(0, 0);
            Size = new STRVector2f(0, 0);
        }
    }
}
