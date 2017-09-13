using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMotion : MonoBehaviour
{
  float speed = 30;

  void Update ()
  {
    transform.Rotate (Vector3.down, Time.deltaTime * speed * 5);
  }
}
