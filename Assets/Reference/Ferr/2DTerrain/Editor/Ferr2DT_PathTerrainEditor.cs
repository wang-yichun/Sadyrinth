using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Ferr2DT_PathTerrain))]
public class Ferr2DT_PathTerrainEditor : Editor {
    bool showVisuals     = true;
    bool showTerrainType = true;
    bool showCollider    = true;

    void OnSceneGUI()
    {
        Ferr2DT_PathTerrain collider = (Ferr2DT_PathTerrain)target;
        Ferr2D_Path         path     = collider.gameObject.GetComponent<Ferr2D_Path>();

        EditorUtility.SetSelectedWireframeHidden(collider.gameObject.GetComponent<Renderer>(), Ferr_Menu.HideMeshes);

        if (collider.enabled == false || path == null || path.pathVerts.Count <= 1 || !collider.createCollider) return;
        
        Handles.color = new Color(0, 1, 0, 1);

        List<Vector2> verts = path.GetVerts(collider.smoothPath, collider.splitDist, collider.splitCorners);
        if (path.closed && collider.smoothPath == false) verts.Add(verts[0]);

        Vector2 norm      = Ferr2D_Path.GetNormal(verts, 0, path.closed);
        Vector2 tmp       = Vector2.zero;
        Vector2 tmpBottom = Vector2.zero;
        if (collider.fill == Ferr2DT_FillMode.None)
        {
            tmp       = verts[0] + new Vector2(norm.x * -collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ], norm.y * -collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top]);
            tmpBottom = verts[0] + new Vector2(norm.x *  collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom], norm.y *  collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
        }
        else
        {
            Vector2 offset = collider.GetOffset(path.pathVerts,0, path.closed);
            tmp = verts[0] + new Vector2(norm.x * -offset.x, norm.y * -offset.y);
        }
        Vector3 startPos   = path.gameObject.transform.position + Ferr2D_PathEditor.offset + Vector3.Scale(new Vector3(tmp      .x, tmp      .y, 0), path.transform.localScale);
        Vector3 currBottom = path.gameObject.transform.position + Ferr2D_PathEditor.offset + Vector3.Scale(new Vector3(tmpBottom.x, tmpBottom.y, 0), path.transform.localScale);
        Vector3 curr       = startPos;

        if (collider.fill == Ferr2DT_FillMode.None) Handles.DrawLine(curr, currBottom);
        for (int i = 0; i < verts.Count - 1; i++)
        {
            norm = Ferr2D_Path.GetNormal(verts, i + 1, path.closed);
            if (collider.fill == Ferr2DT_FillMode.None)
            {
                tmp       = verts[i + 1] + new Vector2(norm.x * -collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ], norm.y * -collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]);
                tmpBottom = verts[i + 1] + new Vector2(norm.x *  collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom], norm.y *  collider.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
                Vector3 pos       = path.gameObject.transform.position + Ferr2D_PathEditor.offset + Vector3.Scale(new Vector3(tmp      .x, tmp      .y, 0), path.transform.localScale);
                Vector3 posBottom = path.gameObject.transform.position + Ferr2D_PathEditor.offset + Vector3.Scale(new Vector3(tmpBottom.x, tmpBottom.y, 0), path.transform.localScale);
                
                if (ColliderNormValid(collider, verts[i  ], verts[i+1])) Handles.DrawLine(curr,       pos      );
                if (ColliderNormValid(collider, verts[i+1], verts[i  ])) Handles.DrawLine(currBottom, posBottom);
                curr       = pos;
                currBottom = posBottom;
            }
            else
            {
                Vector2 offset = collider.GetOffset(verts, i+1, path.closed);
                tmp = verts[i + 1] + new Vector2(norm.x * -offset.x, norm.y * -offset.y);
                Vector3 pos    = path.gameObject.transform.position + Ferr2D_PathEditor.offset + Vector3.Scale(new Vector3(tmp.x, tmp.y, 0), path.transform.localScale);
                if (i+1 == verts.Count-1 && path.closed) pos = startPos;
                if (ColliderNormValid(collider, verts[i], verts[i+1])) Handles.DrawLine(curr, pos);
                curr = pos;
            }
        }
        if (collider.fill == Ferr2DT_FillMode.None) Handles.DrawLine(curr, currBottom);

        if (collider.fill == Ferr2DT_FillMode.Skirt || collider.fill == Ferr2DT_FillMode.FillOnlySkirt)
        {
            Vector3 start = startPos;
            Vector3 end = curr;
            Vector3 skirtStart = new Vector3(start.x, collider.transform.position.y + collider.fillY, start.z);
            Vector3 skirtEnd = new Vector3(end.x, collider.transform.position.y + collider.fillY, end.z);
            Handles.DrawLine(start, skirtStart);
            Handles.DrawLine(skirtStart, skirtEnd);
            Handles.DrawLine(skirtEnd, end);
        }
    }
	public override void OnInspectorGUI() {
		#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		Undo.RecordObject(target, "Modified Path Terrain");
		#else
        Undo.SetSnapshotTarget(target, "Modified Path Terrain");
		#endif

        Ferr2DT_PathTerrain sprite = (Ferr2DT_PathTerrain)target;

        // render the material selector!
        EditorGUILayout.BeginHorizontal();
        GUIContent button = sprite.TerrainMaterial != null && sprite.TerrainMaterial.edgeMaterial != null && sprite.TerrainMaterial.edgeMaterial.mainTexture != null ? new GUIContent(sprite.TerrainMaterial.edgeMaterial.mainTexture) : new GUIContent("Pick");
        if (GUILayout.Button(button, GUILayout.Width(48f),GUILayout.Height(48f))) Ferr2DT_MaterialSelector.Show(sprite.SetMaterial);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField   ("Terrain Material:");
        EditorGUI      .indentLevel = 2;
        EditorGUILayout.LabelField   (sprite.TerrainMaterial == null ? "None" : sprite.TerrainMaterial.name);
        EditorGUI      .indentLevel = 0;
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.EndHorizontal();

        showVisuals = EditorGUILayout.Foldout(showVisuals, "VISUALS");

        
        if (showVisuals) {
            EditorGUI.indentLevel = 2;
            // other visual data
            sprite.vertexColor      = EditorGUILayout.ColorField("Vertex Color",      sprite.vertexColor);
            sprite.pixelsPerUnit    = EditorGUILayout.FloatField("Pixels Per Unit",   sprite.pixelsPerUnit);
			sprite.stretchThreshold = EditorGUILayout.Slider    ("Stretch Threshold", sprite.stretchThreshold, 0f, 1f);
        }
        EditorGUI.indentLevel = 0;


        showTerrainType = EditorGUILayout.Foldout(showTerrainType, "TERRAIN TYPE");
        if (showTerrainType) {
            EditorGUI.indentLevel = 2;
            sprite.fill          = (Ferr2DT_FillMode)EditorGUILayout.EnumPopup("Fill Type", sprite.fill);
		    if (sprite.fill == Ferr2DT_FillMode.Closed || sprite.fill == Ferr2DT_FillMode.InvertedClosed || sprite.fill == Ferr2DT_FillMode.FillOnlyClosed && sprite.GetComponent<Ferr2D_Path>() != null) sprite.GetComponent<Ferr2D_Path>().closed = true;
            if (sprite.fill != Ferr2DT_FillMode.None && (sprite.TerrainMaterial != null && sprite.TerrainMaterial.fillMaterial == null)) sprite.fill = Ferr2DT_FillMode.None;
            if (sprite.fill != Ferr2DT_FillMode.None ) sprite.fillZ = EditorGUILayout.FloatField("Fill Z Offset", sprite.fillZ);
            if (sprite.fill == Ferr2DT_FillMode.Skirt) sprite.fillY = EditorGUILayout.FloatField("Skirt Y Value", sprite.fillY);
            
            sprite.splitCorners  = EditorGUILayout.Toggle    ("Split Corners",   sprite.splitCorners );
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Smooth Path", GUILayout.Width(122f));
            sprite.smoothPath = EditorGUILayout.Toggle(sprite.smoothPath, GUILayout.Width(48f));
            if (sprite.smoothPath)
            {
                EditorGUILayout.LabelField("Splits", GUILayout.Width(64f));
                sprite.splitDist = EditorGUILayout.FloatField(sprite.splitDist, GUILayout.Width(64f));
            }
            EditorGUILayout.EndHorizontal();
            if (sprite.smoothPath) EditorGUILayout.HelpBox("Edge overrides do not currently work with smoothed terrain! Look for this in a coming update =D", MessageType.Info);
        }
        EditorGUI.indentLevel = 0;
        

        showCollider = EditorGUILayout.Foldout(showCollider, "COLLIDER");
        // render collider options
        if (showCollider) {
            EditorGUI.indentLevel = 2;
            sprite.createCollider = EditorGUILayout.Toggle("Create Collider", sprite.createCollider);
            if (sprite.createCollider) {
				#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
				sprite.create3DCollider = EditorGUILayout.Toggle("Use 3D Collider", sprite.create3DCollider);
				
				if (sprite.create3DCollider) {
					sprite.physicsMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial,typeof(PhysicMaterial), true);
					sprite.depth           = EditorGUILayout.FloatField("Collider Width", sprite.depth);
				} else {
					sprite.physicsMaterial2D = (PhysicsMaterial2D)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial2D,typeof(PhysicsMaterial2D), true);
				}
				#else
				sprite.physicsMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial,typeof(PhysicMaterial), true);
				sprite.depth           = EditorGUILayout.FloatField("Collider Width", sprite.depth);
				#endif
                if (sprite.fill == Ferr2DT_FillMode.None)
                {
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ] = EditorGUILayout.FloatField("Thickness Top",    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom] = EditorGUILayout.FloatField("Thickness Bottom", sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
                }
                else
                {
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ] = EditorGUILayout.FloatField("Offset Top",    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Left  ] = EditorGUILayout.FloatField("Offset Left",   sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Left  ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Right ] = EditorGUILayout.FloatField("Offset Right",  sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Right ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom] = EditorGUILayout.FloatField("Offset Bottom", sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
                }

                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField("Generate colliders along:");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Top",GUILayout.Width(25));
                sprite.collidersTop = EditorGUILayout.Toggle(sprite.collidersTop,GUILayout.Width(25));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Left",GUILayout.Width(25));
                sprite.collidersLeft = EditorGUILayout.Toggle(sprite.collidersLeft,GUILayout.Width(25));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Right",GUILayout.Width(35));
                sprite.collidersRight = EditorGUILayout.Toggle(sprite.collidersRight,GUILayout.Width(35));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Bottom",GUILayout.Width(45));
                sprite.collidersBottom = EditorGUILayout.Toggle(sprite.collidersBottom,GUILayout.Width(45));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

				#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
				if (!sprite.collidersBottom || !sprite.collidersLeft || !sprite.collidersRight || !sprite.collidersTop) {
					EditorGUI.indentLevel = 2;
					sprite.colliderThickness = EditorGUILayout.FloatField("Collider Thickness", sprite.colliderThickness);
					EditorGUI.indentLevel = 0;
				}
				#endif
            }
        }
        EditorGUI.indentLevel = 0;

		if (GUI.changed) {
            EditorUtility.SetDirty(target);
			sprite.RecreatePath();
		}
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    sprite.ForceMaterial(sprite.TerrainMaterial, true);
                    sprite.RecreatePath();
                    break;
            }
        }
	}

    bool ColliderNormValid(Ferr2DT_PathTerrain aSprite, Vector2 aOne, Vector2 aTwo) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aTwo, aOne);
        if (dir == Ferr2DT_TerrainDirection.Top    && aSprite.collidersTop   ) return true;
        if (dir == Ferr2DT_TerrainDirection.Left   && aSprite.collidersLeft  ) return true;
        if (dir == Ferr2DT_TerrainDirection.Right  && aSprite.collidersRight ) return true;
        if (dir == Ferr2DT_TerrainDirection.Bottom && aSprite.collidersBottom) return true;
        return false;
    }
}