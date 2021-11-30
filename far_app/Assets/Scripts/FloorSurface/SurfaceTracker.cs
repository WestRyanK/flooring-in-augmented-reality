using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceTracker : MonoBehaviour
{
    [SerializeField]
    private float FloorElevation;
    private ARPlaneManager _arPlaneManager;

    void Start()
    {
        Debug.Log("Initializing SurfaceTracker");
        _arPlaneManager = FindObjectOfType<ARPlaneManager>();
        if (_arPlaneManager)
        {
            _arPlaneManager.planesChanged += OnArPlanesChanged;
        }
        else
        {
            Debug.Log("ArPlaneManager was null in SurfaceTracker!");
        }
    }

    void OnDestroy()
    {
        _arPlaneManager.planesChanged -= OnArPlanesChanged;
    }

    private ARPlane FindBiggestPlane()
    {
        ARPlane biggestPlane = null;
        float biggestPlaneSize = 0;
        foreach (ARPlane plane in _arPlaneManager.trackables)
        {
            float planeSize = plane.size.magnitude;
            if (planeSize > biggestPlaneSize)
            {
                biggestPlaneSize = planeSize;
                biggestPlane = plane;
            }
        }

        return biggestPlane;
    }

    private void OnArPlanesChanged(ARPlanesChangedEventArgs inArgs)
    {
        ARPlane biggestPlane = FindBiggestPlane();
        if (biggestPlane)
        {
            SetTrackingObjectHeight(biggestPlane.center.y);
        }
    }

    private void SetTrackingObjectHeight(float inHeight)
    {
        Vector3 position = gameObject.transform.position;
        position.y = inHeight + FloorElevation;
        gameObject.transform.position = position;
    }
}
