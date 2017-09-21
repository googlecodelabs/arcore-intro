using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour
{

  public Camera firstPersonCamera;
  private Anchor anchor;
  private int score = 0;

  void Start ()
  {
    // Hide the scoreboard UI until a plane is detected.
    foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
      r.enabled = false;
    }
  }

  void Update ()
  {
    // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
    if (Frame.TrackingState != FrameTrackingState.Tracking) {
      return;
    }

    if (anchor == null || anchor.TrackingState == AnchorTrackingState.StoppedTracking) {
      // Wait to detect a plane
      List<TrackedPlane> planes = new List<TrackedPlane> ();
      Frame.GetAllPlanes (ref planes);
      if (planes.Count > 0) {
        CreateAnchor (planes [0].Position);
      }
    } else {
      // Make the scoreboard face the viewer
      transform.LookAt (firstPersonCamera.transform);
    }
  }

  private void CreateAnchor (Vector3 point)
  {
    // Create the position of the anchor by raycasting a point towards the top of the screen.
    Vector2 pos = new Vector2 (Screen.width * .5f, Screen.height * .90f);
    Ray ray = firstPersonCamera.ScreenPointToRay (pos);
    Vector3 anchorPosition = ray.GetPoint (5f);

    // Create the anchor at that point.
    if (anchor != null) {
      DestroyObject (anchor);
    }
    anchor = Session.CreateAnchor (anchorPosition, Quaternion.identity);

    // Attach the scoreboard to the anchor.
    transform.position = anchorPosition;
    transform.SetParent (anchor.transform);

    // Finally, enable the renderers
    foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
      r.enabled = true;
    }
  }

  public void SetScore (int score)
  {
    if (this.score != score) {
      GetComponentInChildren<TextMesh> ().text = "Score: " + score;
      this.score = score;
    }
  }

}
