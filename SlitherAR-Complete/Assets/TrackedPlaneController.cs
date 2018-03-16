//-----------------------------------------------------------------------
//// <copyright file="TrackedPlaneController.cs" company="Google">
/////
///// Copyright 2017 Google Inc. All Rights Reserved.
/////
///// Licensed under the Apache License, Version 2.0 (the "License");
///// you may not use this file except in compliance with the License.
///// You may obtain a copy of the License at
/////
///// http://www.apache.org/licenses/LICENSE-2.0
/////
///// Unless required by applicable law or agreed to in writing, software
///// distributed under the License is distributed on an "AS IS" BASIS,
///// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///// See the License for the specific language governing permissions and
///// limitations under the License.
/////
///// </copyright>
/////-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

/// <summary>
/// TrackedPlaneController for Slither - AR codelab.
/// </summary>
/// <remarks>
/// See the codelab for the complete narrative of what
/// this class does.
/// </remarks>
public class TrackedPlaneController : MonoBehaviour
{
    // The plane being rendered.
    private TrackedPlane trackedPlane;
    // The renderer component for the plane.
    private PlaneRenderer planeRenderer;
    // The list of vertices defining the ARCore plane.
    private List<Vector3> polygonVertices = new List<Vector3>();

    void Awake()
    {
        // Initialize the plane renderer at awake, to avoid calling
        // GetComponent repeatedly.
        planeRenderer = GetComponent<PlaneRenderer>();
    }

    /// <summary>
    /// Sets the tracked plane and initializes the renderer.
    /// </summary>
    public void SetTrackedPlane(TrackedPlane plane)
    {
        trackedPlane = plane;
        trackedPlane.GetBoundaryPolygon (polygonVertices);
        planeRenderer.Initialize();
        planeRenderer.UpdateMeshWithCurrentTrackedPlane(
	    trackedPlane.CenterPose.position,
	    polygonVertices);
    }

    // Update is called once per frame.
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
        if (trackedPlane.TrackingState != TrackingState.Tracking || Session.Status != SessionStatus.Tracking)
        {
            planeRenderer.EnablePlane(false);
            return;
        }

        // OK! Valid plane, so enable rendering and update the polygon data if needed.
        planeRenderer.EnablePlane(true);

        // Update renderer mesh if there is change in plane polygon.
        List<Vector3> newPolygonVertices = new List<Vector3> ();
        trackedPlane.GetBoundaryPolygon (newPolygonVertices);
        if (!AreVerticesListsEqual (polygonVertices, newPolygonVertices)) {
            polygonVertices.Clear ();
            polygonVertices.AddRange (newPolygonVertices);

            planeRenderer.UpdateMeshWithCurrentTrackedPlane (trackedPlane.CenterPose.position,
                polygonVertices);
        }
    }

    bool AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
    {
        if (firstList.Count != secondList.Count)
        {
            return false;
        }

        for (int i = 0; i < firstList.Count; i++)
        {
            if (firstList[i] != secondList[i])
            {
                return false;
            }
        }
        return true;
    }

}
