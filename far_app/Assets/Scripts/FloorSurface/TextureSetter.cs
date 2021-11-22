using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSetter : MonoBehaviour
{
    private Material _material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            _material = renderer.material;
        }
    }

    public void SetTexture(Texture2D inTexture)
    {
        Debug.Log("TextureSet###");
        if (_material)
        {
            _material.mainTexture = inTexture;
        }
        else
        {
            Debug.Log($"Material for TextureSetter is null. GameObject must not have Renderer.");
        }
    }
}
