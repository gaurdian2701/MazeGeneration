using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSA
{
    public class MessageSender : MonoBehaviour
    {
        public delegate void DataHandler(string strData);

        [SerializeField, TextArea(20, 50)]
        public string   m_largeText;

        #region Events

        public event DataHandler OnData;

        #endregion

        private void Start()
        {
            StartCoroutine(SendMessageLogic());
        }

        IEnumerator SendMessageLogic()
        {
            while (m_largeText.Length > 0)
            {
                int iNumLetter = Mathf.Min(Random.Range(2, 6), m_largeText.Length);
                string strData = m_largeText.Substring(0, iNumLetter);
                m_largeText = m_largeText.Substring(iNumLetter);

                // trigger event?
                if (OnData != null)
                {
                    OnData(strData);
                }

                // simulate my polish internet
                yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
            }
        }
    }
}