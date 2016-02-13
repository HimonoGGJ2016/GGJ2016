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

        private enum ESetPoseOption
        {
            None,

            Target,
        }

        [SerializeField]
        private GameSceneUI m_gameUI    = null;

        private float   m_time  = 0.0f;

        private List< AsuraArm >    m_armList   = new List<AsuraArm>();

        private Dictionary< int, AsuraArm > m_selectArmsR   = new Dictionary<int, AsuraArm>();
        private Dictionary< int, AsuraArm > m_selectArmsL   = new Dictionary<int, AsuraArm>();

        private const int DEFAULT_ARM_INDEX_L   = 0;
        private const int DEFAULT_ARM_INDEX_R   = 1000;

    #endregion // Variable


    #region Property

        private bool ActivateTime
        {
            set
            {
                m_gameUI.ActivateTime   = value;
            }
        }

        private float TimeText
        {
            set
            {
                m_gameUI.TimeText   = value;
            }
        }

        private int LocalPlayerCount
        {
            get
            {
                return GameInformation.Instance.LocalPlayerCount;
            }
        }

        private EDifficulty Difficulty
        {
            get
            {
                return GameInformation.Instance.Difficulty;
            }
        }

        private Vector3 LimitArmAngles
        {
            get
            {
                var data    = GameSettingManager.Table.m_coreGameData.m_armData;
                return new Vector3( data.m_limitBackAngle, data.m_limitCenterAngle, data.m_limitFrontAngle );
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


    #region UnityEvent

        void Awake()
        {
            m_time      = GameSettingManager.Table.m_gameTime;
            ActivateTime = false;



            InstantiateHands( HandPointLeftTag,     DEFAULT_ARM_INDEX_L,    GameSettingManager.Instance.GetArm( EArmType.LeftA ) );
            InstantiateHands( HandPointRightTag,    DEFAULT_ARM_INDEX_R,    GameSettingManager.Instance.GetArm( EArmType.RightA ) );
        }

        void Start()
        {
            for( int i = 0; i < LocalPlayerCount; ++i )
            {
                m_selectArmsR.Add( i + 1, null );
                m_selectArmsL.Add( i + 1, null );
            }
            m_gameUI.InstantiateAnswerUI( m_armList.Count );

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

        private void SetPose( object i_angleList, ESetPoseOption i_option = ESetPoseOption.None )
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                SetPoseRPC( i_angleList, (int)i_option );
                return;
            }
            photonView.RPC( "SetPoseRPC", PhotonTargets.All, i_angleList, (int)i_option );
        }
        [PunRPC]
        private void SetPoseRPC( object i_angleList, int i_option )
        {
            var list    = (Vector3[])i_angleList;
            for( int i = 0; i < list.Length; ++i )
            {
                var value = list[i];
                m_armList[ i ].SetPose( value );
            }

            var op  = (ESetPoseOption)i_option;

            if( op == ESetPoseOption.Target )
            {
                GameInformation.Instance.SetTargetPose( m_armList.ToArray() );
                m_gameUI.SaveHint();
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

        private void EndGame()
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                EndGameRPC();
                return;
            }

            photonView.RPC( "EndGameRPC", PhotonTargets.All );
        }
        [PunRPC]
        private void EndGameRPC()
        {
            StartCoroutine( ScoreState() );
        }

        public void SendArmPower( int i_armID, int i_handID, int i_power )
        {
            if( NetworkManager.Instance.OfflineMode )
            {
                SendArmPowerRPC( i_armID, i_handID, i_power );
                return;
            }

            photonView.RPC( "SendArmPowerRPC", PhotonTargets.All, i_armID, i_handID, i_power );
        }
        [PunRPC]
        private void SendArmPowerRPC( int i_armID, int i_handID, int i_power )
        {
            var arm = m_armList.FirstOrDefault( value => value.ID == i_armID );
            if( arm != null )
            {
                arm.RotateArm( i_handID, i_power * Time.deltaTime );
            }
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
            m_gameUI.PlayDoorAnimation( i_anime );
        }

    #endregion // Network


    #region Control

        private void UpdateControl()
        {
            ControlSelectArm();
            ControlMoveArm();
            return;
        }

        private void ControlSelectArm()
        {
            int startIndex  = (int)GamepadInput.GamePad.Index.One;
            for( int i = startIndex; i < startIndex + GameInformation.Instance.LocalPlayerCount; ++i )
            {
                var index   = ( GamepadInput.GamePad.Index )i;

                if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.UpL, index ) )
                {
                    var arm     = GetActivateArm( i, false );
                    var next    = NextArm( arm.ID, DEFAULT_ARM_INDEX_L, -1 );
                    SetActivateArm( i, false, next );
                }
                if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.DownL, index ) )
                {
                    var arm = GetActivateArm( i, false );
                    var next = NextArm( arm.ID, DEFAULT_ARM_INDEX_L, 1 );
                    SetActivateArm( i, false, next );
                }
                if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.UpR, index ) )
                {
                    var arm     = GetActivateArm( i, true );
                    var next    = NextArm( arm.ID, DEFAULT_ARM_INDEX_R, -1 );
                    SetActivateArm( i, true, next );
                }
                if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.DownR, index ) )
                {
                    var arm = GetActivateArm( i, true );
                    var next = NextArm( arm.ID, DEFAULT_ARM_INDEX_R, 1 );
                    SetActivateArm( i, true, next );
                }
            }
        }

        private void ControlMoveArm()
        {
            int startIndex = (int)GamepadInput.GamePad.Index.One;
            for( int i = startIndex; i < startIndex + GameInformation.Instance.LocalPlayerCount; ++i )
            {
                var index = (GamepadInput.GamePad.Index)i;

                {
                    Vector2 axis    = -GamepadInput.GamePad.GetAxis( GamepadInput.GamePad.Axis.LeftStick, index );
                    if( ValidAxis( axis.x ) )
                    {
                        var power = ComputeArmMovePower( axis.x );
                        var arm = GetActivateArm( i, false );
                        SendArmPower( arm.ID, 0, power );
                    }
                }

                {
                    Vector2 axis = GamepadInput.GamePad.GetAxis( GamepadInput.GamePad.Axis.RightStick, index );
                    if( ValidAxis( axis.x ) )
                    {
                        var power = ComputeArmMovePower( axis.x );
                        var arm = GetActivateArm( i, true );
                        SendArmPower( arm.ID, 0, power );
                    }
                }
            }
        }

        private bool ValidAxis( float i_axis )
        {
            return Mathf.Abs( i_axis ) > GameSettingManager.Table.m_coreGameData.m_controlData.m_enableAxis;
        }

        private int ComputeArmMovePower( float i_axis )
        {
            float power = GameSettingManager.Table.m_coreGameData.m_controlData.m_armPower;
            return Mathf.RoundToInt( i_axis * power );
        }

    #endregion // Control


    #region Arm

        private AsuraArm GetActivateArm( int i_localPlayerID, bool i_right )
        {
            var list = i_right ? m_selectArmsR : m_selectArmsL;
            return list[ i_localPlayerID ];
        }

        private void SetActivateArm( int i_localPlayerID, bool i_right, AsuraArm i_arm )
        {
            var list    = i_right ? m_selectArmsR : m_selectArmsL;
            var arm     = list[ i_localPlayerID ];
            if( arm != null )
            {
                arm.Activate( false, i_localPlayerID );
            }
            if( i_arm != null )
            {
                i_arm.Activate( true, i_localPlayerID );
            }

            list[ i_localPlayerID ] = i_arm;
        }

        private AsuraArm NextArm( int i_id, int i_default, int i_add )
        {
            int nextID  = i_id + i_add;
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

        private Vector3[ ] GetArmAngleList()
        {
            var list    = new List< Vector3 >( m_armList.Count );
            foreach( var arm in m_armList )
            {
                list.Add( arm.Angles );
            }
            return list.ToArray();
        }

    #endregion // Arm


    #region Private

        private void InstantiateHands( string i_tag, int i_firstID, AsuraArm i_res )
        {
            {
                var list = GameObject.FindGameObjectsWithTag( i_tag );
                var sorted  = list.OrderBy( value => -value.transform.position.y ).ToArray();
                for( int i = 0, size = sorted.Length; i < size; ++i )
                {
                    var point   = sorted[ i ].transform;
                    var obj     = GameObject.Instantiate( i_res ) as AsuraArm;
                    obj.transform.SetParent( point, false );

                    obj.ID      = i_firstID + i;
                    obj.ActiveColorCurve    = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmCurve;
                    obj.ActivateColorTime   = GameSettingManager.Table.m_coreGameData.m_armData.m_activeArmTime;
                    obj.LimitAngles         = LimitArmAngles;
                    obj.SetActiveColors( GameSettingManager.Table.m_coreGameData.m_armData.m_activeColors );

                    m_armList.Add( obj );
                }
            }
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
                var randList = new List< Vector3 >();
                for( int i = 0; i < m_armList.Count; ++i )
                {
                    var angles  = Vector3.zero;
                    angles.x = Random.Range( -LimitArmAngles.x, LimitArmAngles.x );
                    angles.y = Difficulty == EDifficulty.Hard ? Random.Range( -LimitArmAngles.y, LimitArmAngles.y ) : 0.0f;
                    angles.z = Difficulty == EDifficulty.Hard ? Random.Range( -LimitArmAngles.z, LimitArmAngles.z ) : 0.0f;
                    randList.Add( angles );
                }

                SetPose( randList.ToArray(), ESetPoseOption.Target );
                PlayDoorAnimation( OpenDoorAnimationName );
            }

            m_gameUI.ShowRememberUI();
            
            yield return new WaitForSeconds( 5.0f );


            PlayDoorAnimation( CloseDoorAnimationName );

            yield return new WaitForSeconds( 2.0f );


            GameInformation.Instance.SetTargetPose( m_armList.ToArray() );
            ResetPose();

            yield return new WaitForSeconds( 1.0f );

            PlayDoorAnimation( OpenDoorAnimationName );
            StartGame();
        }

        private IEnumerator GameState()
        {
            for( int i = 0; i < LocalPlayerCount; ++i )
            {
                SetActivateArm( i + 1, true, m_armList.Find( value => value.ID == DEFAULT_ARM_INDEX_R ) );
                SetActivateArm( i + 1, false, m_armList.Find( value => value.ID == DEFAULT_ARM_INDEX_L ) );
            }

            ActivateTime    = true;
            m_gameUI.ShowStartUI();

            yield return null;

            float halfTime  = m_time * 0.5f;

            while( m_time > halfTime )
            {
                UpdateControl();
                m_time  -= Time.deltaTime;
                yield return null;
            }

            m_gameUI.ShowHint( 5.0f );

            while( m_time > 0.0f )
            {
                UpdateControl();
                m_time -= Time.deltaTime;
                yield return null;
            }




            {
                int startIndex = (int)GamepadInput.GamePad.Index.One;
                for( int i = startIndex; i < startIndex + GameInformation.Instance.LocalPlayerCount; ++i )
                {
                    var armL = GetActivateArm( i, false );
                    armL.Activate( false, i );

                    var armR = GetActivateArm( i, true );
                    armR.Activate( false, i );
                }
            }

            if( NetworkManager.Instance.IsMasterClient )
            {
                SetPose( GetArmAngleList() );
                EndGame();
            }
        }

        private IEnumerator ScoreState()
        {
            yield return new WaitForSeconds( 1.0f );

            foreach( var arm in m_armList )
            {
                var result = GameInformation.Instance.IsAnswerPose( arm );
                GameInformation.Instance.AddAnswerResult( result );
                m_gameUI.ShowAnswer( result, arm.Positions[ 0 ] );
                yield return new WaitForSeconds( 0.25f );
            }




            yield return new WaitForSeconds( 2.0f );


            PlayDoorAnimation( CloseDoorAnimationName );

            yield return new WaitForSeconds( 2.0f );

            NetworkManager.Instance.ChangeSceneAllPlayer( EScene.Result );



            yield return null;
        }

//         private IEnumerator EndGameState()
//         {
//             PlayDoorAnimation( CloseDoorAnimationName );
// 
//             yield return new WaitForSeconds( 2.0f );
// 
//             int rate = GameInformation.Instance.SetResultPose( m_armList.ToArray() );
//             NetworkManager.Instance.SetClearRate( rate );
// 
//             NetworkManager.Instance.ChangeSceneAllPlayer( EScene.Result );
//         }

    #endregion // State


    } // class GameScene




} // namespace HimonoLib

