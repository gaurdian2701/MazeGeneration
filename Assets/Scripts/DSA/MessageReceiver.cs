using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DSA
{
    public class MessageReceiver : MonoBehaviour
    {
        [SerializeField]
        public MessageSender    m_sender;

        Queue<char>             m_messageQueue = new Queue<char>();

        #region Properties

        #endregion

        private void OnEnable()
        {
            if (m_sender != null)
            {
                m_sender.OnData += HandleData;
            }
        }

        private void OnDisable()
        {
            if (m_sender != null)
            {
                m_sender.OnData -= HandleData;
            }
        }

        private void HandleData(string strData)
        {
            foreach(char c in strData) 
            {
                m_messageQueue.Enqueue(c);

                // have we received a coherent chunk of data?
                if (c == '\n' ||
                    c == ',' ||
                    c == '.')
                {
                    FlushMessage();
                }
            }
        }

        protected void FlushMessage()
        {
            Stack<char> reversalStack = new Stack<char>();

            string message = "";
            while (m_messageQueue.Count > 0)
            {
                message += m_messageQueue.Dequeue();
                //reversalStack.Push(m_messageQueue.Dequeue());
            }

            // stack reversal
            /*
            while(reversalStack.Count > 0) 
            {
                message += reversalStack.Pop();
            }*/

            GameObject template = transform.Find("MessageTemplate").gameObject;
            GameObject go = Instantiate(template, template.transform.parent);
            go.GetComponent<Text>().text = message;
            go.SetActive(true);
        }
    }
}