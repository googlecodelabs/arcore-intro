//-----------------------------------------------------------------------
//// <copyright file="FoodController.cs" company="Google">
/////
///// Copyright 2017 Google Inc. All Rights Reserved.
/////
///// Licensed under the Apache License, Version 2.0 (the "License");
///// you may not use this file except in compliance with the License.
///// You may obtain a copy of the License at
/////
///// http://www.apache.org/licenses/LICENSE-2.0
/////
///// Unless required by applicable law or agreed to in writing, software
///// distributed under the License is distributed on an "AS IS" BASIS,
///// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///// See the License for the specific language governing permissions and
///// limitations under the License.
/////
///// </copyright>
/////-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

/// <summary>
/// FoodController for Slither - AR codelab.
/// </summary>
/// <remarks>
/// See the codelab for the complete narrative of what
/// this class does.
/// </remarks>
public class FoodController : MonoBehaviour
{
    // Plane to spawn the food objects on.
    private TrackedPlane trackedPlane;
    // The current food instance or null.
    private GameObject foodInstance;
    // Age in seconds of the food instance.
    private float foodAge;
    // Max age of a food before destroying.
    private readonly float maxAge = 10f;
    // Array of models to use when create a food instance.
    public GameObject[] foodModels;

    // Update is called once per frame
    void Update()
    {
        if (trackedPlane == null)
        {
            return;
        }

        if (!trackedPlane.IsValid)
        {
            return;
        }

        // Check for the plane being subsumed
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (trackedPlane.SubsumedBy != null)
        {
            trackedPlane = trackedPlane.SubsumedBy;
        }

        if (foodInstance == null || foodInstance.activeSelf == false)
        {
            SpawnFoodInstance();
            return;
        }

        // Increment the age and destroy if expired.
        foodAge += Time.deltaTime;
        if (foodAge >= maxAge)
        {
            DestroyObject(foodInstance);
            foodInstance = null;
        }
    }

    /// <summary>
    /// Spawns the food instance.
    /// </summary>
    private void SpawnFoodInstance()
    {
        GameObject foodItem = foodModels[Random.Range(0, foodModels.Length)];

        // Pick a location.  This is done by selecting a vertex at random and then
        // a random point between it and the center of the plane.
        List<Vector3> vertices = new List<Vector3>();
        trackedPlane.GetBoundaryPolygon(ref vertices);
        Vector3 pt = vertices[Random.Range(0, vertices.Count)];
        float dist = Random.Range(0.05f, 1f);
        Vector3 position = Vector3.Lerp(pt, trackedPlane.Position, dist);
        // Move the object above the plane
        position.y += .05f;

        // Create an ARCore anchor for this position.
        Anchor anchor = Session.CreateAnchor(position, Quaternion.identity);

        // Create the instance.
        foodInstance = Instantiate(foodItem, position, Quaternion.identity, anchor.transform);

        // Set the tag - IMPORTANT: make sure the tag is defined in the Tag Editor.
        foodInstance.tag = "food";

        foodInstance.transform.localScale = new Vector3(.025f, .025f, .025f);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion>();
    }

    public void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        trackedPlane = selectedPlane;
    }
}
