using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

  public GameObject trackedPlanePrefab;
  public Camera firstPersonCamera;
  public ScoreboardController scoreboard;
  public SnakeController snakeController;

    void Update () {
    _QuitOnConnectionErrors ();
    // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
    if (Frame.TrackingState != FrameTrackingState.Tracking) {
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

  /// <summary>Coroutine to display an error then exit.</summary>
  private IEnumerator ToastAndExit (string message, int seconds)
  {
    _ShowAndroidToastMessage (message);
    yield return new WaitForSeconds (seconds);
    Application.Quit ();
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
        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject> (
          "makeText", unityActivity,message, 0);
        toastObject.Call ("show");
      }));
    }
  }

  /// <summary>
  /// Quit the application if there was a connection error for the ARCore session.
  /// </summary>
  private void _QuitOnConnectionErrors ()
  {
    // Do not update if ARCore is not tracking.
    if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported) {
      StartCoroutine (ToastAndExit ("This device does not support ARCore.", 5));
    } else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission) {
      StartCoroutine (ToastAndExit ("Camera permission is needed to run this application.", 5));
    } else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed) {
      StartCoroutine (ToastAndExit ("ARCore encountered a problem connecting.  Please start the app again.", 5));
    }
  }

  private void ProcessNewPlanes() {
    List<TrackedPlane> planes = new List<TrackedPlane>();
    Frame.GetNewPlanes(ref planes);
    for(int i=0;i<planes.Count;i++) {
      // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
      // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
      // coordinates.
      GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero, Quaternion.identity,
        transform);
      planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);
    }
  }

  private void ProcessTouches() {
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

  private void SetSelectedPlane(TrackedPlane selectedPlane)
  {
    Debug.Log("Selected plane centered at " + selectedPlane.Position);
    scoreboard.SetSelectedPlane(selectedPlane);
    snakeController.SetPlane(selectedPlane);
    GetComponent<FoodController>().SetSelectedPlane(selectedPlane);

  }
}
