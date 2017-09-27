using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FoodController : MonoBehaviour {
  private TrackedPlane trackedPlane;
  private Bounds spawnBounds = new Bounds ();
  private GameObject foodInstance;
  private float foodAge;
  private readonly float maxAge = 10f;
  public GameObject[] foodModels;


  public void SetSelectedPlane(TrackedPlane selectedPlane)
  {
    trackedPlane = selectedPlane;
  }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
    if (trackedPlane == null) {
      return;
    }

    if (!trackedPlane.IsValid) {
      return;
    }

    if (trackedPlane.IsUpdated) {
      spawnBounds.center = trackedPlane.Position;
      // subtract a small amount to match the plane rendering and mesh so we
      // avoid the edge.
      spawnBounds.extents = new Vector3(
        trackedPlane.Bounds.x/2f - .2f,
        0, // it is a plane.
        trackedPlane.Bounds.y/2f - .2f);
    }

    if (foodInstance == null || foodInstance.activeSelf == false) {
      SpawnFoodInstance();
      return;
    }

    foodAge += Time.deltaTime;
    if (foodAge >= maxAge) {
      DestroyObject(foodInstance);
      foodInstance = null;
    }
  }

  private void SpawnFoodInstance() {
    GameObject foodItem = foodModels[Random.Range(0,foodModels.Length-1)];

    // Pick a location - TODO would be nice to have a min distance away
    // from the previous distance.

    float x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
    float z = Random.Range(spawnBounds.min.z, spawnBounds.max.z);
    Vector3 position = new Vector3(x, trackedPlane.Position.y+.05f, z);

    Anchor anchor = Session.CreateAnchor(position, Quaternion.identity);

    foodInstance = Instantiate(foodItem,position, Quaternion.identity,anchor.transform);

    // Set the tag
    foodInstance.tag = "food";

     foodInstance.transform.localScale = new Vector3(.01f,.01f,.01f);
    foodAge = 0;

    foodInstance.AddComponent<PlaneAttachment>();
    foodInstance.GetComponent<PlaneAttachment>().Attach(trackedPlane  );
    foodInstance.AddComponent<FoodMotion>();
  }
}
