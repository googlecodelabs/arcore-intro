//-----------------------------------------------------------------------
//// <copyright file="ScoreboardController.cs" company="Google">
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
/// Scoreboard for Slither - AR codelab.
/// </summary>
/// <remarks>
/// See the codelab for the complete narrative of what
/// this class does.
/// </remarks>
public class ScoreboardController : MonoBehaviour
{
    // Used for gaze raycasting.
    public Camera firstPersonCamera;
    // The fixed position in the real world for the scoreboard.
    private Anchor anchor;
    // The plane the scoreboard is positioned above/near.
    private TrackedPlane trackedPlane;
    // The scoreboard's offset from the plane in the Y axis.
    private float yOffset;
    private int score = 0;

    // Use this for initialization
    void Start()
    {
        // Disable all the renderers on startup.
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
        if (Frame.TrackingState != FrameTrackingState.Tracking)
        {
            return;
        }

        // If there is no plane, then return
        if (trackedPlane == null)
        {
            return;
        }

        // Check for the plane being subsumed
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (trackedPlane.SubsumedBy != null)
        {
            trackedPlane = trackedPlane.SubsumedBy;
        }

        // Make the scoreboard face the viewer
        transform.LookAt(firstPersonCamera.transform);

        // Move the position to stay consistent with the plane
        if (trackedPlane.IsUpdated)
        {
            transform.position = new Vector3(transform.position.x,
                trackedPlane.Position.y + yOffset, transform.position.z);
        }
    }

    /// <summary>
    /// Sets the selected plane and anchor it.
    /// </summary>
    public void SetSelectedPlane(TrackedPlane trackedPlane)
    {
        this.trackedPlane = trackedPlane;
        CreateAnchor();
    }

    /// <summary>
    /// Create an ARCore Anchor to position  the scoreboard.
    /// </summary>
    private void CreateAnchor()
    {
        // Create the position of the anchor by raycasting a point towards the top of the screen.
        Vector2 pos = new Vector2(Screen.width * .5f, Screen.height * .90f);
        Ray ray = firstPersonCamera.ScreenPointToRay(pos);
        Vector3 anchorPosition = ray.GetPoint(5f);

        // Create the anchor at that point.
        if (anchor != null)
        {
            DestroyObject(anchor);
        }
        anchor = Session.CreateAnchor(anchorPosition, Quaternion.identity);

        // Attach the scoreboard to the anchor.
        transform.position = anchorPosition;
        transform.SetParent(anchor.transform);

        // record the y offset from the plane
        yOffset = transform.position.y - trackedPlane.Position.y;

        // Finally, enable the renderers
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    /// <summary>
    /// Sets the score and propagates it to the child mesh.
    /// </summary>
    public void SetScore(int score)
    {
        if (this.score != score)
        {
            GetComponentInChildren<TextMesh>().text = "Score: " + score;
            this.score = score;
        }
    }
}
