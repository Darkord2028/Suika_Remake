using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteScaleManager))]
public class ScaleManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteScaleManager scaler = (SpriteScaleManager)target;

        if (GUILayout.Button("Apply Logarithmic Scaling"))
        {
            scaler.LogarithmicScale();
            EditorUtility.SetDirty(scaler);
        }
        if(GUILayout.Button("Apply Linear Scaling"))
        {
            scaler.LinearScale();
            EditorUtility.SetDirty(scaler);
        }
    }
}
