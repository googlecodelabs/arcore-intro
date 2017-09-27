using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for creating a snake-like segmented motion.
/// </summary>
/// <remarks>
/// The speed and distances are scale sensitive, so when changing
/// the size of the object, keep that in mind.
/// </remarks>
public class Slithering : MonoBehaviour
{

  // The body prefab, each segment is made from this prefab.
  public GameObject bodyPrefab;

  // The head of the body.
  private Transform head;
  public float speed = 15;
  public float rotationSpeed = 50;
  public float minDistance = .005f;

  private List<Transform> bodyParts = new List<Transform> ();

  public Transform Head {
    get {
      return head;
    }
    set {
      head = value;
      if (value != null) {
        ResetSize ();
        AddBodyPart (value);
        Rigidbody r = head.gameObject.GetComponent<Rigidbody> ();
        if (r != null) {
          r.velocity = Vector3.zero;
        }
      }
    }
  }

  void Update ()
  {
    Move ();
  }


  public void ResetSize ()
  {
    while (bodyParts.Count > 0) {
      Transform t = bodyParts [bodyParts.Count - 1];
      if (t != null) {
        DestroyObject (t.gameObject);
      }
      bodyParts.RemoveAt (bodyParts.Count - 1);
    }
  }

  void Move ()
  {
    Transform current;
    Transform prev;

    // For each part of the body move it towards the previous part,
    // with a springy effect so faster if it is further away.
    for (int i = 1; i < bodyParts.Count; i++) {
      current = bodyParts [i];
      prev = bodyParts [i - 1];

      float dist = Vector3.Distance (prev.position, current.position);

      // The new position is the previous position.  Keep the y value
      // the same as the head.
      Vector3 newPos = prev.position;
      newPos.y = bodyParts [0].position.y;

      // Move faster based on the distance.
      float amt = Mathf.Clamp (Time.deltaTime * dist / minDistance * speed, 0, .5f);

      // Don't move if we're really close, but always rotate to give that
      // slithery look.
      if (dist >= minDistance / 2f) {
        current.position = Vector3.Lerp (current.position, newPos, amt);
      }
      current.rotation = Quaternion.Slerp (current.rotation, prev.rotation, amt);
    }
  }

  // Adds a new body part to the end of the body.  If the newPart is null,
  // the bodyPart prefab is used to create a new part.
  public void AddBodyPart (Transform newPart = null)
  {
    Vector3 pos = transform.position;
    Quaternion rot = transform.rotation;
    if (bodyParts.Count != 0) {
      Transform lastPart = bodyParts [bodyParts.Count - 1];
      pos = lastPart.position - lastPart.forward * lastPart.localScale.x;
      rot = lastPart.rotation;
    }

    if (newPart == null) {
      newPart = (Instantiate (bodyPrefab, pos, rot) as GameObject).transform;
    }
    newPart.SetParent (transform);
    bodyParts.Add (newPart);
  }

  public int GetLength ()
  {
    return bodyParts.Count;
  }
}
