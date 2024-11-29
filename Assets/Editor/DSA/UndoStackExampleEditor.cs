using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DSA
{
    [CustomEditor(typeof(UndoStackExample))]
    public class UndoStackExampleEditor : Editor
    {
        public abstract class MyOperation
        {
            public abstract void Redo(UndoStackExample use);
            public abstract void Undo(UndoStackExample use);
        }

        public class Add : MyOperation
        {
            private int m_iValue;

            public Add(int iValue)
            {
                m_iValue = iValue;
            }

            public override void Redo(UndoStackExample use)
            {
                use.m_iValue += m_iValue;
            }

            public override void Undo(UndoStackExample use)
            {
                use.m_iValue -= m_iValue;
            }
        }

        public class Multiply : MyOperation
        {
            private int m_iValue;

            public Multiply(int iValue)
            {
                m_iValue = iValue;
            }

            public override void Redo(UndoStackExample use)
            {
                use.m_iValue *= m_iValue;
            }

            public override void Undo(UndoStackExample use)
            {
                use.m_iValue /= m_iValue;
            }
        }

        Stack<MyOperation> m_undoStack = new Stack<MyOperation>();
        Stack<MyOperation> m_redoStack = new Stack<MyOperation>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UndoStackExample use = target as UndoStackExample;

            GUILayout.Space(50);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Operations", EditorStyles.boldLabel);

            // Add operations here
            if (GUILayout.Button("Add 2"))
            {
                Add add2 = new Add(2);
                add2.Redo(use);
                m_undoStack.Push(add2);
                m_redoStack.Clear();
            }

            if (GUILayout.Button("Multiply 3"))
            {
                Multiply mult3 = new Multiply(3);
                mult3.Redo(use);
                m_undoStack.Push(mult3);
                m_redoStack.Clear();
            }

            GUILayout.EndVertical();

            // undo / redo
            GUILayout.Space(20);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();

            // Undo & Redo here
            GUI.enabled = m_undoStack.Count > 0;
            if (GUILayout.Button("Undo"))
            {
                MyOperation op = m_undoStack.Pop();
                op.Undo(use);
                m_redoStack.Push(op);
            }

            GUI.enabled = m_redoStack.Count > 0;
            if (GUILayout.Button("Redo"))
            {
                MyOperation op = m_redoStack.Pop();
                op.Redo(use);
                m_undoStack.Push(op);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}