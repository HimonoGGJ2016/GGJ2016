//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HimonoLib
{
    public class LobbyScene : MonoBehaviour
    {
    
    #region Variable

        private enum EMenu
        {
            None,
            Wait,

            Mode,
            OfflineCount,
            OnlineLogin,
        }

        [SerializeField, EnumListLabel( typeof( EMenu ) ) ]
        private GameObject[]  m_modeUIList  = null;

    #endregion // Variable


    #region Property

        private string RoomName
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        private int RoomMemberCount
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }


    #endregion // Property


    #region UnityEvent

        void Awake()
        {
            GameInformation.Instance.Reset();
        }

        void Start()
        {
            SetMenuUI( EMenu.Mode );
        }

        void Update()
        {
            

        }

    #endregion // UnityEvent


    #region Callback

        public void OnSelectedOffline()
        {
            NetworkManager.Instance.OfflineMode = true;
            OnMenuOfflineCount();
        }

        public void OnSelectedOnline()
        {
            NetworkManager.Instance.OfflineMode = false;
            GameInformation.Instance.LocalPlayerCount   = 1;

        }

        public void OnSetOfflineCount( int i_count )
        {
            GameInformation.Instance.LocalPlayerCount   = i_count;
            NetworkManager.Instance.ChangeSceneAllPlayer( EScene.Game );
            if( NetworkManager.Instance.IsMasterClient && PhotonNetwork.room != null )
            {
                PhotonNetwork.room.open = false;
            }
        }

        public void OnCreateRoom( string i_roomName )
        {
            NetworkManager.Instance.CreateRoom( i_roomName, ( result ) =>
            {

            } );
        }

        public void OnEnterRoom( string i_roomName )
        {
            NetworkManager.Instance.EnterRoom( i_roomName, ( result ) =>
            {

            } );
        }

        public void OnStartOnlineGame()
        {

        }


        public void OnMenuMode()
        {
            SetMenuUI( EMenu.Mode );
            
        }

        public void OnMenuOfflineCount()
        {
            SetMenuUI( EMenu.OfflineCount );
        }


    #endregion // Callback

    
    #region Private

        private void SetMenuUI( EMenu i_menu )
        {
            if( m_modeUIList == null || m_modeUIList.Length <= 0 )
            {
                return;
            }

            StopAllCoroutines();

            int index   = (int)i_menu;
            for( int i = 0, size = m_modeUIList.Length; i < size; ++i )
            {
                var ui  = m_modeUIList[ i ];
                if( ui != null )
                {
                    ui.SetActive( i == index );
                    if( i == index )
                    {
                        StartCoroutine( ModeState( ui ) );
                    }
                }
            }


        }

    #endregion // Private


    #region State

        private IEnumerator OnlineConnectState()
        {

            if( NetworkManager.Instance == null )
            {
                OnMenuMode();
                yield break;
            }

            if( !NetworkManager.Instance.Connected )
            {
                NetworkManager.Instance.Connect();
                while( !NetworkManager.Instance.Connected )
                {
                    yield return null;
                }
            }

            //NetworkManager.Instance.Join( OnJoined );

        }

        private IEnumerator ModeState( GameObject i_obj )
        {
            yield return null;

            var selectList  = i_obj.GetComponentsInChildren< Selectable >();

            if( selectList.Length == 0 )
            {
                yield break;
            }

            

            int max = selectList.Length;
            int cur = 0;

            selectList[ cur ].Select();

            while( true )
            {
                yield return null;

                if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.DownL, GamepadInput.GamePad.Index.One ) || GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.RightL, GamepadInput.GamePad.Index.One ) )
                {
                    cur++;
                }
                else if( GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.UpL, GamepadInput.GamePad.Index.One ) || GamepadInput.GamePad.GetButton( GamepadInput.GamePad.AxisButton.LeftL, GamepadInput.GamePad.Index.One ) )
                {
                    cur--;
                }
                else
                {
                    continue;
                }

                if( cur < 0 )
                {
                    cur = max - 1;
                }

                cur = cur % max;
                selectList[ cur ].Select();

            }




            
        }



    #endregion // State


    } // class LobbyScene
    
} // namespace HimonoLib

