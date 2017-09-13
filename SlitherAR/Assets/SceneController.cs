using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour
{

  public GameObject trackedPlanePrefab;
  public SnakeController snakeController;
  public ScoreboardController scoreboardController;

  public Camera firstPersonCamera;

  private TrackedPlane selectedPlane;

  void Update ()
  {
    _QuitOnConnectionErrors ();
    // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
    if (Frame.TrackingState != FrameTrackingState.Tracking) {
      const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
      Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
      return;
    }

    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    ProcessNewPlanes ();

    ProcessTouches ();

    scoreboardController.SetScore (snakeController.GetLength ());

  }

  private void ProcessTouches ()
  {
    Touch touch;
    if (Input.touchCount < 1 || (touch = Input.GetTouch (0)).phase != TouchPhase.Began) {
      return;
    }

    TrackableHit hit;
    TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

    if (Session.Raycast (firstPersonCamera.ScreenPointToRay (touch.position), raycastFilter, out hit)) {
      SetSelectedPlane (hit.Plane);
    }
  }

  private void SetSelectedPlane (TrackedPlane plane)
  {
    selectedPlane = plane;
    GetComponent<FoodController> ().SetPlane (selectedPlane);
    snakeController.SetPlane (selectedPlane);
  }

  void ProcessNewPlanes ()
  {
    List<TrackedPlane> planes = new List<TrackedPlane> ();
    Frame.GetNewPlanes (ref planes);
    for (int i = 0; i < planes.Count; i++) {
      TrackedPlane plane = planes [i];
// Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
// the origin with an identity rotation since the mesh for our prefab is updated in Unity World
// coordinates.
      GameObject planeObject = Instantiate (trackedPlanePrefab, Vector3.zero, Quaternion.identity,
                           transform);
      planeObject.GetComponent<TrackedPlaneController> ().SetTrackedPlane (planes [i]);


      /*
       * *   DEBUG CODE FOR CLAYTON - NOT FOR THE CODELAB!
       */
      Vector3 newG = planes [i].Rotation * Vector3.down;
      newG.Normalize ();
      Debug.Log ("CWCW plane[" + i + "] gravity is " + newG);

      List<Vector3> poly = new List<Vector3> ();
      plane.GetBoundaryPolygon (ref poly);
      float diff = 0;
      foreach (Vector3 v in poly) {
        if (Mathf.Abs (v.y) < 100) {
          diff = Mathf.Abs (poly [0].y - v.y);
        }
      }
      if (diff > 0) {
        Debug.Log ("CWCW Poly ct =  " + poly.Count);
        for (int v = 0; v < poly.Count; v++) {
          if (Mathf.Abs (poly [v].y) < 100) {
            Debug.Log ("CWCW poly[" + v + "] = " + poly [v]);
          } else {
            Debug.Log ("CWCW SKIPPED poly[" + v + "] = " + poly [v]);
          }
        }
      }
      /*
       * * END OF DEBUG CODE
       */
    }
  }

  /// <summary>
  /// Quit the application if there was a connection error for the ARCore session.
  /// </summary>
  private void _QuitOnConnectionErrors ()
  {
// Do not update if ARCore is not tracking.
    if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported) {
      _ShowAndroidToastMessage ("This device does not support ARCore.");
      Application.Quit ();
    } else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission) {
      _ShowAndroidToastMessage ("Camera permission is needed to run this application.");
      Application.Quit ();
    } else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed) {
      _ShowAndroidToastMessage ("ARCore encountered a problem connecting.  Please start the app again.");
      Application.Quit ();
    }
  }

  /// <summary>
  /// Show an Android toast message.
  /// </summary>
  /// <param name="message">Message string to show in the toast.</param>
  /// <param name="length">Toast message time length.</param>
  private static void _ShowAndroidToastMessage (string message)
  {
    AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
    AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

    if (unityActivity != null) {
      AndroidJavaClass toastClass = new AndroidJavaClass ("android.widget.Toast");
      unityActivity.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject> ("makeText", unityActivity,
                                  message, 0);
        toastObject.Call ("show");
      }));
    }
  }
}
