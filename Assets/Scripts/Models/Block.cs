using Assets.Scripts.Enums;

namespace Models
{
    public class Block
    {
        public bool Exists { get; set; }
        public MaterialType Material { get; set; }

        public Block(MaterialType material)
        {
            Exists = true;
            Material = material;
        }

        public Block()
        {
            Exists = false;
        }
    }
}

