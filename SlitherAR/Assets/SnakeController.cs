using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class SnakeController : MonoBehaviour {

  private TrackedPlane trackedPlane;

  public GameObject snakeHeadPrefab;

  private GameObject snakeInstance;

  public GameObject pointer;
  public Camera firstPersonCamera;

  // speed to move.
  public float speed = 20f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (snakeInstance == null || snakeInstance.activeSelf == false) {
      pointer.SetActive(false);
      return;
    }

    if (!pointer.activeSelf) {
      pointer.transform.position = snakeInstance.transform.position;
      pointer.SetActive(true);
    }

    Ray ray =  firstPersonCamera.ScreenPointToRay(new Vector2
      (Screen.width/2, Screen.height/2));

    TrackableHit hit;
    TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds;

    if (Session.Raycast(ray, raycastFilter, out hit))
    {
      Vector3 pt = hit.Point;
      pt.y = snakeInstance.transform.position.y;
    

    // Set the y position relative to the plane and attach the pointer to the plane
    Vector3 pos = pointer.transform.position;
      pos.y = pt.y;
    pointer.transform.position = pos;
    pointer.GetComponent<PlaneAttachment>().Attach(hit.Plane);

   // Now lerp to the position

      pointer.transform.position = Vector3.Lerp(pointer.transform.position ,pt,
      Time.smoothDeltaTime * speed);

      // then look at the pointer
      snakeInstance.transform.LookAt(pointer.transform.position);
  }

   // move towards the pointer
    // Only move if we're far away.

    //TODO: make this smooooother
    if (Vector3.Distance(snakeInstance.transform.position, pointer.transform.position) >
      0.2f){
      snakeInstance.transform.Translate(snakeInstance.transform.forward * speed * Time.smoothDeltaTime, Space.World);
    }
	}

  public void SetPlane (TrackedPlane plane)
  {
    trackedPlane = plane;
    // Spawn a new snake.
    SpawnSnake();
  }

  private void SpawnSnake() {
    if (snakeInstance != null) {
      DestroyImmediate(snakeInstance);
    }

    Vector3 pos = trackedPlane.Position;
    pos.y += .2f;
    // Not anchored, it is rigidbody that is influenced by the physics engine.
    snakeInstance = Instantiate (snakeHeadPrefab, trackedPlane.Position, Quaternion.identity, transform);

    //TODO: scale it smartly.

    GetComponent<Slithering>().Head = snakeInstance.transform;
   
    snakeInstance.AddComponent<FoodConsumer>().enabled = true;
   
  }

  public int GetLength() {
    return GetComponent<Slithering>().GetLength();
  }
}
