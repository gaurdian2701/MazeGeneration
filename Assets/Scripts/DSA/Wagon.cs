using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSA
{
    public class Wagon : MonoBehaviour
    {
        private Wagon   m_nextWagon;
        private Wagon   m_prevWagon;

        public static List<Wagon> AllWagons = new List<Wagon>();

        #region Properties

        public virtual bool HasTrain
        {
            get
            {
                Wagon w = m_nextWagon;
                while (w != null)
                {
                    if (w is Train)
                    {
                        return true;
                    }

                    w = w.m_nextWagon;
                }

                return false;
            }
        }

        public Wagon LastWagon => m_prevWagon == null ? this : m_prevWagon.LastWagon;

        #endregion

        private void OnEnable()
        {
            AllWagons.Add(this);
        }

        private void OnDisable()
        {
            AllWagons.Remove(this);
        }

        protected virtual void Update()
        {
            if (m_nextWagon == null)
            {
                // find next wagon
                Wagon snapWagon = AllWagons.Find(w => w.HasTrain && Vector3.Distance(transform.position, w.transform.position) < 1.0f);
                if(snapWagon != null)
                {
                    m_nextWagon = snapWagon.LastWagon;
                    m_nextWagon.m_prevWagon = this;
                }
            }
            else
            {
                // move to be behind the next wagon
                Vector3 vToNext = m_nextWagon.transform.position - transform.position;
                transform.position = m_nextWagon.transform.position - vToNext.normalized * 1.5f;
            }
        }
    }
}