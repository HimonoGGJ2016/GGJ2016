//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class AsuraArm : MonoBehaviour
    {
    
    #region Variable

        [SerializeField]
        private GameObject  m_armFront  = null;
        [SerializeField]
        private GameObject  m_armCenter = null;
        [SerializeField]
        private GameObject  m_armBack   = null;

        private int m_id    = 0;

    #endregion // Variable

    
    #region Property

        public int ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id    = value;
            }
        }

        #endregion // Property


        #region Public

        public void RotateArm( int i_handID, float i_power )
        {
            Transform   ts = null;
            switch( i_handID )
            {
                case 0:
                    ts  = m_armBack.transform;
                    break;

                case 1:
                    ts = m_armCenter.transform;
                    break;

                case 2:
                    ts = m_armFront.transform;
                    break;
            }

            if( ts != null )
            {
                ts.localRotation *= Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_power ) );
            }

        }

        #endregion // Public


        #region UnityEvent

        void Update()
        {

        }

    #endregion // UnityEvent

    
    #region Private


    #endregion // Private


    } // class AsuraArm
    
} // namespace HimonoLib

