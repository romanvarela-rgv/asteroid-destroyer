using System;

namespace EngineGDI
{
    public abstract class ABSCharacter
    {
        // Campos privados
        private STRTransform transform;
        private STRVector2f velocity;
        protected string sprite;

        // Propiedades públicas
        public STRTransform Transform 
        { 
            get { return transform; } 
            set { transform = value; } 
        }

        public STRVector2f Velocity 
        { 
            get { return velocity; } 
            set { velocity = value; } 
        }
        
        public ABSCharacter(STRVector2f position, float scale = 1f)
        {
            this.transform = new STRTransform(position.X, position.Y, scale);
        }
        
        protected void SetSprite(string path)
        {
            this.sprite = path;
            STRTransform t = this.Transform;
            t.Size = ESTEngine.GetTextureSize(path);
            this.Transform = t;
        }

        public virtual void Update(float deltaTime)
        {
            transform.Position += velocity * deltaTime;
            Wrap();
        }

        protected virtual void Wrap()
        {
            if (transform.Position.X < -50) transform.Position.X = ESTProgram.SCREEN_WIDTH + 50;
            if (transform.Position.X > ESTProgram.SCREEN_WIDTH + 50) transform.Position.X = -50;
            if (transform.Position.Y < -50) transform.Position.Y = ESTProgram.SCREEN_HEIGHT + 50;
            if (transform.Position.Y > ESTProgram.SCREEN_HEIGHT + 50) transform.Position.Y = -50;
        }

        public abstract void Draw();
    }
}
