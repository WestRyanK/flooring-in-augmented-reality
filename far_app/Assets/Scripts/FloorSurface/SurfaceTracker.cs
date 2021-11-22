using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceTracker : MonoBehaviour
{
    [SerializeField]
    private int TrackedRecentFramesCount;
    [SerializeField]
    private float FloorElevation;
    [SerializeField]
    private DataPointAverager.Calculation HeightAveragingCalculation;

    private ARRaycastManager _raycastManager;
    private Vector2 _centerScreen;
    private DataPointAverager _dataPointAverager;

    void Start()
    {
        Debug.Log("Initializing SurfaceTracker");
        _raycastManager = FindObjectOfType<ARRaycastManager>();
        _centerScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.05f);
        _dataPointAverager = new DataPointAverager(TrackedRecentFramesCount, HeightAveragingCalculation);
        SetTrackingObjectHeight(0);
        //StartCoroutine(RepeatAttachToSurface(TrackingInterval));
    }

    private IEnumerator RepeatAttachToSurface(float inInverval)
    {
        while (true)
        {
            yield return new WaitForSeconds(inInverval);
            AttachToSurface();
        }
    }

    void Update()
    {
        AttachToSurface();
    }

    private void AttachToSurface()
    {
        if (_raycastManager)
        {
            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
            _raycastManager.Raycast(_centerScreen, hitResults);
            if (hitResults.Count > 0)
            {
                // Debug.Log("Attaching to surface...  found " + hitResults.Count.ToString());
                //if (TrackingObject.transform.position.y > hitResults[0].pose.position.y)
                if (true)
                {
                    SetTrackingObjectHeight(hitResults[0].pose.position.y);
                }
            }
        }
    }

    private void SetTrackingObjectHeight(float inHeight)
    {
        _dataPointAverager.AddDataPoint(inHeight);
        float averageHeight = _dataPointAverager.Average;

        Vector3 position = gameObject.transform.position;
        position.y = averageHeight + FloorElevation;
        gameObject.transform.position = position;
        // Debug.Log("Position y: " + position.y.ToString());
    }
}
