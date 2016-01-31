//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace HimonoLib
{
    public class GameScene : SceneBase
    {
    
    #region Variable

        private List< AsuraArm >    m_armList   = new List<AsuraArm>();

        private AsuraArm    m_selectR   = null;
        private AsuraArm    m_selectL   = null;
        private int         m_selectArmR    = 0;
        private int         m_selectArmL    = 0;

    #endregion // Variable


    #region Property

        private string HandPointLeftTag
        {
            get
            {
                return GameSettingManager.Table.m_handPointLTag;
            }

        }

        private string HandPointRightTag
        {
            get
            {
                return GameSettingManager.Table.m_handPointRTag;
            }

        }

    #endregion // Property

    
    #region Public

    #endregion // Public

    
    #region UnityEvent

        protected override void AwakeImpl()
        {
            NetworkManager.Instance.OnChangeArm  += OnChangePower;
            NetworkManager.Instance.OnSetPose   += OnSetPose;
            NetworkManager.Instance.OnInitPose += OnInitPose;
            NetworkManager.Instance.OnStartGame += OnStartGame;

            

            InstantiateHands( HandPointLeftTag,     0,      GameSettingManager.Instance.GetArm( EArmType.LeftA ) );
            InstantiateHands( HandPointRightTag,    1000,   GameSettingManager.Instance.GetArm( EArmType.RightA ) );

            NetworkManager.Instance.ActivateUI  = false;
        }

        protected override void StartImpl()
        {
            m_selectL   = m_armList.Find( value => value.ID == 0 );
            m_selectR   = m_armList.Find( value => value.ID == 1000 );

            if( NetworkManager.Instance.IsMasterClient )
            {
                StartCoroutine( PoseState() );
            }

        }

        protected override void UpdateImpl()
        {
            


//             NetworkManager.Instance.SendArmPower( 1, hPowerL );
//             NetworkManager.Instance.SendArmPower( 2, vPowerL );
//             NetworkManager.Instance.SendArmPower( 4, hPowerR );
//             NetworkManager.Instance.SendArmPower( 5, vPowerR );
        }

    #endregion // UnityEvent


    #region Private

        private void InstantiateHands( string i_tag, int i_firstID, AsuraArm i_res )
        {
            {
                var list = GameObject.FindGameObjectsWithTag( i_tag );
                var sorted  = list.OrderBy( value => value.transform.position.y ).ToArray();
                for( int i = 0, size = sorted.Length; i < size; ++i )
                {
                    var point   = sorted[ i ].transform;
                    var obj     = GameObject.Instantiate( i_res ) as AsuraArm;
                    obj.transform.SetParent( point, false );
                    obj.ID      = i_firstID + i;
                    m_armList.Add( obj );
                }
            }
        }

        private void UpdateControl()
        {
            if( m_selectL != null )
            {
                int vPower  = Mathf.RoundToInt( Input.GetAxis( "Vertical" ) * 10.0f );
                if( Mathf.Abs( vPower ) < 2 )
                {
                    vPower = 0;
                }

                NetworkManager.Instance.SendArmPower( m_selectL.ID, m_selectArmL, vPower );


//                 int hPowerL = Mathf.RoundToInt( Input.GetAxis( "Horizontal" ) * 10.0f );
//                 if( hPowerL < 2 )
//                 {
//                     hPowerL = 0;
//                 }

                if( Input.GetButtonDown( "SelectL" )/* || Input.GetButtonDown( "" )*/ )
                {
                    m_selectL = NextArm( m_selectL, 0, 1 );
                }

                if( Input.GetButtonDown( "SelectL2" ) )
                {
                    m_selectArmL++;
                    m_selectArmL    %= 3;
                }
            }

            if( m_selectR != null )
            {
                int vPower  = Mathf.RoundToInt( Input.GetAxis( "VerticalDPad" ) * 10.0f );
                if( Mathf.Abs( vPower ) < 2 )
                {
                    vPower  = 0;
                }

                NetworkManager.Instance.SendArmPower( m_selectR.ID, m_selectArmR, vPower );


//                 int hPowerL = Mathf.RoundToInt( Input.GetAxis( "HorizontalDPad" ) * 10.0f );
//                 if( hPowerL < 2 )
//                 {
//                     hPowerL = 0;
//                 }

                if( Input.GetButtonDown( "SelectR" )/* || Input.GetButtonDown( "" )*/ )
                {
                    m_selectR = NextArm( m_selectR, 1000, 1 );
                }

                if( Input.GetButtonDown( "SelectR2" ) )
                {
                    m_selectArmR++;
                    m_selectArmR %= 3;
                }
            }
        }

        private AsuraArm NextArm( AsuraArm i_arm, int i_default, int i_add )
        {
            if( i_arm == null )
            {
                return m_armList.Find( value => value.ID == i_default );
            }

            int nextID  = i_arm.ID + i_add;
            if( nextID >= i_default + 10 )
            {
                nextID  = i_default;
            }
            else if( nextID < i_default )
            {
                nextID  = i_default + 10 - 1;
            }

            return m_armList.Find( value => value.ID == nextID );
        }


        private void OnChangePower( int i_armID, int i_handID, int i_power )
        {
            var arm = m_armList.FirstOrDefault( value => value.ID == i_armID );
            if( arm != null )
            {
                arm.RotateArm( i_handID, i_power * 30.0f * Time.deltaTime );
            }
        }

        private void OnSetPose( object i_value )
        {
            var list    = ( float[] )i_value;
            for( int i = 0, j = 0; j < m_armList.Count; i+=3, ++j )
            {
                //var value = list[i];
                m_armList[ j ].SetPose( list[ i ], list[ i+1 ], list[ i+2 ] );
            }
        }

        private void OnInitPose()
        {
            foreach( var arm in m_armList )
            {
                arm.InitRot();
            }
        }

        private void OnStartGame()
        {
            StartCoroutine( GameState() );
        }

        #endregion // Private


        #region State

        private IEnumerator PoseState()
        {
            yield return new WaitForSeconds( 1.0f );


            {
                var randList = new List< float >();
                for( int i = 0; i < m_armList.Count * 30.0f; ++i )
                {
                    randList.Add( Random.Range( -45.0f, 45.0f ) );
                }

                NetworkManager.Instance.SendPose( randList.ToArray() );
            }
            

            yield return new WaitForSeconds( 5.0f );

            NetworkManager.Instance.InitPose();

            yield return new WaitForSeconds( 1.0f );

            NetworkManager.Instance.StartGame();
        }

        private IEnumerator GameState()
        {
            while( true )
            {
                UpdateControl();

                yield return null;
            }
            
        }







        #endregion // State








    } // class GameScene



} // namespace HimonoLib

