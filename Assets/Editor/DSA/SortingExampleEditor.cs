using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DSA
{
    [CustomEditor(typeof(SortingExample))]
    public class SortingExampleEditor : Editor
    {
        Gradient m_valueColors;

        private void OnEnable()
        {
            m_valueColors = new Gradient();
            m_valueColors.colorKeys = new GradientColorKey[]{
                new GradientColorKey(Color.red, 0.0f),
                new GradientColorKey(new Color(1.0f, 0.5f, 0.0f), 0.2f),
                new GradientColorKey(Color.yellow, 0.4f),
                new GradientColorKey(Color.green, 0.6f),
                new GradientColorKey(Color.blue, 0.8f),
                new GradientColorKey(new Color(1.0f, 0.0f, 1.0f), 1.0f)
            };
        }

        private void OnSceneGUI()
        {
            Tools.current = Tool.None;
            SortingExample se = target as SortingExample;
            if (se.m_array != null)
            {
                // quick sort
                Vector2Int vRange = se.CurrentRange;
                DSAEditorUtils.DrawArray(se.m_array, (int elem, int i) => i >= vRange.x && i <= vRange.y ? m_valueColors.Evaluate(elem / 64.0f) : Color.gray, (int i) => i.ToString(), false);

                if (se.Left >= 0)
                {
                    float f1 = (se.Left + 0.5f) * 1.05f;
                    DSAEditorUtils.DrawArrow(new Vector3(f1, 2.0f, 0.0f), new Vector3(f1, 1.2f, 0.0f), 0.3f, Color.red, 4.0f);
                    DSAEditorUtils.DrawTextAt("Left", new Vector3(f1, 2.5f, 0.0f), 16.0f, Color.red, TextAnchor.MiddleCenter);
                }

                if (se.Right >= 0)
                {
                    float f2 = (se.Right + 0.5f) * 1.05f;
                    DSAEditorUtils.DrawArrow(new Vector3(f2, 2.0f, 0.0f), new Vector3(f2, 1.2f, 0.0f), 0.3f, Color.yellow, 4.0f);
                    DSAEditorUtils.DrawTextAt("Right", new Vector3(f2, 2.5f, 0.0f), 16.0f, Color.yellow, TextAnchor.MiddleCenter);
                }

                if (se.PivotValue != 0)
                {
                    DSAEditorUtils.DrawTextAt("Pivot: " + se.PivotValue, new Vector3(0.0f, -0.5f, 0.0f), 20.0f, Color.black, TextAnchor.MiddleLeft);
                }
            }
        }
    }
}