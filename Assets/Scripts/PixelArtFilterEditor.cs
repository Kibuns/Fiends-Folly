using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PixelArtFilter))]
public class PixelArtFilterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PixelArtFilter pixelArtFilter = (PixelArtFilter)target;

        if (GUILayout.Button("Apply Pixelation"))
        {
            //pixelArtFilter.ApplyPixelationInEditor();
        }
    }
}