using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorChanger : MonoBehaviour
{
    public Transform _projectorParent;
    public Material _segmentationProjectorMaterial;
    public Material _lightingProjectorMaterial;
    public List<Texture2D> _segmentationTextures;
    public List<Texture2D> _lightingTextures;
    int _currentIndex = 0;
    
    void Start()
    {
        UpdateProjector();
    }

    public void UpdateProjectorToNextValue()
    {
        _currentIndex++;
        UpdateProjector();
    }

    public void ResetProjector()
    {
        _currentIndex = 0;
        UpdateProjector();
    }

    void UpdateProjector()
    {
        _projectorParent.position = this.transform.position;
        _projectorParent.rotation = this.transform.rotation;
        _segmentationProjectorMaterial.mainTexture = _segmentationTextures[_currentIndex];
        _lightingProjectorMaterial.mainTexture = _lightingTextures[_currentIndex];
    }
}
