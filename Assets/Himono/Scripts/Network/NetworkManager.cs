//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace HimonoLib
{
    public delegate void OnJoinLobby( bool i_result );
    public delegate void OnCreatRoomCallback( bool i_result );
    public delegate void OnEnterRoomCallback( bool i_result );


    public class NetworkManager : Photon.PunBehaviour
    {
    
    #region Variable

        [SerializeField]
        private NetworkUI   m_networkUI = null;

    #endregion // Variable


    #region Property

        public static NetworkManager Instance
        {
            get;
            private set;
        }

        public bool IsMasterClient
        {
            get
            {
                return PhotonNetwork.isMasterClient;
            }
        }

        public bool Connected
        {
            get
            {
                return PhotonNetwork.connected;
            }
        }

        public bool OfflineMode
        {
            get
            {
                return PhotonNetwork.offlineMode;
            }
            set
            {
                PhotonNetwork.offlineMode   = value;
            }
        }

        public bool InsideLobby
        {
            get
            {
                return PhotonNetwork.insideLobby;
            }
        }

        public bool InRoom
        {
            get
            {
                return PhotonNetwork.inRoom;
            }
        }





        public string LobbyName
        {
            get
            {
                return PhotonNetwork.insideLobby ? PhotonNetwork.lobby.Name : "";
            }
        }

        public string RoomName
        {
            get
            {
                return PhotonNetwork.inRoom ? PhotonNetwork.room.name : "";
            }
        }

        public int RoomPlayerCount
        {
            get
            {
                return PhotonNetwork.inRoom ? PhotonNetwork.room.playerCount : 0;
            }
        }

        public bool ActivateUI
        {
            set
            {
                if( m_networkUI != null )
                {
                    m_networkUI.gameObject.SetActive( value );
                }
            }
        }

    #endregion // Property


    #region Public

        public void Reset()
        {
            StopAllCoroutines();
        }

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings( "0.1" );
        }

        public void Diconnect()
        {
            PhotonNetwork.Disconnect();
            Reset();
        }

        public void JoinLobby( string i_name = null, OnJoinLobby i_callback = null )
        {
            StartCoroutine( JoinLobbyCoroutine( i_callback ) );
        }

        public void CreateRoom( string i_name = null, OnCreatRoomCallback i_callback = null )
        {
            if( !PhotonNetwork.insideLobby )
            {
                return;
            }

            StartCoroutine( CreateRoomCoroutine( i_name, i_callback ) );
        }

        public void JoinRoom( string i_name = null, OnEnterRoomCallback i_callback = null )
        {
            if( !PhotonNetwork.insideLobby )
            {
                return;
            }

            StartCoroutine( EnterRoomCoroutine( i_name, i_callback ) );
        }


        public void ChangeSceneAllPlayer( EScene i_scene )
        {
            if( OfflineMode )
            {
                ChangeScene( i_scene );
                return;
            }

            photonView.RPC( "ChangeScene_RPC", PhotonTargets.All, (int)i_scene );

        }
        public void ChangeScene( EScene i_scene )
        {
            SceneController.Instance.ChangeScene( i_scene );
        }
        [PunRPC]
        private void ChangeScene_RPC( int i_scene )
        {
            ChangeScene( (EScene)i_scene );
        }

    #endregion // Public


    #region UnityEvent

        void Awake()
        {
            if( Instance != null )
            {
                GameObject.Destroy( this );
                return;
            }

            Instance    = this;
            DontDestroyOnLoad( gameObject );

            if( photonView.viewID == 0 )
            {
                photonView.viewID   = 1;
            }

            OfflineMode = true;
        }

        void Start()
        {
            ActivateUI  = false;
            // Connect();
        }

        void Update()
        {
            ApplyUI();
        }

    #endregion // UnityEvent


    #region Photon

        public override void OnConnectedToMaster()
        {
            // Debug.Log( "OnConnectedToMaster" );
        }

        public override void OnJoinedLobby()
        {
            // Debug.Log( "OnJoinedLobby" );
            // PhotonNetwork.JoinRandomRoom();

            var roomList = PhotonNetwork.GetRoomList();
            if( roomList.Length <= 0 )
            {
                PhotonNetwork.CreateRoom( null );
                return;
            }

            foreach( var room in roomList )
            {
                if( !room.open )
                {
                    continue;
                }
                PhotonNetwork.JoinRoom( room.name );
                return;
            }
        }

        public void OnPhotonRandomJoinFailed()
        {
            Debug.Log( "OnPhotonRandomJoinFailed" );
            PhotonNetwork.CreateRoom( null );
        }

        public override void OnJoinedRoom()
        {
            Debug.Log( "OnJoinedRoom" );
        }

    #endregion // Photon


    #region Private

        private void ApplyUI()
        {
            if( m_networkUI == null )
            {
                return;
            }


            m_networkUI.Connect = Connected;
            m_networkUI.Lobby   = PhotonNetwork.inRoom ? PhotonNetwork.room.playerCount.ToString() : "";
            m_networkUI.Room    = RoomName;
        }

        private IEnumerator JoinLobbyCoroutine( OnJoinLobby i_callback )
        {
            if( !Connected )
            {
                yield return null;
            }

            bool ret = PhotonNetwork.JoinLobby();

            while( !InsideLobby )
            {
                yield return null;
            }

            if( i_callback != null )
            {
                i_callback( ret );
            }
        }

        private IEnumerator CreateRoomCoroutine( string i_roomName, OnCreatRoomCallback i_callback )
        {
            bool ret = PhotonNetwork.CreateRoom( i_roomName );

            if( !ret )
            {
                i_callback( false );
                yield break;
            }

            while( !InRoom )
            {
                yield return null;
            }

            if( i_callback != null )
            {
                i_callback( true );
            }
        }

        private IEnumerator EnterRoomCoroutine( string i_roomName, OnEnterRoomCallback i_callback )
        {
            bool ret = false;
            if( string.IsNullOrEmpty( i_roomName ) )
            {
                ret = PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                ret = PhotonNetwork.JoinRoom( i_roomName );
            }

            if( !ret )
            {
                i_callback( false );
                yield break;
            }

            if( !PhotonNetwork.inRoom )
            {
                yield break;
            }

            if( i_callback != null )
            {
                i_callback( true );
            }
        }

    #endregion // Private


    } // class NetworkManager
    
} // namespace HimonoLib

