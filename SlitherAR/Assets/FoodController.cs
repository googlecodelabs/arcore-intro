using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class FoodController : MonoBehaviour
{


  private TrackedPlane trackedPlane;

  private Bounds spawnBounds = new Bounds ();

  // Collection of food models.
  public GameObject[] foodModels;
  private GameObject foodInstance;
  private float foodAge;
  private readonly float maxAge = 10f;

  // Update is called once per frame
  void Update ()
  {
    // Check for a plane
    if (trackedPlane == null) {
      return;
    }

    if (!trackedPlane.IsValid) {
      return;
    }

    if (trackedPlane.IsUpdated) {
      spawnBounds.center = trackedPlane.Position;
      // Subtract a small amount to match the plane rendering and mesh so we
      // avoid the edge.
      spawnBounds.extents = new Vector3 (
        trackedPlane.Bounds.x / 2f - .2f,
        0, // it is a plane.
        trackedPlane.Bounds.y / 2f - .2f);
    }

    if (foodInstance == null || foodInstance.activeSelf == false) {
      SpawnFoodInstance ();
      return;
    }

    foodAge += Time.deltaTime;
    if (foodAge >= maxAge) {
      DestroyObject (foodInstance);
      foodInstance = null;
    }
  }

  public void SetPlane (TrackedPlane plane)
  {
    trackedPlane = plane;
  }

  private void SpawnFoodInstance ()
  {
    GameObject foodItem = foodModels [Random.Range (0, foodModels.Length - 1)];

    // Pick a location - TODO would be nice to have a min distance away
    // from the previous distance.
    float x = Random.Range (spawnBounds.min.x, spawnBounds.max.x);
    float z = Random.Range (spawnBounds.min.z, spawnBounds.max.z);

    // Move the object up .05 to arbitrarily be above the plane mesh.
    Vector3 position = new Vector3 (x, trackedPlane.Position.y + .05f, z);
    //TODO: use the collider on the plane and make sure it overlaps.

    Anchor anchor = Session.CreateAnchor (position, Quaternion.identity);

    foodInstance = Instantiate (foodItem, position, Quaternion.identity, anchor.transform);

    // used to detect colliding with food.
    foodInstance.tag = "food";

    //TODO: scale it smartly. .25 is a guess.
    foodInstance.transform.localScale = new Vector3 (.25f, .25f, .25f);
    foodAge = 0;
 
    foodInstance.AddComponent<PlaneAttachment> ().Attach (trackedPlane);
    foodInstance.AddComponent<FoodMotion> ();
  }
}
