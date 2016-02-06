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

        public float FrontAngle
        {
            get
            {
                var angle   = m_armFront.transform.localRotation.eulerAngles.z;
                if( angle > 180.0f )
                {
                    angle = angle - 360.0f;
                }
                return angle;
            }
        }

        public float CenterAngle
        {
            get
            {
                var angle = m_armCenter.transform.localRotation.eulerAngles.z;
                if( angle > 180.0f )
                {
                    angle = angle - 360.0f;
                }
                return angle;
            }
        }

        public float BackAnglet
        {
            get
            {
                var angle = m_armBack.transform.localRotation.eulerAngles.z;
                if( angle > 180.0f )
                {
                    angle = angle - 360.0f;
                }
                return angle;
            }
        }

        public bool Activate
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        #endregion // Property


        #region Public

        public void SetPose( float i_front, float i_center, float i_back )
        {
//             m_armFront.transform.localRotation   = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_front ) );
//             m_armCenter.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_center ) );
            m_armBack.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_back ) );
        }

        public void InitRot()
        {
            m_armFront.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, 0.0f ) );
            m_armCenter.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, 0.0f ) );
            m_armBack.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, 0.0f ) );
        }

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
                var rot = ts.localRotation.eulerAngles;
                rot    += new Vector3( 0.0f, 0.0f, i_power );
                if( rot.z > 180.0f )
                {
                    rot.z   = rot.z - 360.0f;
                }
                if( rot.z > 45.0f )
                {
                    rot.z = 45.0f;
                }
                if( rot.z < -45.0f )
                {
                    rot.z = -45.0f;
                }

                ts.localRotation    = Quaternion.Euler( rot );
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

