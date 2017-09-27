using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Food motion - rotates the object around the Y axis.
/// </summary>
public class FoodMotion : MonoBehaviour
{
  float speed = 30;

  void Update ()
  {
    transform.Rotate (Vector3.down, Time.deltaTime * speed * 5);
  }
}
