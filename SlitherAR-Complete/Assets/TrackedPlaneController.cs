using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class TrackedPlaneController : MonoBehaviour
{
    private TrackedPlane trackedPlane;
    private PlaneRenderer planeRenderer;
    private List<Vector3> polygonVertices = new List<Vector3>();

    void Awake()
    {
        planeRenderer = GetComponent<PlaneRenderer>();
    }

    public void SetTrackedPlane(TrackedPlane plane)
    {
        trackedPlane = plane;
        trackedPlane.GetBoundaryPolygon(ref polygonVertices);
        planeRenderer.Initialize();
        planeRenderer.UpdateMeshWithCurrentTrackedPlane(trackedPlane.Position, polygonVertices);
    }

    void Update()
    {
        // If no plane yet, disable the renderer and return.
        if (trackedPlane == null)
        {
            planeRenderer.EnablePlane(false);
            return;
        }

        // If this plane was subsumed by another plane, destroy this object, the other
        // plane's display will render it.
        if (trackedPlane.SubsumedBy != null)
        {
            Destroy(gameObject);
            return;
        }

        // If this plane is not valid or ARCore is not tracking, disable rendering.
        if (!trackedPlane.IsValid || Frame.TrackingState != FrameTrackingState.Tracking)
        {
            planeRenderer.EnablePlane(false);
            return;
        }

        // OK! Valid plane, so enable rendering and update the polygon data if needed.
        planeRenderer.EnablePlane(true);

        if (trackedPlane.IsUpdated)
        {
            trackedPlane.GetBoundaryPolygon(ref polygonVertices);
            planeRenderer.UpdateMeshWithCurrentTrackedPlane(trackedPlane.Position, polygonVertices);
        }
    }
}
