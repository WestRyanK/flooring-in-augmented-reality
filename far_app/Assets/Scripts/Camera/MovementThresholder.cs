using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementThresholder : MonoBehaviour
{
    [SerializeField]
    private float _positionDistanceThreshold;
    [SerializeField]
    private float _orientationAngleThreshold;

    private Vector3 _savedPosition;
    private Quaternion _savedOrientation;

    public void SaveTransform()
    {
        _savedPosition = transform.position;
        _savedOrientation = transform.rotation;
    }

    public bool HasExceededMovementThreshold()
    {
        float positionDistance = Vector3.Distance(_savedPosition, transform.position);
        float orientationDistance = Quaternion.Angle(_savedOrientation, transform.rotation);


        bool overPositionThreshold = positionDistance > _positionDistanceThreshold;
        bool overOrientationThreshold = orientationDistance > _orientationAngleThreshold;

        // Debug.Log($"Pos: {positionDistance} {overPositionThreshold} Ori: {orientationDistance} {overOrientationThreshold}");
        return overPositionThreshold || overOrientationThreshold;
    }
}
