//-----------------------------------------------------------------------
//// <copyright file="SceneController.cs" company="Google">
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
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

/// <summary>
/// SceneController for Slither - AR codelab.
/// </summary>
/// <remarks>
/// See the codelab for the complete narrative of what
/// this class does.
/// </remarks>
public class SceneController : MonoBehaviour
{

    // Prefab used to render all the tracked planes.
    public GameObject trackedPlanePrefab;
    // Camera used for tap input raycasting.
    public Camera firstPersonCamera;
    public ScoreboardController scoreboard;
    public SnakeController snakeController;

    // Use this for initialization
    void Start()
    {
        // Check on startup that this device is compatible with ARCore apps.
        QuitOnConnectionErrors();
    }

    // Update is called once per frame
    void Update()
    {
        // The session status must be Tracking in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking) {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Add this to the bottom of Update
        ProcessNewPlanes();

        // Add to the end of Update()
        ProcessTouches();

        scoreboard.SetScore(snakeController.GetLength());
    }

    /// <summary>
    /// Quit the application if there was a connection error for the ARCore session.
    /// </summary>
    void QuitOnConnectionErrors()
    {
        // Do not update if ARCore is not tracking.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            StartCoroutine (CodelabUtils.ToastAndExit (
                "Camera permission is needed to run this application.", 5));
        } else if (Session.Status.IsError ()) {
            // This covers a variety of errors.  See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            StartCoroutine (CodelabUtils.ToastAndExit (
                "ARCore encountered a problem connecting.  Please start the app again.", 5));
        }
    }

    /// <summary>
    /// List the newly detected planes from the ARCore Frame object and render them.
    /// </summary>
    void ProcessNewPlanes()
    {
        List<TrackedPlane> planes = new List<TrackedPlane>();
        Session.GetTrackables (planes, TrackableQueryFilter.New);

        for (int i = 0; i < planes.Count; i++) {
            // Instantiate a plane visualization prefab and set it to track the new plane.
            // The transform is set to the origin with an identity rotation since the mesh
            // for our prefab is updated in Unity World coordinates.
            GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero,
                                Quaternion.identity, transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);
        }
    }

    /// <summary>
    /// Processes a single tap to select a plane based on a hittest.
    /// </summary>
    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast (touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane (hit.Trackable as TrackedPlane);
        }
    }

    void SetSelectedPlane (TrackedPlane selectedPlane)
    {
        Debug.Log("Selected plane centered at " + selectedPlane.CenterPose.position);
        // Add to the end of SetSelectedPlane.
        scoreboard.SetSelectedPlane(selectedPlane);
        // Add to SetSelectedPlane()
        snakeController.SetPlane(selectedPlane);

        // Add to the bottom of SetSelectedPlane()
        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }
}
