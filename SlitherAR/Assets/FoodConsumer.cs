using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodConsumer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "food") {
      collision.gameObject.SetActive(false);
      Slithering s = GetComponentInParent<Slithering>();

      if (s != null) {
        s.AddBodyPart();
      } 
    }
  }
}
