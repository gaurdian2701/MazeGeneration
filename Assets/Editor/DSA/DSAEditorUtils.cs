using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DSA
{
    /// <summary>
    /// A Small helper class to help with the visualization of Data Structures & Algorithms
    /// </summary>
    public static class DSAEditorUtils
    {
        public delegate Color ColorCallback<T>(T elem, int iIndex);
        public delegate string StringCallback<T>(T elem);

        private static GUIStyle         sm_textStyle;

        #region Array Drawing

        public static void DrawArray<T>(T[] array, ColorCallback<T> cc, StringCallback<T> sc, bool bDrawIndices = true)
        {
            // iterate through the array and draw the elements
            Rect dest = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < array.Length; i++)
            {
                DrawElement(array[i], dest, 0.0f, cc(array[i], i), sc(array[i]), i.ToString(), bDrawIndices);
                dest.x += dest.width * 1.05f;
            }
        }

        #endregion

        #region Data Structures Drawing

        /*
        public static void DrawList<T>(List<T> list, ColorCallback<T> cc, StringCallback<T> sc)
        {
            // iterate through the list and draw the elements
            Rect dest = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < list.Count; i++)
            {
                DrawElement(list[i], dest, 0.0f, cc(list[i]), sc(list[i]), i.ToString());
                dest.x += dest.width * 1.05f;
            }

            // draw remaining capacity
            for (int i = list.Count; i < list.Capacity; ++i)
            {
                DrawElement(list[i], dest, 0.0f, new Color(1.0f, 1.0f, 1.0f, 0.3f), "", i.ToString());;
                dest.x += dest.width * 1.05f;
            }
        }

        public static void DrawQueue<T>(Queue<T> queue, ColorCallback<T> cc, StringCallback<T> sc)
        {
            // iterate through the queue and draw the elements
            Rect dest = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < queue.Count; i++)
            {
                DrawElement(queue[i], dest, 0.0f, cc(queue[i]), sc(queue[i]), i.ToString());
                dest.x += dest.width * 1.05f;
            }

            // draw remaining capacity
            for (int i = queue.Count; i < queue.Capacity; ++i)
            {
                DrawElement(queue[i], dest, 0.0f, new Color(1.0f, 1.0f, 1.0f, 0.3f), "", i.ToString()); ;
                dest.x += dest.width * 1.05f;
            }
        }

        public static void DrawStack<T>(Stack<T> stack, ColorCallback<T> cc, StringCallback<T> sc)
        {
            // iterate through the stack and draw the elements
            Rect dest = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < stack.Count; i++)
            {
                DrawElement(stack[i], dest, 0.0f, cc(stack[i]), sc(stack[i]), i.ToString());
                dest.x += dest.width * 1.05f;
            }

            // draw remaining capacity
            for (int i = stack.Count; i < stack.Capacity; ++i)
            {
                DrawElement(stack[i], dest, 0.0f, new Color(1.0f, 1.0f, 1.0f, 0.3f), "", i.ToString()); ;
                dest.x += dest.width * 1.05f;
            }
        }

        public static void DrawLinkedList<T>(LinkedList<T> list, ColorCallback<T> cc, StringCallback<T> sc)
        {
            // iterate through the stack and draw the elements
            Rect dest = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            int iCount = list.Count;
            for (int i = 0; i < iCount; i++)
            {                
                DrawElement(list[i], dest, 0.0f, cc(list[i]), sc(list[i]), i.ToString());

                Rect next = new Rect(dest.xMax, dest.y, dest.width * 0.4f, dest.height);
                DrawRect(next, 0.0f, new Color(1.0f, 1.0f, 1.0f, 0.5f), Color.black, 2.0f);
                if (i < iCount - 1)
                {
                    DrawArrow(next.center, (Vector3)next.center + Vector3.right * 0.75f, 0.15f, Color.red, 4.0f);
                }

                dest.x += dest.width * 2.0f;
            }
        }
        */
        #endregion

        #region Drawing Helpers

        public static void DrawText(string txt, Rect destRect, float fSize, Color color, TextAnchor anchor)
        {
            if (sm_textStyle == null)
            {
                sm_textStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                sm_textStyle.richText = true;
                sm_textStyle.alignment = TextAnchor.MiddleCenter;
                sm_textStyle.font = Resources.Load<Font>("Fonts/Candara");
                sm_textStyle.normal.textColor = Color.white;
                sm_textStyle.onNormal.textColor = Color.white;
                sm_textStyle.fontStyle = FontStyle.Normal;
            }

            sm_textStyle.alignment = anchor;
            sm_textStyle.fontSize = Mathf.RoundToInt(fSize);
            sm_textStyle.normal.textColor = color;
            sm_textStyle.hover.textColor = color;
            GUI.color = color;
            GUI.Label(destRect, txt, sm_textStyle);
        }

        public static void DrawTextAt(string txt, Vector3 vWorldPosition, float fSize, Color color, TextAnchor anchor)
        {
            const float SIZE = 600.0f;

            if (!string.IsNullOrEmpty(txt))
            {
                Vector3 vSP = HandleUtility.WorldToGUIPointWithDepth(vWorldPosition);
                if (vSP.z > 0.0f)
                {
                    Rect sr = new Rect(vSP.x, vSP.y, 0.0f, 0.0f);
                    switch (anchor)
                    {
                        case TextAnchor.UpperLeft:
                            sr = new Rect(vSP.x, vSP.y, SIZE, SIZE);
                            break;
                        case TextAnchor.UpperCenter:
                            sr = new Rect(vSP.x - SIZE * 0.5f, vSP.y, SIZE, SIZE);
                            break;
                        case TextAnchor.UpperRight:
                            sr = new Rect(vSP.x - SIZE, vSP.y, SIZE, SIZE);
                            break;
                        case TextAnchor.MiddleLeft:
                            sr = new Rect(vSP.x, vSP.y - SIZE * 0.5f, SIZE, SIZE);
                            break;
                        case TextAnchor.MiddleCenter:
                            sr = new Rect(vSP.x - SIZE * 0.5f, vSP.y - SIZE * 0.5f, SIZE, SIZE);
                            break;
                        case TextAnchor.MiddleRight:
                            sr = new Rect(vSP.x - SIZE, vSP.y - SIZE * 0.5f, SIZE, SIZE);
                            break;
                        case TextAnchor.LowerLeft:
                            sr = new Rect(vSP.x, vSP.y - SIZE, SIZE, SIZE);
                            break;
                        case TextAnchor.LowerCenter:
                            sr = new Rect(vSP.x - SIZE * 0.5f, vSP.y - SIZE, SIZE, SIZE);
                            break;
                        case TextAnchor.LowerRight:
                            sr = new Rect(vSP.x - SIZE, vSP.y - SIZE, SIZE, SIZE);
                            break;
                    }

                    Handles.BeginGUI();
                    DrawText(txt, sr, fSize, color, anchor);
                    Handles.EndGUI();
                }
            }
        }

        public static void DrawArrow(Vector3 v1, Vector3 v2, float fSize, Color color, float fThickness)
        {
            Vector3 vCamForward = SceneView.currentDrawingSceneView.camera.transform.forward;

            Handles.color = color;
            Handles.DrawLine(v1, v2, fThickness);
            Vector3 vForward = (v2 - v1).normalized;
            Vector3 vUp = Vector3.Cross(vCamForward, vForward).normalized;
            Handles.DrawLine(v2, v2 - vForward * fSize + vUp * fSize, fThickness);
            Handles.DrawLine(v2, v2 - vForward * fSize - vUp * fSize, fThickness);
        }   

        public static void DrawRect(Rect dest, float fZ, Color fill, Color border, float fBorderThickness)
        {
            Vector3[] corners = new Vector3[]
            {
                new Vector3(dest.x, dest.y, fZ),
                new Vector3(dest.x, dest.yMax, fZ),
                new Vector3(dest.xMax, dest.yMax, fZ),
                new Vector3(dest.xMax, dest.y, fZ),
            };

            // fill
            Handles.color = fill;
            Handles.DrawAAConvexPolygon(corners);

            // outline
            Handles.color = border;
            for (int i = 0; i < corners.Length; ++i)
            {
                Handles.DrawLine(corners[i], corners[(i + 1) % corners.Length], fBorderThickness);
            }
        }

        public static void DrawElement<T>(T elem, Rect dest, float fZ, Color color, string txt, string index, bool bDrawIndices = true)
        {
            // draw rect
            DrawRect(dest, fZ, color, Color.black, 2.0f);

            // draw index
            if (bDrawIndices)
            {
                Vector3 vSP = HandleUtility.WorldToGUIPointWithDepth(new Vector3(dest.center.x, dest.yMax, fZ));
                if (vSP.z > 0.0f)
                {
                    Rect sr = new Rect(vSP.x - 300.0f, vSP.y + 3.0f, 600.0f, 300.0f);
                    Handles.BeginGUI();
                    DrawText(index, sr, 12.0f, Color.white, TextAnchor.UpperCenter);
                    Handles.EndGUI();
                }
            }

            // text
            if (!string.IsNullOrEmpty(txt))
            {
                Vector3 vSP = HandleUtility.WorldToGUIPointWithDepth(new Vector3(dest.center.x, dest.center.y, fZ));
                if (vSP.z > 0.0f)
                {
                    Rect sr = new Rect(vSP.x - 300.0f, vSP.y - 300.0f, 600.0f, 600.0f);
                    Handles.BeginGUI();
                    DrawText(txt, sr, 18.0f, Color.black, TextAnchor.MiddleCenter);
                    Handles.EndGUI();
                }
            }
        }

        #endregion
    }
}