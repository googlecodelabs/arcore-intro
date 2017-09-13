using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class TrackedPlaneController : MonoBehaviour
{

  /// <summary>
  /// The ARCore tracked plane to represent.
  /// </summary>
  private TrackedPlane trackedPlane;

  private List<Vector3> meshVertices = new List<Vector3> ();

  private List<Color> meshColors = new List<Color> ();

  private List<int> meshIndices = new List<int> ();

  private Mesh mesh;

  private MeshRenderer meshRenderer;
  private MeshCollider meshCollider;


  private readonly Color[] planeColors = {
    Color.black,
    Color.blue,
    Color.cyan,
    Color.gray,
    Color.green,
    Color.magenta,
    Color.red,
    Color.white,
    Color.yellow   
  };

  /// <summary>
  /// The Unity Awake() method.
  /// </summary>
  private void Awake ()
  {
    mesh = GetComponent<MeshFilter> ().mesh;
    meshRenderer = GetComponent<MeshRenderer> ();
    meshCollider = GetComponent<MeshCollider> ();
  }

  // Update is called once per frame
  void Update ()
  {
    if (trackedPlane == null) {
      return;
    } else if (trackedPlane.SubsumedBy != null) {
      Destroy (gameObject);
      return;
    } else if (!trackedPlane.IsValid || Frame.TrackingState != FrameTrackingState.Tracking) {
      EnablePlane (false);
      return;
    }

    EnablePlane (true);

    if (trackedPlane.IsUpdated) {
      _UpdateMeshWithCurrentTrackedPlane ();
    }
  }

  private void EnablePlane (bool enabled)
  {
    meshRenderer.enabled = enabled;
    meshCollider.enabled = enabled;
  }

  /// <summary>
  /// Update the TrackedPlane reference.
  /// </summary>
  /// <param name="plane">The TrackedPlane reference..</param>
  public void SetTrackedPlane (TrackedPlane plane)
  {

// Apply a random color and grid rotation.
    GetComponent<Renderer> ().material.SetColor ("_GridColor", planeColors [Random.Range (0,
      planeColors.Length - 1)]);
    GetComponent<Renderer> ().material.SetFloat ("_UvRotation", Random.Range (0.0f, 360.0f));
    trackedPlane = plane;
    _UpdateMeshWithCurrentTrackedPlane ();
  }

  private void _UpdateMeshWithCurrentTrackedPlane ()
  {
// Note that GetBoundaryPolygon returns points in clockwise order.
    trackedPlane.GetBoundaryPolygon (ref meshVertices);

    //TODO: This might be fixed sometime?
// Remove bogus points
    int pos = 0;
    while (pos != meshVertices.Count) {
      if (meshVertices [pos].sqrMagnitude < 100) {
        pos++;
      } else {
        meshVertices.RemoveAt (pos);
      }
    }

    Vector3 planeCenter = trackedPlane.Position;
    int planePolygonCount = meshVertices.Count;

// The following code convert a polygon to a mesh with two polygons, inner
// polygon renders with 100% opacity and fade out to outter polygon with opacity 0%, as shown below.
// The indices shown in the diagram are used in comments below.
// _______________     0_______________1
// |             |      |4___________5|
// |             |      | |         | |
// |             | =>   | |         | |
// |             |      | |         | |
// |             |      |7-----------6|
// ---------------     3---------------2
    meshColors.Clear ();

// Fill transparent color to vertices 1 to 3.
    for (int i = 0; i < planePolygonCount; ++i) {
      meshColors.Add (new Color (0.0f, 0.0f, 0.0f, 0.0f));
    }

// Feather distance 0.2 meters.
    const float FEATHER_LENGTH = 0.2f;

// Feather scale over the distance between plane center and vertices.
    const float FEATHER_SCALE = 0.2f;

// Add vertex 4 to 5.
    for (int i = 0; i < planePolygonCount; ++i) {
      Vector3 v = meshVertices [i];

// Vector from plane center to current point
      Vector3 d = v - planeCenter;

      float scale = 1.0f - Mathf.Min ((FEATHER_LENGTH / d.magnitude), FEATHER_SCALE);
      meshVertices.Add (scale * d + planeCenter);

      meshColors.Add (new Color (0.0f, 0.0f, 0.0f, 1.0f));
    }

    meshIndices.Clear ();
    int verticeLength = meshVertices.Count;
    int verticeLengthHalf = verticeLength / 2;
// Generate triangle (4, 5, 6) and (4, 6, 7).
    for (int i = verticeLengthHalf + 1; i < verticeLength - 1; ++i) {
      meshIndices.Add (verticeLengthHalf);
      meshIndices.Add (i);
      meshIndices.Add (i + 1);
    }

// Generate triangle (0, 1, 4), (4, 1, 5), (5, 1, 2), (5, 2, 6), (6, 2, 3), (6, 3, 7)
// (7, 3, 0), (7, 0, 4)
    for (int i = 0; i < verticeLengthHalf; ++i) {
      meshIndices.Add (i);
      meshIndices.Add ((i + 1) % verticeLengthHalf);
      meshIndices.Add (i + verticeLengthHalf);

      meshIndices.Add (i + verticeLengthHalf);
      meshIndices.Add ((i + 1) % verticeLengthHalf);
      meshIndices.Add ((i + verticeLengthHalf + 1) % verticeLengthHalf + verticeLengthHalf);
    }

    mesh.Clear ();
    mesh.SetVertices (meshVertices);
    mesh.SetIndices (meshIndices.ToArray (), MeshTopology.Triangles, 0);
    mesh.SetColors (meshColors);

    CreateCollider ();
  }

  private void CreateCollider ()
  {
    if (meshCollider != null) {
      PhysicMaterial m = meshCollider.sharedMaterial;
      bool convex = meshCollider.convex;
      float skin = meshCollider.skinWidth;
      bool inflate = meshCollider.inflateMesh;
      DestroyImmediate (meshCollider);
      meshCollider = gameObject.AddComponent<MeshCollider> ();
      meshCollider.sharedMesh = mesh;
      meshCollider.sharedMaterial = m;
      meshCollider.convex = convex;
      meshCollider.skinWidth = skin;
      meshCollider.inflateMesh = inflate;
      meshCollider.enabled = true;
    }
  }
}
