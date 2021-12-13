using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class FrustumViewer : MonoBehaviour
{
    public Color _frustumLineColor;
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }
    void Update()
    {
        DrawFrustum();
    }

    void DrawFrustum()
    {
        Vector3[] nearCorners = new Vector3[4]; 
        Vector3[] farCorners = new Vector3[4]; 
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(_camera); 

        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop
        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = Plane3Intersect(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = Plane3Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], _frustumLineColor, Time.deltaTime, true); //near corners on the created projection matrix
            Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], _frustumLineColor, Time.deltaTime, true); //far corners on the created projection matrix
            Debug.DrawLine(nearCorners[i], farCorners[i], _frustumLineColor, Time.deltaTime, true); //sides of the created projection matrix
        }
    }



    Vector3 Plane3Intersect(Plane p1, Plane p2, Plane p3)
    { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
         (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }
}