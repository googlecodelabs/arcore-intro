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
        // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
        if (Frame.TrackingState != FrameTrackingState.Tracking)
        {
            const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
            Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
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
    private void QuitOnConnectionErrors()
    {
        // Do not update if ARCore is not tracking.
        if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                    "This device does not support ARCore.", 5));
        }
        else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                    "Camera permission is needed to run this application.", 5));
        }
        else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                    "ARCore encountered a problem connecting.  Please start the app again.", 5));
        }
    }

    /// <summary>
    /// List the newly detected planes from the ARCore Frame object and render them.
    /// </summary>
    private void ProcessNewPlanes()
    {
        List<TrackedPlane> planes = new List<TrackedPlane>();
        Frame.GetNewPlanes(ref planes);
        for (int i = 0; i < planes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
            // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
            // coordinates.
            GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero, Quaternion.identity,
                                         transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);
        }
    }

    /// <summary>
    /// Processes a single tap to select a plane based on a hittest.
    /// </summary>
    private void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

        if (Session.Raycast(firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Plane);
        }
    }

    /// <summary>
    /// Sets  the selected plane and passes it to the other controllers that are part of the scene.
    /// </summary>
    private void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        Debug.Log("Selected plane centered at " + selectedPlane.Position);
        // Add to the end of SetSelectedPlane
        scoreboard.SetSelectedPlane(selectedPlane);
        // Add to SetSelectedPlane()
        snakeController.SetPlane(selectedPlane);

        // Add to the bottom of SetSelectedPlane()
        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }
}
