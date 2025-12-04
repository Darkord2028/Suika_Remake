using UnityEditor;
using UnityEngine;

public class SpriteScaleManager : MonoBehaviour
{
    [SerializeField] GameObject[] prefabsToScale;

    [Header("Logarithmic Scaling")]
    [SerializeField] float baseScale;
    [SerializeField] float scaleFactor;

    [Header("Linear Scaling")]
    [SerializeField] float minScale;
    [SerializeField] float maxScale;

    public void LogarithmicScale()
    {
        int length = prefabsToScale.Length;
        if (length == 0) return;

        for (int i = 0; i < length; i++)
        {
            float newScale = baseScale + Mathf.Log(i  + 1)* scaleFactor;
            prefabsToScale[i].transform.localScale = Vector3.one * newScale;
        }

    }

    public void LinearScale()
    {
        int length = prefabsToScale.Length;
        if (length == 0) return;

        float t = (maxScale - minScale) / length;
        for (int i = 0;i < length; i++)
        {
            float scale = Mathf.Lerp(minScale, maxScale, t * (i+1));
            prefabsToScale[i].transform.localScale = scale * Vector3.one;
        }
    }

    private void SetInitialScale()
    {
        for (int i = 0; i < prefabsToScale.Length; i++)
        {
            prefabsToScale[i].transform.localScale = Vector3.one;
        }
    }

}
