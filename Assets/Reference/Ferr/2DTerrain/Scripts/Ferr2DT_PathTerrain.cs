using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A generic IComparer, primarily for sorting mesh segments by z value, to avoid overlap and Z issues.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Ferr2DT_Comparer<T> : IComparer<T> {
    private readonly Func<T, T, int> func;
    public Ferr2DT_Comparer(Func<T, T, int> comparerFunc) {
        this.func = comparerFunc;
    }

    public int Compare(T x, T y) {
        return this.func(x, y);
    }
}

/// <summary>
/// Describes how the terrain path should be filled.
/// </summary>
public enum Ferr2DT_FillMode
{
    /// <summary>
    /// The interior of the path will be filled, and edges will be treated like a polygon.
    /// </summary>
    Closed,
    /// <summary>
    /// Drops some extra vertices down, and fill the interior. Edges only around the path itself.
    /// </summary>
    Skirt,
    /// <summary>
    /// Doesn't fill the interior at all. Just edges.
    /// </summary>
    None,
    /// <summary>
    /// Fills the outside of the path rather than the interior, also inverts the edges, upside-down.
    /// </summary>
    InvertedClosed,
    /// <summary>
    /// Just like Closed, but with no edges
    /// </summary>
    FillOnlyClosed,
    /// <summary>
    /// Just like Skirt, but with no edges
    /// </summary>
    FillOnlySkirt
}

[AddComponentMenu("Ferr2DT/Path Terrain"), RequireComponent (typeof (Ferr2D_Path)), RequireComponent (typeof (MeshFilter)), RequireComponent (typeof (MeshRenderer))]
public class Ferr2DT_PathTerrain : MonoBehaviour, Ferr2D_IPath {
	
	#region Public fields
	public Ferr2DT_FillMode fill    = Ferr2DT_FillMode.Closed;
    /// <summary>
    /// If fill is set to Skirt, this value represents the Y value of where the skirt will end.
    /// </summary>
	public float    fillY           = 0;
    /// <summary>
    /// In order to combat Z-Fighting, this allows you to set a Z-Offset on the fill.
    /// </summary>
	public float    fillZ           = 0.2f;
    /// <summary>
    /// This will separate edges at corners, for applying different material parts to different slopes,
    /// as well as creating sharp corners on smoothed paths.
    /// </summary>
	public bool     splitCorners    = true;
    /// <summary>
    /// Makes the path curvy. It's not a perfect algorithm just yet, but it does make things curvier.
    /// </summary>
    public bool     smoothPath      = false;
    /// <summary>
    /// On smoothed surfaces, the distance between each split on the curve (Unity units)
    /// </summary>
    public float    splitDist       = 4;
    /// <summary>
    /// Roughly how many pixels we try to fit into one unit of Unity space
    /// </summary>
	public float    pixelsPerUnit   = 32;
    /// <summary>
    /// The color for every vertex! If you use the right shader (like the Ferr2D shaders) this will influence
    /// the color of the terrain. This is faster, because you don't need additional materials for new colors!
    /// </summary>
    public Color    vertexColor     = Color.white;
	/// <summary>
	/// When a segment is too small to fit 3 texture pieces, but too large to fit 2 texture peices, should it go
	/// with 2, or 3? Use this value to influence the decision! 0-1, 0 being larger (fewer) pieces, 1 being smaller (more) pieces.
	/// </summary>
	public float    stretchThreshold = 0.5f;

    /// <summary>
    /// Should we generate a collider on Start?
    /// </summary>
    public bool     createCollider  = true;
	/// <summary>
	/// By default in Unity versions >= 4.3, Ferr2DT will create a 2D Poly Collider
	/// Use this to force a 3D mesh collider instead
	/// </summary>
	public bool     create3DCollider = false;
    /// <summary>
    /// How wide should the collider be on the Z axis? (Unity units)
    /// </summary>
    public float    depth           = 4.0f;
    /// <summary>
    /// For offseting the collider, so it can line up with stuff better visually. On fill = None terrain,
    /// this behaves significantly different than regular closed terrain.
    /// </summary>
    public float[]  surfaceOffset   = new float[] {0,0,0,0};
    /// <summary>
    /// When the collider is created (or Recreated), this will be assigned as its material!
    /// </summary>
    public PhysicMaterial physicsMaterial = null;

#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
	/// <summary>
	/// When the collider is created (or Recreated), this will be assigned as its material!
	/// </summary>
	public PhysicsMaterial2D physicsMaterial2D = null;
#endif

    public bool collidersLeft   = true;
    public bool collidersRight  = true;
    public bool collidersTop    = true;
    public bool collidersBottom = true;
	public float colliderThickness = 0.1f;
    
    /// <summary>
    /// This property will call SetMaterial when set.
    /// </summary>
    public Ferr2DT_TerrainMaterial TerrainMaterial { get { return terrainMaterial; } set { SetMaterial(value); } }

    [SerializeField()]
    public List<Ferr2DT_TerrainDirection> directionOverrides;
	#endregion
	
	#region Private fields
    [SerializeField()]
    Ferr2DT_TerrainMaterial terrainMaterial;
	Ferr2D_Path             path;
	Ferr2DT_DynamicMesh     dMesh;
	Vector2                 unitsPerUV = Vector2.one;
	#endregion

    #region MonoBehaviour Methods
    void Start() {
        if (createCollider) {
            RecreateCollider();
        }
        for (int i = 0; i < Camera.allCameras.Length; i++) {
            Camera.allCameras[i].transparencySortMode = TransparencySortMode.Orthographic;
        }
    }
    #endregion

    #region Creation methods
	/// <summary>
	/// Creates a TerrainMaterial from a JSON string. Does NOT recreate mesh data automatically or link materials!
	/// </summary>
	/// <param name="aJSON">A JSON string containing TerrainPath and Path data</param>
	public void           FromJSON(string aJSON) {
		FromJSON(Ferr_JSON.Parse(aJSON));
	}
	/// <summary>
	/// Creates a TerrainMaterial from a JSON object. Does NOT recreate mesh data automatically or link materials!
	/// </summary>
	/// <param name="aJSON">A parsed JSON value containing PathTerrain and Path data</param>
	public void           FromJSON(Ferr_JSONValue aJSON) {
		fill             = (Ferr2DT_FillMode)Enum.Parse(typeof(Ferr2DT_FillMode), aJSON["fill", "Closed"]);
		fillY            = aJSON["fillY",            0];
		fillZ            = aJSON["fillZ",            0.2f];
		splitCorners     = aJSON["splitCorners",     true];
		smoothPath       = aJSON["smoothPath",       false];
		splitDist        = aJSON["splitDist",        4];
		pixelsPerUnit    = aJSON["pixelsPerUnit",    32];
		stretchThreshold = aJSON["stretchThreshold", 0.5f];
		vertexColor      = Ferr_Color.FromHex(aJSON["vertexColor", "FFFFFF"]);
		createCollider   = aJSON["createCollider",   true];
		create3DCollider = aJSON["create3DCollider", false];
		depth            = aJSON["depth",            4];
		surfaceOffset[0] = aJSON["surfaceOffset.0",  0];
		surfaceOffset[1] = aJSON["surfaceOffset.1",  0];
		surfaceOffset[2] = aJSON["surfaceOffset.2",  0];
		surfaceOffset[3] = aJSON["surfaceOffset.3",  0];
		collidersBottom  = aJSON["colliders.bottom", true];
		collidersTop     = aJSON["colliders.top",    true];
		collidersLeft    = aJSON["colliders.left",   true];
		collidersRight   = aJSON["colliders.right",  true];
		
		Ferr_JSONValue overrides = aJSON["directionOverrides"];
		for (int i = 0; i < overrides.Length; i++) { 
			directionOverrides.Add ( (Ferr2DT_TerrainDirection)Enum.Parse(typeof(Ferr2DT_TerrainDirection), overrides[i,"None"]) );
		}
		
		path.FromJSON (aJSON["path"]);
	}
	/// <summary>
	/// Creates a JSON object containing PathTerrain and Path data.
	/// </summary>
	/// <returns>A JSON Value containing PathTerrain and Path data. You can ToString this for the JSON string.</returns>
	public Ferr_JSONValue ToJSON  () {
		Ferr_JSONValue result = new Ferr_JSONValue();
		result["fill"            ] = ""+fill;
		result["fillY"           ] = fillY;
		result["fillZ"           ] = fillZ;
		result["splitCorners"    ] = splitCorners;
		result["smoothPath"      ] = smoothPath;
		result["splitDist"       ] = splitDist;
		result["pixelsPerUnit"   ] = pixelsPerUnit;
		result["stretchThreshold"] = stretchThreshold;
		result["vertexColor"     ] = Ferr_Color.ToHex(vertexColor);
		result["createCollider"  ] = createCollider;
		result["create3DCollider"] = create3DCollider;
		result["depth"           ] = depth;
		result["surfaceOffset.0" ] = surfaceOffset[0];
		result["surfaceOffset.1" ] = surfaceOffset[1];
		result["surfaceOffset.2" ] = surfaceOffset[2];
		result["surfaceOffset.3" ] = surfaceOffset[3];
		result["colliders.bottom"] = collidersBottom;
		result["colliders.top"   ] = collidersTop;
		result["colliders.left"  ] = collidersLeft;
		result["colliders.right" ] = collidersRight;
		
		result["directionOverrides"] = 0;
		Ferr_JSONValue dir = result["directionOverrides"];
		for (int i = 0; i < directionOverrides.Count; i++) {
			dir[i] = directionOverrides[i].ToString ();
		}
		result["path"] = path.ToJSON();
		return result;
	}

    /// <summary>
    /// The Ferr2DT_IPath method, gets called automatically whenever the Ferr2DT path gets updated in the 
    /// editor. This will completely recreate the the visual mesh (only) for the terrain. If you want
    /// To recreate the collider as well, that's a separate call to RecreateCollider.
    /// </summary>
    public  void RecreatePath    () {
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();

		if (path			== null) path			 = GetComponent<Ferr2D_Path> ();
		if (dMesh			== null) dMesh			 = new Ferr2DT_DynamicMesh   ();
        if (terrainMaterial == null) {
            Debug.LogWarning("Cannot create terrain without a Terrain Material!");
            return;
        }
		
		MatchOverrides();

        // double check the materials!
        ForceMaterial(terrainMaterial, true, false);
		
		dMesh.Clear ();
        dMesh.color = vertexColor;
		
		if (path.Count < 2) {
			GetComponent<MeshFilter>().sharedMesh = null;
			return;
		}
		
		// make sure we can keep a consistent scale for the texture
		if (terrainMaterial.edgeMaterial.mainTexture != null && terrainMaterial.edgeMaterial.mainTexture != null) {
			unitsPerUV.x = terrainMaterial.edgeMaterial.mainTexture.width  / pixelsPerUnit;
			unitsPerUV.y = terrainMaterial.edgeMaterial.mainTexture.height / pixelsPerUnit;
		}
		
        if (fill != Ferr2DT_FillMode.FillOnlyClosed && fill != Ferr2DT_FillMode.FillOnlySkirt) {
		    // split the path into segments based on the split angle
		    List<List<Vector2>>            segments = new List<List<Vector2>>           ();
            List<Ferr2DT_TerrainDirection> dirs     = new List<Ferr2DT_TerrainDirection>();
            
            segments = GetSegments(path.GetVerts(smoothPath,splitDist,splitCorners), out dirs);
            if (dirs.Count < segments.Count) dirs.Add( directionOverrides[directionOverrides.Count-1]);
            List<int> order = new List<int>();
            for (int i = 0; i < segments.Count; i++) order.Add(i);

            order.Sort(
                new Ferr2DT_Comparer<int>(
                    (x, y) => GetDescription(segments[y]).zOffset.CompareTo(GetDescription(segments[x]).zOffset)
                ));
		
		    // process the segments into meshes
		    for (int i = 0; i < order.Count; i++) {
			    AddSegment (segments[order[i]], order.Count <= 1 && path.closed, dirs[order[i]]);
		    }
        }
		int[] submesh1 = dMesh.GetCurrentTriangleList();
		
		// add a fill if the user desires
        if ((fill == Ferr2DT_FillMode.Skirt || fill == Ferr2DT_FillMode.FillOnlySkirt) && terrainMaterial.fillMaterial != null)
        {
			AddFill(true);
        }
        else if ((fill == Ferr2DT_FillMode.Closed || fill == Ferr2DT_FillMode.InvertedClosed || fill == Ferr2DT_FillMode.FillOnlyClosed) && terrainMaterial.fillMaterial != null)
        {
            AddFill(false);
        }
        else if (fill == Ferr2DT_FillMode.None) { }
		int[] submesh2 = dMesh.GetCurrentTriangleList(submesh1.Length);
		
		// compile the mesh!
		Mesh   m    = GetComponent<MeshFilter>().sharedMesh;
		string name = "Ferr2DT_PathMesh_" + gameObject.name + gameObject.GetInstanceID();
		if (m == null || m.name != name) {
			GetComponent<MeshFilter>().sharedMesh = m = new Mesh();
			m.name = name;
		}
		dMesh.Build(ref m);
		
		// set up submeshes and submaterials
		m.subMeshCount=2;
		if (GetComponent<Renderer>().sharedMaterials.Length < 2) {
			Material[] old = GetComponent<Renderer>().sharedMaterials;
			GetComponent<Renderer>().sharedMaterials = new Material[2];
			if (old.Length > 0) GetComponent<Renderer>().sharedMaterials[0] = old[0];
		}
		m.SetTriangles(submesh1,1);
		m.SetTriangles(submesh2,0);
        //sw.Stop();
        //Debug.Log("Creating mesh took: " + sw.Elapsed.TotalMilliseconds + "ms");
	}

	/// <summary>
	/// Creates a mesh or poly and adds it to the collider object. This is automatically calld on Start,
	/// if createCollider is set to true. This will automatically add a collider if none is 
	/// attached already.
	/// </summary>
	public  void RecreateCollider() {
#if UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0
		RecreateCollider3D();
#else
		if (create3DCollider) {
			RecreateCollider3D();
		} else {
			RecreateCollider2D();
		}
#endif
	}

    private void                RecreateCollider3D()
    {
		Ferr2DT_DynamicMesh dMesh = new Ferr2DT_DynamicMesh();
        List<Vector2>       verts = GetColliderVerts();

        // create the solid mesh for it
        for (int i = 0; i < verts.Count; i++) {
            if (path.closed && i == verts.Count-1) dMesh.AddVertex(verts[0]);
            else dMesh.AddVertex(verts[i]);
        }
        dMesh.ExtrudeZ(depth, fill == Ferr2DT_FillMode.InvertedClosed);

        // remove any faces the user may not want
        if (!collidersTop   ) dMesh.RemoveFaces(new Vector3( 0, 1,0), 45);
        if (!collidersLeft  ) dMesh.RemoveFaces(new Vector3(-1, 0,0), 45);
        if (!collidersRight ) dMesh.RemoveFaces(new Vector3( 1, 0,0), 45);
        if (!collidersBottom) dMesh.RemoveFaces(new Vector3( 0,-1,0), 45);

        // make sure there's a MeshCollider component on this object
        if (GetComponent<MeshCollider>() == null) {
            gameObject.AddComponent<MeshCollider>();
        }
        if (physicsMaterial != null) GetComponent<MeshCollider>().sharedMaterial = physicsMaterial;

        // compile the mesh!
        Mesh m = GetComponent<MeshCollider>().sharedMesh;
        string name = "Ferr2DT_PathCollider_" + gameObject.name + gameObject.GetInstanceID();
        if (m == null || m.name != name) {
            GetComponent<MeshCollider>().sharedMesh = m = new Mesh();
            m.name = name;
        }
        GetComponent<MeshCollider>().sharedMesh = null;
        dMesh.Build(ref m);
        GetComponent<MeshCollider>().sharedMesh = m;
    }
	private void                RecreateCollider2D() {
#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		List<Vector2> verts = GetColliderVerts ();

		// make sure there's a collider component on this object
		if (GetComponent<PolygonCollider2D>() == null) {
			gameObject.AddComponent<PolygonCollider2D>();
		}
		if (physicsMaterial != null) GetComponent<PolygonCollider2D>().sharedMaterial = physicsMaterial2D;

		List<List<Vector2>> segs = RemoveDirections2D(verts);
		PolygonCollider2D   poly = GetComponent<PolygonCollider2D>();
		poly.pathCount = segs.Count;
		if (segs.Count > 1) {
			for (int i = 0; i < segs.Count; i++) {
				poly.SetPath (i, ExpandColliderPath(segs[i], colliderThickness).ToArray());
			}
		} else {
			if (fill == Ferr2DT_FillMode.InvertedClosed) {
				Rect bounds = Ferr2D_Path.GetBounds(segs[0]);
				poly.pathCount = 2;
				poly.SetPath (0, segs[0].ToArray());
				poly.SetPath (1, new Vector2[]{
					new Vector2(bounds.xMin-bounds.width, bounds.yMax+bounds.height),
					new Vector2(bounds.xMax+bounds.width, bounds.yMax+bounds.height),
					new Vector2(bounds.xMax+bounds.width, bounds.yMin-bounds.height),
					new Vector2(bounds.xMin-bounds.width, bounds.yMin-bounds.height)
				});
			} else {
				poly.SetPath (0, segs[0].ToArray());
			}
		}
#else
		Debug.LogWarning("Ferr2DTerrain cannot create a 2D collider in Unity versions < 4.3! No collider has been generated!");
#endif
	}
	private List<List<Vector2>> RemoveDirections2D(List<Vector2> aList) {
		List<List<Vector2>> segments = new List<List<Vector2>>();

		List<Vector2> curr = new List<Vector2>();
		for (int i = 0; i < aList.Count; i++) {
			Vector2 normal = aList[i== aList.Count-1?i-1:i+1] - aList[i];
			normal = new Vector2(-normal.y, normal.x);

			if ((!collidersTop    && Vector2.Angle(normal,  Vector2.up   ) < 45f) ||
			    (!collidersLeft   && Vector2.Angle(normal, -Vector2.right) < 45f) ||
			    (!collidersRight  && Vector2.Angle(normal,  Vector2.right) < 45f) ||
			    (!collidersBottom && Vector2.Angle(normal, -Vector2.up   ) < 45f)) {
				if (curr.Count > 0) {
					curr.Add (aList[i]);
					segments.Add (curr);
					curr = new List<Vector2>();
				}
			} else {
				curr.Add (aList[i]);
			}
		}
		if (curr.Count > 0) {
			segments.Add (curr);
		}
		return segments;
	}
	private List<Vector2>       ExpandColliderPath(List<Vector2> aList, float aAmount) {
		int count = aList.Count;
		for (int i = count - 1; i >= 0; i--) {
			Vector2 norm = Ferr2D_Path.GetNormal(aList, i, false);
			aList.Add (aList [i] + new Vector2 (norm.x * aAmount, norm.y * aAmount));
		}
		return aList;
	}
	private List<Vector2>       GetColliderVerts  () {
		Ferr2D_Path   path  = GetComponent<Ferr2D_Path> ();
		List<Vector2> verts = path.GetVerts (smoothPath, splitDist, splitCorners);
		if (path.closed && smoothPath == false) verts.Add (verts [0]);
		List<Vector2> raw   = new List<Vector2> (verts);

		MatchOverrides ();

		// consider the offset values for the collider. Also, if it's not filled, make sure we add in 
		// extra path verts for the underside of the path
		int count = raw.Count;
		for (int i = count - 1; i >= 0; i--) {
			Vector2 norm = Ferr2D_Path.GetNormal (raw, i, path.closed);
			if (fill == Ferr2DT_FillMode.None) {
				verts.Add (raw [i] + new Vector2 (norm.x * surfaceOffset [(int)Ferr2DT_TerrainDirection.Top], norm.y * surfaceOffset [(int)Ferr2DT_TerrainDirection.Top]));
				verts [i] += new Vector2 (norm.x * -surfaceOffset [(int)Ferr2DT_TerrainDirection.Bottom], norm.y * -surfaceOffset [(int)Ferr2DT_TerrainDirection.Bottom]);
			}
			else {
				Vector2 offset = GetOffset (raw, i, path.closed);
				verts [i] += new Vector2 (norm.x * -offset.x, norm.y * -offset.y);
			}
		}

		if (fill == Ferr2DT_FillMode.Skirt || fill == Ferr2DT_FillMode.FillOnlySkirt) {
			Vector2 start = verts [0];
			Vector2 end = verts [verts.Count - 1];
			verts.Add (new Vector2 (end.x,                              fillY));
			verts.Add (new Vector2 (Mathf.Lerp (end.x, start.x, 0.33f), fillY));
			verts.Add (new Vector2 (Mathf.Lerp (end.x, start.x, 0.66f), fillY));
			verts.Add (new Vector2 (start.x,                            fillY));
		}
		else if (fill == Ferr2DT_FillMode.None) {
		}
		return verts;
	}
	#endregion
	
	#region Mesh manipulation methods
	private void AddSegment(List<Vector2> aSegment, bool aClosed, Ferr2DT_TerrainDirection aDir = Ferr2DT_TerrainDirection.None) {
		Ferr2DT_SegmentDescription	desc 		= GetDescription(aSegment);
        if (aDir != Ferr2DT_TerrainDirection.None) { desc = terrainMaterial.GetDescriptor(aDir);}
        int                         bodyID      = UnityEngine.Random.Range(0, desc.body.Length);
		Rect                        body        = terrainMaterial.ToUV( desc.body[bodyID] );
		float						bodyWidth	= body.width * unitsPerUV.x;

        int tSeed = UnityEngine.Random.seed;

        Vector2 capLeftSlideDir  = (aSegment[1               ] - aSegment[0               ]);
        Vector2 capRightSlideDir = (aSegment[aSegment.Count-2] - aSegment[aSegment.Count-1]);
        capLeftSlideDir .Normalize();
        capRightSlideDir.Normalize();
        aSegment[0               ] -= capLeftSlideDir  * desc.capOffset;
        aSegment[aSegment.Count-1] -= capRightSlideDir * desc.capOffset;

		for (int i = 0; i < aSegment.Count-1; i++) {
			Vector2 norm1   = Vector2.zero;
			Vector2 norm2   = Vector2.zero;
			float   length  = Vector2.Distance(aSegment[i+1],aSegment[i]);
			int     repeats = Mathf.Max(1,Mathf.FloorToInt(length / bodyWidth + stretchThreshold));
			
			norm1 = Ferr2D_Path.GetNormal(aSegment, i,   aClosed);
			norm2 = Ferr2D_Path.GetNormal(aSegment, i+1, aClosed);
			
			for (int t = 1; t < repeats+1; t++) {
                UnityEngine.Random.seed = (int)(transform.position.x * 100000 + transform.position.y * 10000 + i * 100 + t);
                body                    = terrainMaterial.ToUV(desc.body[UnityEngine.Random.Range(0, desc.body.Length)]);
                Vector2 pos1, pos2, n1, n2;

                pos1 = Vector2.Lerp(aSegment[i], aSegment[i + 1], (float)(t - 1) / repeats);
                pos2 = Vector2.Lerp(aSegment[i], aSegment[i + 1], (float) t      / repeats);
                n1   = Vector2.Lerp(norm1,       norm2,           (float)(t - 1) / repeats);
                n2   = Vector2.Lerp(norm1,       norm2,           (float) t      / repeats);

                float d    = (body.height / 2) * unitsPerUV.y;
                float yOff = fill == Ferr2DT_FillMode.InvertedClosed ? -desc.yOffset : desc.yOffset;
                int   v1 = dMesh.AddVertex(pos1.x + n1.x * (d + yOff), pos1.y + n1.y * (d + yOff), desc.zOffset, body.x,    fill == Ferr2DT_FillMode.InvertedClosed ? body.yMax : body.y   );
                int   v2 = dMesh.AddVertex(pos1.x - n1.x * (d - yOff), pos1.y - n1.y * (d - yOff), desc.zOffset, body.x,    fill == Ferr2DT_FillMode.InvertedClosed ? body.y    : body.yMax);
                int   v3 = dMesh.AddVertex(pos2.x + n2.x * (d + yOff), pos2.y + n2.y * (d + yOff), desc.zOffset, body.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? body.yMax : body.y   );
                int   v4 = dMesh.AddVertex(pos2.x - n2.x * (d - yOff), pos2.y - n2.y * (d - yOff), desc.zOffset, body.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? body.y    : body.yMax);
				dMesh.AddFace(v1, v3, v4, v2);
			}
		}
        if (!aClosed) {
            AddCap(aSegment, desc, -1);
            AddCap(aSegment, desc, 1);
        }
        UnityEngine.Random.seed = tSeed;
	}
	private void AddCap    (List<Vector2> aSegment, Ferr2DT_SegmentDescription aDesc, float aDir) {
		int     index = 0;
		Vector2 dir   = Vector2.zero;
		if (aDir < 0) {
			index = 0;
			dir   = aSegment[0] - aSegment[1];
		} else {
			index = aSegment.Count-1;
			dir   = aSegment[aSegment.Count-1] - aSegment[aSegment.Count-2];
		}
		dir.Normalize();
		Vector2 norm = Ferr2D_Path.GetNormal(aSegment, index, false);
		Vector2 pos  = aSegment[index];
        Rect    lCap = fill == Ferr2DT_FillMode.InvertedClosed ? terrainMaterial.ToUV(aDesc.rightCap) : terrainMaterial.ToUV(aDesc.leftCap);
        Rect    rCap = fill == Ferr2DT_FillMode.InvertedClosed ? terrainMaterial.ToUV(aDesc.leftCap ) : terrainMaterial.ToUV(aDesc.rightCap);
        float   yOff = fill == Ferr2DT_FillMode.InvertedClosed ? -aDesc.yOffset : aDesc.yOffset;

		if (aDir < 0) {
			float width =  lCap.width     * unitsPerUV.x;
			float scale = (lCap.height/2) * unitsPerUV.y;

            int v1 = dMesh.AddVertex(pos + dir * width + norm * (scale + yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed? lCap.xMax : lCap.x, fill == Ferr2DT_FillMode.InvertedClosed ? lCap.yMax : lCap.y));
            int v2 = dMesh.AddVertex(pos + norm * (scale + yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? lCap.x : lCap.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? lCap.yMax : lCap.y));

            int v3 = dMesh.AddVertex(pos - norm * (scale - yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? lCap.x : lCap.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? lCap.y : lCap.yMax));
            int v4 = dMesh.AddVertex(pos + dir * width - norm * (scale - yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? lCap.xMax : lCap.x, fill == Ferr2DT_FillMode.InvertedClosed ? lCap.y : lCap.yMax));
			dMesh.AddFace(v1, v2, v3, v4);
		} else {
			float width =  rCap.width     * unitsPerUV.x;
			float scale = (rCap.height/2) * unitsPerUV.y;

            int v1 = dMesh.AddVertex(pos + dir * width + norm * (scale + yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? rCap.x : rCap.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? rCap.yMax : rCap.y));
            int v2 = dMesh.AddVertex(pos + norm * (scale + yOff),               aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? rCap.xMax : rCap.x, fill == Ferr2DT_FillMode.InvertedClosed ? rCap.yMax : rCap.y));

            int v3 = dMesh.AddVertex(pos - norm * (scale - yOff),               aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? rCap.xMax : rCap.x, fill == Ferr2DT_FillMode.InvertedClosed ? rCap.y : rCap.yMax));
            int v4 = dMesh.AddVertex(pos + dir * width - norm * (scale - yOff), aDesc.zOffset, new Vector2(fill == Ferr2DT_FillMode.InvertedClosed ? rCap.x : rCap.xMax, fill == Ferr2DT_FillMode.InvertedClosed ? rCap.y : rCap.yMax));
			dMesh.AddFace(v4, v3, v2, v1);
		}
	}
	private void AddFill   (bool aSkirt) {
        List<Vector2> fillVerts = new List<Vector2>(path.GetVerts(smoothPath,splitDist,splitCorners));
        Vector2       scale     = Vector2.one;

        // scale is different for the fill texture
        if (terrainMaterial.fillMaterial != null && terrainMaterial.fillMaterial.mainTexture != null) {
            scale = new Vector2(
            terrainMaterial.fillMaterial.mainTexture.width / pixelsPerUnit,
            terrainMaterial.fillMaterial.mainTexture.height / pixelsPerUnit);
        }

        if (aSkirt) {
            Vector2 start = fillVerts[0];
            Vector2 end   = fillVerts[fillVerts.Count - 1];

            fillVerts.Add(new Vector2(end.x, fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.33f), fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.66f), fillY));
            fillVerts.Add(new Vector2(start.x, fillY));
        }

        int       offset  = dMesh.VertCount;
        List<int> indices = Ferr2DT_Triangulator.GetIndices(ref fillVerts, true, fill == Ferr2DT_FillMode.InvertedClosed);
        for (int i = 0; i < fillVerts.Count; i++) {
            dMesh.AddVertex(fillVerts[i].x, fillVerts[i].y, fillZ, fillVerts[i].x / scale.x, fillVerts[i].y / scale.y);
        }
        for (int i = 0; i < indices.Count; i+=3) {
            dMesh.AddFace(indices[i  ] + offset,
                          indices[i+1] + offset,
                          indices[i+2] + offset);
        }
	}
	#endregion
	
	#region Supporting methods
    /// <summary>
    /// Sets the material of the mesh. Calls ForceMaterial with an aForceUpdate of false.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material prefab.</param>
    public  void                        SetMaterial     (Ferr2DT_TerrainMaterial aMaterial) { 
        ForceMaterial(aMaterial, false); 
    }
    /// <summary>
    /// This will allow you to set the terrain material regardless of whether it's marked as the current material already or not. Also calls RecreatePath when finished.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material prefab.</param>
    /// <param name="aForceUpdate">Force it to set the material, even if it's already the set material, or no?</param>
    /// <param name="aRecreate">Should we recreate the mesh? Usually, this is what you want (only happens if the material changes, or is forced to change)</param>
    public  void                        ForceMaterial   (Ferr2DT_TerrainMaterial aMaterial, bool aForceUpdate, bool aRecreate = true)
    {
        if (terrainMaterial != aMaterial || aForceUpdate)
        {
            terrainMaterial = aMaterial;

            // copy the materials into the renderer
            Material[] newMaterials = new Material[] {
                aMaterial.fillMaterial,
                aMaterial.edgeMaterial
            };
            GetComponent<Renderer>().sharedMaterials = newMaterials;

            if (aRecreate) {
                RecreatePath();
            }
        }
    }
    /// <summary>
    /// Adds a terrain vertex at the specified index, or at the end if the index is -1. Returns the index of the added vert.
    /// </summary>
    /// <param name="aPt">The terrain point to add, z is always 0</param>
    /// <param name="aAtIndex">The index to put the point at, or -1 to put at the end</param>
    /// <returns>Index of the point</returns>
    public  int                         AddPoint        (Vector2 aPt, int aAtIndex = -1) {
        if (path               == null) path               = GetComponent<Ferr2D_Path>();
        if (directionOverrides == null) directionOverrides = new List<Ferr2DT_TerrainDirection>();
        if (aAtIndex == -1) {
            path              .Add(aPt                          );
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
            return path.pathVerts.Count;
        } else {
            path.pathVerts    .Insert(aAtIndex, aPt                          );
            directionOverrides.Insert(aAtIndex, Ferr2DT_TerrainDirection.None);
            return aAtIndex;
        }
    }
	/// <summary>
	/// Inserts a point into the path, automatically determining insert index using Ferr2DT_Path.GetClosestSeg
	/// </summary>
	/// <returns>The index of the point that was just added.</returns>
	/// <param name="aPt">A 2D point to add to the path.</param>
	public int                          AddAutoPoint    (Vector2 aPt) {
		if (path == null) path = GetComponent<Ferr2D_Path>();
		int at = path.GetClosestSeg(aPt);
		return AddPoint(aPt, at+1 == path.pathVerts.Count ? -1 : at+1 );
	}

    private Ferr2DT_SegmentDescription	GetDescription	(List<Vector2> aSegment) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aSegment, 0, fill == Ferr2DT_FillMode.InvertedClosed);
        return terrainMaterial.GetDescriptor(dir);
	}
    private List<List<Vector2>>         GetSegments     (List<Vector2> aPath, out List<Ferr2DT_TerrainDirection> aSegDirections)
    {
        List<List<Vector2>> segments = new List<List<Vector2>>();
        if (splitCorners) {
            segments = Ferr2D_Path.GetSegments(aPath, out aSegDirections, directionOverrides,
                fill == Ferr2DT_FillMode.InvertedClosed,
                GetComponent<Ferr2D_Path>().closed);
        } else {
            aSegDirections = new List<Ferr2DT_TerrainDirection>();
            aSegDirections.Add(Ferr2DT_TerrainDirection.Top);
            segments.Add(aPath);
        }
        if (path.closed && smoothPath == false) {
            Ferr2D_Path.CloseEnds(ref segments, splitCorners);
        }
        return segments;
    }

    public  Vector2                     GetOffset       (List<Vector2> aSegment, int i, bool aClosed) {
        Ferr2DT_TerrainDirection prev = Ferr2D_Path.GetDirection(aSegment, i-1, fill == Ferr2DT_FillMode.InvertedClosed, aClosed);
        Ferr2DT_TerrainDirection next = Ferr2D_Path.GetDirection(aSegment, i,   fill == Ferr2DT_FillMode.InvertedClosed, aClosed);
        int prevID = (int)prev;
        int nextID = (int)next;

        Vector2 result = new Vector2(0,0);
        if (prev == Ferr2DT_TerrainDirection.Left || prev == Ferr2DT_TerrainDirection.Right) {
            result.x += surfaceOffset[prevID];
        } else if (next == Ferr2DT_TerrainDirection.Left || next == Ferr2DT_TerrainDirection.Right) {
            result.x += surfaceOffset[nextID];
        }

        if (prev == Ferr2DT_TerrainDirection.Top || prev == Ferr2DT_TerrainDirection.Bottom) {
            result.y += surfaceOffset[prevID];
        } else if (next == Ferr2DT_TerrainDirection.Top || next == Ferr2DT_TerrainDirection.Bottom) {
            result.y += surfaceOffset[nextID];
        }

        return result;
    }
    public  void                        MatchOverrides  () {
        if (directionOverrides == null) directionOverrides = new List<Ferr2DT_TerrainDirection>();
        if (path == null) path = GetComponent<Ferr2D_Path>();

        for (int i = directionOverrides.Count; i < path.pathVerts.Count; i++) {
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
        }
        if (directionOverrides.Count > path.pathVerts.Count) {
            int diff = directionOverrides.Count - path.pathVerts.Count;
            directionOverrides.RemoveRange(directionOverrides.Count - diff - 1, diff);
        }
    }
	#endregion
}