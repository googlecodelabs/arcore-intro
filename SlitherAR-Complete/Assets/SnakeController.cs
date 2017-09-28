using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SnakeController : MonoBehaviour
{

	private TrackedPlane trackedPlane;

	public GameObject snakeHeadPrefab;
	private Rigidbody snakeInstance;
	private Slithering slitherComponent;
	public GameObject pointer;
	public Camera firstPersonCamera;
	// speed to move.
	public float speed = 20f;


	// Update is called once per frame
	void Update ()
	{
		if (snakeInstance == null || snakeInstance.gameObject.activeSelf == false) {
			pointer.SetActive (false);
			return;
		} else {
			pointer.SetActive (true);
		}


		Ray ray = firstPersonCamera.ScreenPointToRay (new Vector2 (Screen.width / 2, Screen.height / 2));
		TrackableHit hit;
		TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds;

		if (Session.Raycast (ray, raycastFilter, out hit)) {
			Vector3 pt = hit.Point;
			//Set the Y to the Y of the snakeInstance
			pt.y = snakeInstance.transform.position.y;
			// Set the y position relative to the plane and attach the pointer to the plane
			Vector3 pos = pointer.transform.position;
			pos.y = pt.y;
			pointer.transform.position = pos;

			// Now lerp to the position
			pointer.transform.position = Vector3.Lerp (pointer.transform.position, pt,
				Time.smoothDeltaTime * speed);
		}

		// move towards the pointer
		// Only move if we're far away.
		float dist = Vector3.Distance (pointer.transform.position, snakeInstance.transform.position) - 0.05f;
		if (dist < 0) {
			dist = 0;
		}
		Rigidbody rb =
			snakeInstance.GetComponent<Rigidbody> ();
	
		rb.transform.LookAt (pointer.transform.position);
		rb.velocity = 1f * snakeInstance.transform.localScale.x * snakeInstance.transform.forward * dist / .01f;
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
		snakeInstance = (Instantiate (snakeHeadPrefab, pos, Quaternion.identity, transform) as GameObject).GetComponent<Rigidbody> ();

		slitherComponent = GetComponent<Slithering> ();
		// Pass the head to the slithering component to make movement work.
		slitherComponent.Head = snakeInstance.transform;

		// After instantiating a new snake instance, add the FoodConsumer component.
		snakeInstance.gameObject.AddComponent<FoodConsumer> ();
	}

	public int GetLength ()
	{
		return slitherComponent == null ? 0 : slitherComponent.GetLength ();
	}

}
