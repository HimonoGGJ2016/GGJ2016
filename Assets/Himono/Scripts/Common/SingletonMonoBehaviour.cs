//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {

    #region Variable

        private static T m_instance = null;

    #endregion // Variable


    #region Property

        public static T Instance
        {
            get
            {
                if( m_instance == null )
                {
                    m_instance = (T)FindObjectOfType( typeof( T ) );

                    if( m_instance == null )
                    {
                        Debug.LogError( typeof( T ) + "is nothing" );
                    }
                }

                return m_instance;
            }
        }

        public static bool HasInstance
        {
            get { return Instance != null; }
        }


    #endregion // Property


    #region UnityEvent

        protected void Awake()
        {
            if( CheckInstance() )
            {
                AwakeImpl();
            }
        }
        protected virtual void AwakeImpl()
        {

        }

    #endregion // UnityEvent


    #region Private

        private bool CheckInstance()
        {
            if( this == Instance )
            {
                return true;
            }
            Destroy( gameObject );
            return false;
        }

    #endregion // Private

    } // class SingletonMonoBehaviour
    
} // namespace HimonoLib

