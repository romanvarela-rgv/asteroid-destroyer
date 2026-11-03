namespace EngineGDI
{
    // Interfaz para guardar y cargar los datos. CLSGameManager usa esta interfaz y no sabe que es un archivo
    public interface INTSaveStore
    {
        CLSSaveData Load();
        void Save(CLSSaveData data);
    }
}
