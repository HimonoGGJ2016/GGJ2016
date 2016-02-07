//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        private int     m_id        = 0;
        private Color   m_nextColor = Color.white;


        private List< ActiveData >  m_activePlayerList  = new List<ActiveData>();
        private Dictionary< int, Color >    m_activeColorList   = new Dictionary< int, Color >();


        private bool m_activate = false;
        private SpriteRenderer[]    m_spriteList    = null;

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


        public Color ActiveColorR
        {
            get;
            set;
        }

        public Color ActiveColorL
        {
            get;
            set;
        }

        public Color InactiveColor
        {
            get
            {
                return Color.white;
            }
        }

        public AnimationCurve ActiveColorCurve
        {
            get;
            set;
        }

        public float ActivateColorTime
        {
            get;
            set;
        }

    #endregion // Property


    #region Public

        public void Activate( bool i_active, int i_localPlayerID )
        {
            if( !i_active )
            {
                if( m_activePlayerList.Exists( value => value.m_playerID == i_localPlayerID ) )
                {
                    return;
                }

                m_activePlayerList.Add( new ActiveData( i_localPlayerID, m_activeColorList[ i_localPlayerID ] ) );
            }
            else
            {
                m_activePlayerList.RemoveAll( value => value.m_playerID == i_localPlayerID );
            }
        }





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

        public void AddActiveColor( int index, Color i_color )
        {
            m_activeColorList.Add( index, i_color );
        }

    #endregion // Public


    #region UnityEvent

        void Awake()
        {
            m_spriteList    = GetComponentsInChildren< SpriteRenderer >( true );
            AddActiveColor( 0, InactiveColor );
        }

        void Start()
        {
            StartCoroutine( ActivateColorState() );
        }

        void Update()
        {







        }

    #endregion // UnityEvent


    #region Private

        private void SetArmColor( Color i_color )
        {
            foreach( var sprite in m_spriteList )
            {
                sprite.color    = i_color;
            }
        }

        private IEnumerator ActivateColorState()
        {
            int     activeIndex = 0;
            float   time        = 0.0f;
            Color   curColor    = Color.white;

            while( true )
            {
                if( ActiveColorCurve == null || ActivateColorTime <= 0.0f )
                {
                    yield return null;
                    continue;
                }

                if( m_activePlayerList.Count == 0 )
                {
                    activeIndex = 0;
                    time        = 0.0f;
                    SetArmColor( InactiveColor );
                    yield return null;
                    continue;
                }



                time       += Time.deltaTime;
                float step  = time / ActivateColorTime;
                var rate    = ActiveColorCurve.Evaluate( step );

//                 SetArmColor( Color.Lerp( curColor, nextColor, rate ) );
// 
//                 if( step > 1.0f )
//                 {
//                     time -= ActivateColorTime;
// 
//                     if( m_activePlayerList.Count > 1 )
//                     {
//                         activeIndex = ( activeIndex + 1 ) % m_activePlayerList.Count;
//                     }
// 
//                     if( activeIndex < m_activePlayerList.Count )
//                     {
//                         nextColor   = m_activeColorList[ m_activePlayerList[ activeIndex ] ];
//                     }
//                     else
//                     {
//                         nextColor   = 
//                     }
//                 }

                yield return null;
            }
        }


    #endregion // Private


    #region SubClass

        private struct ActiveData
        {
            public ActiveData( int i_id, Color i_color )
            {
                m_playerID  = i_id;
                m_color     = i_color;
            }
            public int      m_playerID;
            public Color    m_color;
        }

    #endregion // SubClass



    } // class AsuraArm
    
} // namespace HimonoLib

