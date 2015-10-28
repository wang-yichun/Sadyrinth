using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I recommend agains using this for your own mesh creation, it's a little quirky, and pretty specific
/// to the path stuff in Ferr2DTerrain. But it's a utility to make mesh creation easier for the path stuff.
/// </summary>
public class Ferr2DT_DynamicMesh
{
    #region Fields and Properties
    List<Vector3> mVerts;
	List<int    > mIndices;
	List<Vector2> mUVs;
    List<Color  > mColors;

    /// <summary>
    /// The color assigned to any new verticies.
    /// </summary>
    public Color color = Color.white;

    /// <summary>
    /// The number of vertices currently in the mesh.
    /// </summary>
    public int VertCount
    {
        get { return mVerts.Count; }
    }
    #endregion

    #region Constructor
    public Ferr2DT_DynamicMesh() {
		mVerts   = new List<Vector3>();
		mUVs     = new List<Vector2>();
		mIndices = new List<int>();
        mColors  = new List<Color>();
	}
    #endregion

    #region General Methods
    /// <summary>
    /// Clears all verices, indices, uvs, and colors from this mesh, resets color to white.
    /// </summary>
    public void  Clear                 ()
    {
        mVerts  .Clear();
        mIndices.Clear();
        mUVs    .Clear();
        mColors .Clear();
        color = Color.white;
    }
    /// <summary>
    /// Clears out the mesh, fills in the data, and recalculates normals and bounds.
    /// </summary>
    /// <param name="aMesh">An already existing mesh to fill out.</param>
    public void  Build                 (ref Mesh aMesh)
    {
        aMesh.Clear();
        aMesh.vertices  = mVerts  .ToArray();
        aMesh.uv        = mUVs    .ToArray();
        aMesh.triangles = mIndices.ToArray();
        aMesh.colors    = mColors .ToArray();
        aMesh.RecalculateNormals();
        aMesh.RecalculateBounds();
    }
    /// <summary>
    /// This extrude is pretty specific to the Ferr2DT path stuff, but it extrudes a 2D mesh out a certain
    /// distance, for use with collision meshes.
    /// </summary>
    /// <param name="aDist">How far on the Z axis to extrude.</param>
    /// <param name="aInverted">If this is the mesh of an inverted terrain, it should behave differently.</param>
    public void  ExtrudeZ              (float    aDist, bool aInverted)
    {
        int count    = mVerts.Count;
        int indCount = mIndices.Count;

        mUVs   .AddRange(mUVs   .ToArray());
        mColors.AddRange(mColors.ToArray());

        Vector3 off = new Vector3(0,0, aDist);
        for (int i = 0; i < count; i++)
        {
            mVerts[i] -= off/2;
        }
        for (int i = 0; i < count; i++)
        {
            mVerts.Add(mVerts[i] + off);
        }
        
        for (int i = 0; i < indCount; i += 3)
        {
            mIndices.Add(mIndices[i+2] + count);
            mIndices.Add(mIndices[i+1] + count);
            mIndices.Add(mIndices[i  ] + count);
        }

        int edges = count - 1;
        for (int i = 0; i < edges; i++)
        {
            if (aInverted) AddFace(i, i + count,i + count + 1, i+1);
            else           AddFace(i + 1, i + count + 1, i + count, i);
        }

        if (aInverted) AddFace(count - 1, (count - 1) + count, count, 0);
        else           AddFace(0, count, (count - 1) + count, count - 1);
    }
    /// <summary>
    /// Removes any faces that match the given normal, within the tolerance specified. No verts are deleted, just faces.
    /// </summary>
    /// <param name="aFacing">Normalized direction to delete with</param>
    /// <param name="aDegreesTolerance">Angle of tolerance, in degrees</param>
    public void RemoveFaces(Vector3 aFacing, float aDegreesTolerance) 
    {
        for (int i = 0; i < mIndices.Count; i+=3)
        {
            Vector3 norm = Vector3.Cross(mVerts[mIndices[i+1]] - mVerts[mIndices[i]], mVerts[mIndices[i+2]] - mVerts[mIndices[i]]);
            norm.Normalize();
            if (Vector3.Angle(norm, aFacing) < aDegreesTolerance) {
                mIndices.RemoveRange(i,3);
                i-=3;
            }
        }
    }
    /// <summary>
    /// Gets the current list of triangles.
    /// </summary>
    /// <param name="aStart">An offset to start from.</param>
    /// <returns></returns>
	public int[] GetCurrentTriangleList(int      aStart = 0) {
		int[] result = new int[mIndices.Count - aStart];
		int   curr   = 0;
		for (int i = aStart; i < mIndices.Count; i++) {
			result[curr] = mIndices[i];
			curr+=1;
		}
		return result;
	}
    #endregion 

    #region Vertex and Face Methods
    public int AddVertex(float aX, float aY, float aZ) {
		mVerts .Add(new Vector3(aX, aY, aZ));
		mUVs   .Add(new Vector2(0,0));
        mColors.Add(color);
		return mVerts.Count-1;
	}
    public int AddVertex(float aX, float aY, float aZ, float aU, float aV) {
		mVerts .Add(new Vector3(aX, aY, aZ));
		mUVs   .Add(new Vector2(aU,aV));
        mColors.Add(color);
		return mVerts.Count-1;
	}
	public int AddVertex(Vector3 aVert) {
		mVerts .Add(aVert);
		mUVs   .Add(new Vector2(0,0));
        mColors.Add(color);
		return mVerts.Count-1;
	}
	public int AddVertex(Vector3 aVert, Vector2 aUV) {
		mVerts .Add(aVert);
		mUVs   .Add(aUV);
        mColors.Add(color);
		return mVerts.Count-1;
	}
	public int AddVertex(Vector2 aVert, float aZ, Vector2 aUV) {
		mVerts .Add(new Vector3(aVert.x, aVert.y, aZ));
		mUVs   .Add(aUV);
        mColors.Add(color);
		return mVerts.Count-1;
	}
	
	public void AddFace(int aV1, int aV2, int aV3) {
		mIndices.Add (aV1);
		mIndices.Add (aV2);
		mIndices.Add (aV3);
	}
	public void AddFace(int aV1, int aV2, int aV3, int aV4) {
		mIndices.Add (aV3);
		mIndices.Add (aV2);
		mIndices.Add (aV1);
		
		mIndices.Add (aV4);
		mIndices.Add (aV3);
		mIndices.Add (aV1);
	}
    #endregion
}