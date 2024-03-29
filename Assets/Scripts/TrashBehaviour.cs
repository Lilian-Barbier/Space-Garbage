using System.Collections.Generic;
using Assets.Scripts.Enums;
using Enums;
using Models;
using UnityEngine;
using Utils;

public class TrashBehaviour : MonoBehaviour
{
    [SerializeField] private bool setRandomDomino = false;
    [SerializeField] private bool setBlockMetalOrg = false;
    [SerializeField] private bool setLMetalOrg = false;

    SpriteRenderer spriteRenderer;
    TrashGenerator dominoGenerator;
    public Trash trash;

    int paintingLayer = 0;

    void Start()
    {
        dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();

        if (setRandomDomino)
            trash = new Trash(TrashUtils.GetRandomValidDomino());
        else if (setBlockMetalOrg)
            trash = new Trash(TrashUtils.Skew);
        else if (setLMetalOrg)
            trash = new Trash(TrashUtils.DoubleMetal);

        SetSpriteAndCollider();
    }

    public void SetTrash(Trash domino)
    {
        this.trash = domino;
        dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();
        SetSpriteAndCollider();
    }

    public void RotateDominoClockwise()
    {
        trash.Blocks = TrashUtils.RotateDominoClockwise(trash.Blocks);
        SetSpriteAndCollider();
    }

    public void RotateDominoCounterClockwise()
    {
        trash.Blocks = TrashUtils.RotateDominoCounterClockwise(trash.Blocks);
        SetSpriteAndCollider();
    }

    void SetSpriteAndCollider()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = dominoGenerator.GenerateTrashSprite(trash);

        Sprite sprite = spriteRenderer.sprite;
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();

        //Set BoxCollider2D to sprite size
        Rect croppedRect = new Rect(
          (sprite.textureRectOffset.x + sprite.textureRect.width / 2f) / sprite.pixelsPerUnit,
          (sprite.textureRectOffset.y + sprite.textureRect.height / 2f) / sprite.pixelsPerUnit,
          sprite.textureRect.width / sprite.pixelsPerUnit,
          sprite.textureRect.height / sprite.pixelsPerUnit);

        // offset is relative to sprite's pivot
        collider.offset = croppedRect.position - sprite.pivot / sprite.pixelsPerUnit;
        collider.size = croppedRect.size;
    }

    //Fonctions utilisés dans l'assembleur
    public void MoveDominoUp()
    {
        trash.Blocks = TrashUtils.MoveDominoUp(trash.Blocks);
    }
    public void MoveDominoRight()
    {
        trash.Blocks = TrashUtils.MoveDominoRight(trash.Blocks);
    }
    public void MoveDominoLeft()
    {
        trash.Blocks = TrashUtils.MoveDominoLeft(trash.Blocks);
    }
    public void MoveDominoDown()
    {
        trash.Blocks = TrashUtils.MoveDominoDown(trash.Blocks);
    }

    internal void AddColor(bool isPainterBlue, bool isPainterRed)
    {
        paintingLayer++;

        if (trash.isAssembled)
        {
            trash.SetColor(BlockColor.Failed);
            SetSpriteAndCollider();
            return;
        }

        if (isPainterBlue)
        {
            var color = trash.GetColor();
            if (color == BlockColor.Failed || color == BlockColor.Red || color == BlockColor.LightRed)
            {
                trash.SetColor(BlockColor.Failed);
            }
            else
            {
                switch (paintingLayer)
                {
                    case 1:
                        trash.SetColor(BlockColor.LightBlue);
                        break;
                    case 2:
                        trash.SetColor(BlockColor.Blue);
                        break;
                    case 3:
                        trash.SetColor(BlockColor.Failed);
                        break;
                }
            }

        }
        else if (isPainterRed)
        {
            var color = trash.GetColor();
            if (color == BlockColor.Failed || color == BlockColor.Blue || color == BlockColor.LightBlue)
            {
                trash.SetColor(BlockColor.Failed);
            }
            else
            {
                switch (paintingLayer)
                {
                    case 1:
                        trash.SetColor(BlockColor.LightRed);
                        break;
                    case 2:
                        trash.SetColor(BlockColor.Red);
                        break;
                    case 3:
                        trash.SetColor(BlockColor.Failed);
                        break;
                }
            }
        }
        SetSpriteAndCollider();
    }

    public int GetTrashSize()
    {
        return trash.GetTrashSize();
    }

    public bool IsOnlyOneMaterialTrash(MaterialType material)
    {
        foreach (var line in trash.Blocks)
        {
            foreach (var block in line)
            {
                if (block.Exists && block.Material != material)
                    return false;
            }
        }
        return true;
    }

    public bool IsOnlyOneMaterialTrash()
    {
        MaterialType material = MaterialType.None;
        foreach (var line in trash.Blocks)
        {
            foreach (var block in line)
            {
                if (block.Exists && material == MaterialType.None)
                {
                    material = block.Material;
                }
                else if (block.Exists && block.Material != material)
                    return false;
            }
        }
        return true;
    }

    public MaterialType GetTrashMaterial()
    {
        foreach (var line in trash.Blocks)
        {
            foreach (var block in line)
            {
                if (block.Exists)
                    return block.Material;
            }
        }
        return MaterialType.None;
    }

    public List<MaterialType> GetBlocksMaterialType()
    {
        List<MaterialType> materialTypes = new List<MaterialType>();
        foreach (var line in trash.Blocks)
        {
            foreach (var block in line)
            {
                if (block.Exists)
                    materialTypes.Add(block.Material);
            }
        }
        return materialTypes;
    }
}
