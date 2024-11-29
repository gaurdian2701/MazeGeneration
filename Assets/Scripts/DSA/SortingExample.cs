using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace DSA
{
    [ExecuteInEditMode]
    public class SortingExample : MonoBehaviour
    {
        public class WaitForKey : CustomYieldInstruction
        {
            static float m_fLastKeyPress = -1.0f;

            public override bool keepWaiting
            {
                get
                {
                    if (Time.unscaledTime - m_fLastKeyPress > 0.3f && (Input.GetKeyDown(KeyCode.Space) || false))
                    {
                        m_fLastKeyPress = Time.unscaledTime;
                        return false;
                    }

                    return true;
                }
            }
        }

        public int[] m_array;

        #region Properties

        public int Left { get; private set; } = -1;

        public int Right { get; private set; } = -1;

        public Vector2Int CurrentRange { get; private set; }

        public int PivotValue { get; private set; }

        private int PivotIndex { get; set; }

        #endregion

        private void Start()
        {
            // create a randomized int array
            m_array = new int[32];
            for (int i = 0; i < m_array.Length; i++)
            {
                m_array[i] = Random.Range(0, 64);
            }
            //m_array = new int[] { 3, 4, 5, 5, 2, 2, 3, 3 };

            Left = -1;
            Right = -1;
            CurrentRange = new Vector2Int(0, m_array.Length - 1);

            QuickSort(m_array, 0, m_array.Length - 1);
            //StartCoroutine(CR_QuickSort(m_array, 0, m_array.Length - 1));
        }

        #region Regular Version

        public static int Partition(int[] array, int iStart, int iEnd)
        {
            // select the pivot value
            int iPivotValue = array[iStart];  //array[(iStart + iEnd) / 2];
            int iLeft = iStart;
            int iRight = iEnd;

            while (iLeft <= iRight)
            {
                // move left index until a value greater or equal to the pivot is found
                while (array[iLeft] < iPivotValue)
                {
                    iLeft++;
                }

                // move right index until a value less or equal to the pivot is found
                while (array[iRight] > iPivotValue)
                {
                    iRight--;
                }

                // should we swap?
                if (iLeft <= iRight)
                {
                    // ... otherwise swap
                    int iTemp = array[iLeft];
                    array[iLeft] = array[iRight];
                    array[iRight] = iTemp;

                    iLeft++;
                    iRight--;
                }
            }

            return iLeft;
        }

        public static void QuickSort(int[] array, int iStart, int iEnd)
        {
            if (iStart < iEnd)
            {
                // Partition the array
                int iPivotIndex = Partition(array, iStart, iEnd);

                // Send left side off to QuickSort
                QuickSort(array, iStart, iPivotIndex - 1);

                // Send right side off to QuickSort
                QuickSort(array, iPivotIndex, iEnd);        // <-- iPivotIndex + 1 :(
            }
        }

        #endregion

        #region Coroutine Version

        IEnumerator CR_Partition(int[] array, int iStart, int iEnd)
        {
            // select the pivot value
            PivotValue = array[iStart];  //array[(iStart + iEnd) / 2];
            Left = iStart;
            Right = iEnd;
            CurrentRange = new Vector2Int(iStart, iEnd);

            while (Left <= Right)
            {
                // move left index until a value greater or equal to the pivot is found
                while (array[Left] < PivotValue)
                {
                    yield return new WaitForKey();
                    Left++;
                }

                // move right index until a value less or equal to the pivot is found
                while (array[Right] > PivotValue)
                {
                    yield return new WaitForKey();
                    Right--;
                }

                // should we swap?
                if (Left <= Right)
                {
                    // ... otherwise swap
                    yield return new WaitForKey();
                    int iTemp = array[Left];
                    array[Left] = array[Right];
                    array[Right] = iTemp;

                    Left++;
                    Right--;
                }
            }

            yield return new WaitForKey();
            PivotIndex = Left;
        }

        IEnumerator CR_QuickSort(int[] array, int iStart, int iEnd)
        {
            if (iStart < iEnd)
            {
                // Partition the array
                yield return CR_Partition(array, iStart, iEnd);

                // Send left side off to QuickSort
                yield return CR_QuickSort(array, iStart, PivotIndex - 1);

                // Send right side off to QuickSort
                yield return CR_QuickSort(array, PivotIndex, iEnd);
            }

            Left = -1;
            Right = -1;
            CurrentRange = new Vector2Int(iStart, iEnd);
        }

        #endregion
    }
}