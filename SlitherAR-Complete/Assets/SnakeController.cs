using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class SnakeController : MonoBehaviour
{
  public GameObject pointer;
  public Camera firstPersonCamera;
  // speed to move.
  public float speed = 20f;


  private TrackedPlane trackedPlane;

  public GameObject snakeHeadPrefab;
  private GameObject snakeInstance;

  void Update ()
  {
    if (snakeInstance == null || snakeInstance.activeSelf == false) {
      pointer.SetActive (false);
      return;
    }
    pointer.SetActive (true);
    Ray ray = firstPersonCamera.ScreenPointToRay (
                new Vector2 (Screen.width / 2, Screen.height / 2));

    TrackableHit hit;
    TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds;

    if (Session.Raycast (ray, raycastFilter, out hit)) {
      Vector3 pt = hit.Point;
      pt.y = (hit.Plane.Position.y + snakeInstance.transform.position.y) / 2f;
      // Set the y position relative to the plane and attach the pointer to the plane
      Vector3 pos = pointer.transform.position;
      pos.y = pt.y;
      pointer.transform.position = pos;
      pointer.GetComponent<PlaneAttachment> ().Attach (hit.Plane);

      // Now lerp to the position

      pointer.transform.position = Vector3.Lerp (pointer.transform.position, pt,
        Time.smoothDeltaTime * speed);

    }
    // move towards the pointer
    // Only move if we're far away.
    if (Vector3.Distance (snakeInstance.transform.position, pointer.transform.position) >
        0.1f) {
      // then look at the pointer
      snakeInstance.transform.LookAt (pointer.transform.position);
      snakeInstance.transform.position = Vector3.Lerp (
        snakeInstance.transform.position,
        pointer.transform.position,
        speed * Time.smoothDeltaTime);
    }

  }

  public void SetPlane (TrackedPlane plane)
  {
    trackedPlane = plane;
    // Spawn a new snake.
    SpawnSnake ();
  }

  private void SpawnSnake ()
  {
    if (snakeInstance != null) {
      DestroyImmediate (snakeInstance);
    }

    Vector3 pos = trackedPlane.Position;
    pos.y += 0.1f;

    // Not anchored, it is rigidbody that is influenced by the physics engine.
    snakeInstance = Instantiate (snakeHeadPrefab, pos, Quaternion.identity, transform);

    // Pass the head to the slithering component to make movement work.
    GetComponent<Slithering> ().Head = snakeInstance.transform;

    snakeInstance.AddComponent<FoodConsumer> ();
  }

  public int GetLength ()
  {
    return GetComponent<Slithering> ().GetLength ();
  }

}
