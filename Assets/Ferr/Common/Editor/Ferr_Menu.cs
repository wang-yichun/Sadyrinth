using UnityEngine;
using UnityEditor;
using System.Collections;

public static class Ferr_Menu {
    static bool  prefsLoaded = false;
    static bool  hideMeshes  = true;
    static float pathScale   = 1;

    public static bool HideMeshes {
        get{LoadPrefs();return hideMeshes;}
    }
    public static float PathScale {
        get{LoadPrefs();return pathScale;}
    }

    [PreferenceItem("Ferr")]
    static void Ferr2DT_PreferencesGUI() 
    {
        LoadPrefs();

        hideMeshes = EditorGUILayout.Toggle    ("Hide terrain meshes", hideMeshes);
        pathScale  = EditorGUILayout.FloatField("Path vertex scale",   pathScale );

        if (GUI.changed) {
            SavePrefs();
        }
    }

    static void LoadPrefs() {
        if (prefsLoaded) return;
        prefsLoaded = true;
        hideMeshes  = EditorPrefs.GetBool ("Ferr_hideMeshes", true);
        pathScale   = EditorPrefs.GetFloat("Ferr_pathScale",  1   );
    }
    static void SavePrefs() {
        if (!prefsLoaded) return;
        EditorPrefs.SetBool ("Ferr_hideMeshes", hideMeshes);
        EditorPrefs.SetFloat("Ferr_pathScale",  pathScale );
    }
}
