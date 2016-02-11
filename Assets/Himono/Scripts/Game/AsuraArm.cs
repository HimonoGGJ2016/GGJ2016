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


        private List< int > m_activePlayerList  = new List< int >();
        private Dictionary< int, Color >            m_activeColorList           = new Dictionary< int, Color >();
        private Dictionary< SpriteRenderer, int >   m_spriteDefaultOrderList    = new Dictionary<SpriteRenderer, int>();


        private bool m_activate = false;
        private SpriteRenderer[]    m_spriteList    = null;

        private const int   ACTIVE_SORT_ORDER   = 50;

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

        public Vector3 Angles
        {
            get
            {
                var angles  = Vector3.zero;
                angles.x    = BackAnglet;
                angles.y    = CenterAngle;
                angles.z    = FrontAngle;
                return angles;
            }
        }

        public Vector3 LimitAngles
        {
            get;
            set;
        }

        private float FrontAngle
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

        private float CenterAngle
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

        private float BackAnglet
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
            if( i_active )
            {
                if( m_activePlayerList.Exists( value => value == i_localPlayerID ) )
                {
                    return;
                }

                StopColorAnimation();

                m_activePlayerList.Insert( 0, i_localPlayerID );
                StartColorAnimation();

                if( m_activePlayerList.Count == 1 )
                {
                    foreach( var pair in m_spriteDefaultOrderList )
                    {
                        pair.Key.sortingOrder   += ACTIVE_SORT_ORDER;
                    }
                }
            }
            else
            {
                m_activePlayerList.RemoveAll( value => value == i_localPlayerID );
                StopColorAnimation();

                if( m_activePlayerList.Count > 0 )
                {
                    StartColorAnimation();
                }

                if( m_activePlayerList.Count == 0 )
                {
                    foreach( var pair in m_spriteDefaultOrderList )
                    {
                        pair.Key.sortingOrder = pair.Value;
                    }
                }
            }
        }




        public void SetPose( Vector3 i_angles )
        {
            SetPose( i_angles.x, i_angles.y, i_angles.z );
        }
        public void SetPose( float i_back, float i_center, float i_front )
        {
            m_armFront.transform.localRotation  = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_front ) );
            m_armCenter.transform.localRotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_center ) );
            m_armBack.transform.localRotation   = Quaternion.Euler( new Vector3( 0.0f, 0.0f, i_back ) );
        }

        public void InitRot()
        {
            m_armFront.transform.localRotation  = Quaternion.identity;
            m_armCenter.transform.localRotation = Quaternion.identity;
            m_armBack.transform.localRotation   = Quaternion.identity;
        }

        public void RotateArm( int i_handID, float i_power )
        {
            Transform   ts      = null;
            float       limit   = 0.0f;
            switch( i_handID )
            {
                case 0:
                    ts      = m_armBack.transform;
                    limit   = LimitAngles.x;
                    break;

                case 1:
                    ts      = m_armCenter.transform;
                    limit   = LimitAngles.y;
                    break;

                case 2:
                    ts      = m_armFront.transform;
                    limit   = LimitAngles.z;
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
                if( rot.z > limit )
                {
                    rot.z = limit;
                }
                if( rot.z < -limit )
                {
                    rot.z = -limit;
                }

                ts.localRotation    = Quaternion.Euler( rot );
            }

        }

        public void SetActiveColors( Color[] i_colors )
        {
            int playerIndex = 1;
            foreach( var color in i_colors )
            {
                m_activeColorList.Add( playerIndex++, color );
            }
        }

    #endregion // Public


    #region UnityEvent

        void Awake()
        {
            m_spriteList    = GetComponentsInChildren< SpriteRenderer >( true );
            foreach( var sprite in m_spriteList )
            {
                m_spriteDefaultOrderList.Add( sprite, sprite.sortingOrder );
            }
        }

    #endregion // UnityEvent


    #region Private

        private void StartColorAnimation()
        {
            var color = GetColorFromRegisteredList( 0 );
            StartCoroutine( ActivateColorState( color, 0, ActivateColorTime ) );
        }

        private void StopColorAnimation()
        {
            StopAllCoroutines();
            SetArmColor( Color.white );
        }

        private void SetArmColor( Color i_color )
        {
            foreach( var sprite in m_spriteList )
            {
                sprite.color    = i_color;
            }
        }

        private IEnumerator ActivateColorState( Color i_color, int i_index, float i_time )
        {
            var nextColor   = GetColorFromRegisteredList( i_index + 1 );
            if( i_color == nextColor )
            {
                nextColor   = Color.white;
            }

            float curTime   = i_time;

            while( curTime > 0.0f )
            {
                float step      = 1.0f - ( curTime / i_time );
                var rate        = ActiveColorCurve.Evaluate( step );
                var curColor    = Color.Lerp( i_color, nextColor, rate );

                SetArmColor( curColor );
                yield return null;
                curTime     -= Time.deltaTime;
            }

            i_index = GetNextPlayerIndex( i_index );
            StartCoroutine( ActivateColorState( nextColor, i_index, i_time ) );
        }

        private Color GetColorFromRegisteredList( int i_index )
        {
            if( m_activePlayerList == null || m_activePlayerList.Count == 0 )
            {
                return Color.white;
            }

            i_index = i_index % m_activePlayerList.Count;
            int id  = m_activePlayerList[ i_index ];
            
            var color   = Color.white;
            bool ret = m_activeColorList.TryGetValue( id, out color );

            return ret ? color : Color.white;
        }

        private int GetNextPlayerIndex( int i_index )
        {
            return m_activePlayerList.Count > 0 ? ( i_index + 1 ) % m_activePlayerList.Count : 0;
        }

    #endregion // Private


    } // class AsuraArm
    
} // namespace HimonoLib

