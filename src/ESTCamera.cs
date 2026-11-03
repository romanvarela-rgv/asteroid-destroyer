namespace EngineGDI
{
    // Camara para el screen shake. Lo que se dibuja con ESTCamera.Draw se mueve con el shake,
    // el fondo y el HUD usan ESTEngine.Draw y quedan quietos
    public static class ESTCamera
    {
        public static STRVector2f Offset = new STRVector2f(0, 0);

        public static void Draw(string path, STRTransform transform)
        {
            transform.Position += Offset; // es un struct, asi que se cambia una copia
            ESTEngine.Draw(path, transform);
        }
    }
}
