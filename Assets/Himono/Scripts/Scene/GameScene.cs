//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace HimonoLib
{
    public class GameScene : Photon.MonoBehaviour
    {
    
    #region Variable

        [SerializeField]
        private GameObject  m_timePanel = null;
        [SerializeField]
        private Text    m_timeText  = null;
        [SerializeField]
        private Animator    m_doorAnimator  = null;

        private float   m_time  = 0.0f;

        private List< AsuraArm >    m_armList   = new List<AsuraArm>();

        private AsuraArm    m_selectR   = null;
        private AsuraArm    m_selectL   = null;
        private int         m_selectArmR    = 0;
        private int         m_selectArmL    = 0;

        private const int DEFAULT_ARM_INDEX_L   = 0;
        private const int DEFAULT_ARM_INDEX_R   = 1000;

    #endregion // Variable


    #region Property

        private bool ActivateTime
        {
            set
            {
                if( m_timePanel == null )
                {
                    return;
                }

                m_timePanel.SetActive( value );
            }
        }

        private float TimeText
        {
            set
            {
                if( m_timeText == null )
                {
                    return;
                }

                value   = Mathf.Max( 0.0f, value );
                m_timeText.text = value.ToString( "F2" );
            }
        }

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

        private string OpenDoorAnimationName
        {
            get
            {
                return GameSettingManager.Table.m_coreGameData.m_openDoorAnimation;
            }
        }

        private string CloseDoorAnimationName
        {
            get
            {
                return GameSettingManager.Table.m_coreGameData.m_closeDoorAnimation;
            }
        }

    #endregion // Property


        #region Public

        #endregion // Public


    #region UnityEvent

        void Awake()
        {
            NetworkManager.Instance.OnChangeArm  += OnChangePower;
            NetworkManager.Instance.OnClearRate += OnSetClearRate;

            m_time      = GameSettingManager.Table.m_gameTime;
            ActivateTime = false;



            InstantiateHands( HandPointLeftTag,     DEFAULT_ARM_INDEX_L,    GameSettingManager.Instance.GetArm( EArmType.LeftA ) );
            InstantiateHands( HandPointRightTag,    DEFAULT_ARM_INDEX_R,    GameSettingManager.Instance.GetArm( EArmType.RightA ) );

            NetworkManager.Instance.ActivateUI  = false;
        }

        void Start()
        {
            m_selectL   = m_armList.Find( value => value.ID == DEFAULT_ARM_INDEX_L );
            m_selectR   = m_armList.Find( value => value.ID == DEFAULT_ARM_INDEX_R );

            if( NetworkManager.Instance.IsMasterClient )
            {
                StartCoroutine( WaitPlayerState() );
            }
            else
            {
                StartCoroutine( ReadyState() );
            }

        }

        void Update()
        {
            TimeText    = m_time;
        }

    #endregion // UnityEvent


    #region Network

        private void SetPose( object i_angleList )
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                SetPoseRPC( i_angleList );
                return;
            }
            photonView.RPC( "SetPoseRPC", PhotonTargets.All, i_angleList );
        }
        [PunRPC]
        private void SetPoseRPC( object i_angleList )
        {
            var list    = (float[])i_angleList;
            for( int i = 0, j = 0; j < m_armList.Count; i += 1, ++j )
            {
                //var value = list[i];
                m_armList[ j ].SetPose( list[ i ], list[ i ], list[ i ] );
            }
        }

        public void ResetPose()
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                ResetPoseRPC();
                return;
            }

            photonView.RPC( "ResetPoseRPC", PhotonTargets.All );
        }
        [PunRPC]
        private void ResetPoseRPC()
        {
            foreach( var arm in m_armList )
            {
                arm.InitRot();
            }
        }

        public void StartGame()
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                StartGameRPC();
                return;
            }

            photonView.RPC( "StartGameRPC", PhotonTargets.All );
        }
        [PunRPC]
        private void StartGameRPC()
        {
            StartCoroutine( GameState() );
        }

        public void PlayDoorAnimation( string i_anime )
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                PlayDoorAnimationRPC( i_anime );
                return;
            }
            photonView.RPC( "PlayDoorAnimationRPC", PhotonTargets.All, i_anime );
        }
        [PunRPC]
        private void PlayDoorAnimationRPC( string i_anime )
        {
            m_doorAnimator.Play( i_anime );
        }

    #endregion // Network


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
                    obj.ActiveColorL        = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmColorL;
                    obj.ActiveColorR        = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmColorR;
                    obj.ActiveColorCurve    = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmCurve;
                    obj.ActivateColorTime   = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmTime;

                    m_armList.Add( obj );
                }
            }
        }

        private void UpdateControl()
        {
            ControlSelectArm();
            ControlMoveArm();
            return;
            if( m_selectL != null )
            {
                int vPower  = Mathf.RoundToInt( Input.GetAxis( "Vertical" ) * 10.0f );
                if( Mathf.Abs( vPower ) < 2 )
                {
                    vPower = 0;
                }

                NetworkManager.Instance.SendArmPower( m_selectL.ID, 0, vPower );


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

                NetworkManager.Instance.SendArmPower( m_selectR.ID, 0, vPower );


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

        private void ControlSelectArm()
        {
            for( int i = (int)GamepadInput.GamePad.Index.One; i <= (int)GamepadInput.GamePad.Index.Four; ++i )
            {
                var index   = ( GamepadInput.GamePad.Index )i;

                if( GamepadInput.GamePad.GetButtonDown( GamepadInput.GamePad.Button.LeftShoulder, index ) )
                {
//                     m_selectL.Activate  = false;
//                     m_selectL           = NextArm( m_selectL, DEFAULT_ARM_INDEX_L, 1 );
//                     m_selectL.Activate  = true;
                }
                if( GamepadInput.GamePad.GetButtonDown( GamepadInput.GamePad.Button.RightShoulder, index ) )
                {
//                     m_selectR.Activate  = false;
//                     m_selectR           = NextArm( m_selectR, DEFAULT_ARM_INDEX_R, 1 );
//                     m_selectR.Activate  = true;
                }
            }
        }

        private void ControlMoveArm()
        {

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


        private void OnSetClearRate( int rate )
        {
            GameInformation.Instance.SetClearRate( rate );
            Debug.Log( rate );
        }

    #endregion // Private


    #region State

        private IEnumerator ReadyState()
        {
            yield return new WaitForSeconds( 1.0f );
        }

        private IEnumerator WaitPlayerState()
        {
            yield return new WaitForSeconds( 1.0f );
            StartCoroutine( PoseState() );
        }

        private IEnumerator PoseState()
        {
            yield return null;

            {
                var randList = new List< float >();
                for( int i = 0; i < m_armList.Count; ++i )
                {
                    randList.Add( Random.Range( -45.0f, 45.0f ) );
                }

                SetPose( randList.ToArray() );
                PlayDoorAnimation( OpenDoorAnimationName );
            }
            
//             yield return new WaitForSeconds( 5.0f );
// 
// 
//             PlayDoorAnimation( CloseDoorAnimationName );
// 
//             yield return new WaitForSeconds( 2.0f );
// 
// 
//             GameInformation.Instance.SetTargetPose( m_armList.ToArray() );
//             ResetPose();
// 
//             yield return new WaitForSeconds( 1.0f );

            PlayDoorAnimation( OpenDoorAnimationName );
            StartGame();
        }

        private IEnumerator GameState()
        {
            ActivateTime        = true;
//             m_selectL.Activate  = true;
//             m_selectR.Activate  = true;


            yield return null;

            while( m_time > 0.0f )
            {
                UpdateControl();
                m_time  -= Time.deltaTime;
                yield return null;
            }

            if( NetworkManager.Instance.IsMasterClient )
            {
                StartCoroutine( EndGameState() );
            }
        }

        private IEnumerator EndGameState()
        {
            PlayDoorAnimation( CloseDoorAnimationName );

            yield return new WaitForSeconds( 2.0f );

            int rate = GameInformation.Instance.SetResultPose( m_armList.ToArray() );
            NetworkManager.Instance.SetClearRate( rate );

            NetworkManager.Instance.ChangeSceneAllPlayer( EScene.Result );
        }

    #endregion // State








    } // class GameScene




} // namespace HimonoLib

