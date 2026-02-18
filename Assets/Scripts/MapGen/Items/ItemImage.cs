using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class ItemImage : MonoBehaviour {
    public Item originalItem;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private MeshFilter meshFilter;

    internal void UpdateImage(Item itemInput)
    {
        if (originalItem == itemInput)
            return;
        originalItem = itemInput;
        if (itemInput.image == null)
            return;
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (originalMaterial == null)
            originalMaterial = meshRenderer.sharedMaterial;

        Color partColor = ContentLoader.GetColor(itemInput);
        int textureIndex = ContentLoader.GetPatternIndex(itemInput.material);
        int shapeIndex = ContentLoader.GetShapeIndex(itemInput.material);

        meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, partColor.a);

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", partColor);
        prop.SetInt("_MatIndex", textureIndex);
        prop.SetInt("_ShapeIndex", shapeIndex);
        meshRenderer.SetPropertyBlock(prop);

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = ImageManager.Instance.CreateMesh(itemInput.image, ImageManager.Direction.Front);
    }
}
