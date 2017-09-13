using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;


// Moves the object based on the gaze.  The center of the screen is used 
// as the point when raycasted through  the camera.
using GoogleARCore.HelloAR;


public class FollowGazeController : MonoBehaviour {
  // object displayed showing the hittest location.
  public GameObject reticleObject;

  // The object to move with the gaze.
  public Transform head;

  // The camera to use when raycasting.
  public Camera m_firstPersonCamera;

  // speed to move.
  public float speed = 2;

	
	// Update is called once per frame
	void Update () {
    if (head == null) {
      Slithering s = GetComponent<Slithering>();
      if (s != null) {
        head = s.Head;
      }
    }

    Ray ray =  m_firstPersonCamera.ScreenPointToRay(new Vector2
      (Screen.width/2, Screen.height/2));

    TrackableHit hit;
    TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds;

    if (Session.Raycast(ray, raycastFilter, out hit))
    {
      Vector3 pt = hit.Point;
      pt.y = (hit.Plane.Position.y + head.position.y)/2;
    
      Vector3 pos = reticleObject.transform.position;
      pos.y = pt.y;
      reticleObject.transform.position = pos;
      reticleObject.GetComponent<PlaneAttachment>().Attach(hit.Plane);


      reticleObject.transform.position = Vector3.Lerp(reticleObject.transform.position ,pt,
        Time.smoothDeltaTime * speed* 10);

      head.LookAt(reticleObject.transform.position);
      } 

    // Only move if we're far away.
    if (Vector3.Distance(head.transform.position, reticleObject.transform.position) >
      0.1f){
      head.Translate(head.forward * speed * Time.smoothDeltaTime, Space.World);
   }
  }
}
