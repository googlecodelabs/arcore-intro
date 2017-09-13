using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour
{

  public Camera firstPersonCamera;

  private Anchor anchor;

  private int score = 0;

  // Update is called once per frame
  void Update ()
  {
    // The tracking state must be FrameTrackingState.Tracking in order to
    // access the Frame, including creating anchors.
    if (Frame.TrackingState != FrameTrackingState.Tracking) {
      return;
    }

    if (anchor == null ||
          anchor.TrackingState == AnchorTrackingState.StoppedTracking) {
      CreateAnchor ();
    }

    // Make the scoreboard face the viewer
    transform.LookAt (firstPersonCamera.transform);

  }

  private void CreateAnchor ()
  {

    Vector2 pos = new Vector2 (Screen.width / 2, Screen.height * .70f);
    Ray ray = firstPersonCamera.ScreenPointToRay (pos);
    Vector3 anchorPosition = ray.GetPoint (2f);

    anchor = Session.CreateAnchor (anchorPosition, Quaternion.identity);

    transform.SetParent (anchor.transform);
    transform.localPosition = Vector3.zero;
  }

  public void SetScore (int score)
  {
    if (this.score != score) {
      GetComponentInChildren<TextMesh> ().text = "Score: " + score;
      this.score = score;
    }
  }
}
