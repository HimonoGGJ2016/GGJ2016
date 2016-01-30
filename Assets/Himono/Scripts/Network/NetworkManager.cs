//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace HimonoLib
{
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

        public bool Connected
        {
            get
            {
                return PhotonNetwork.connected;
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

    #endregion // Property


    #region Public

        public void Join( System.Action i_callback )
        {
            StartCoroutine( JoinCoroutine( i_callback ) );
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
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings( "0.1" );
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
            PhotonNetwork.JoinRandomRoom();
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
            m_networkUI.Lobby   = LobbyName;
            m_networkUI.Room    = RoomName;
        }

        private IEnumerator JoinCoroutine( System.Action i_callback )
        {
            if( !Connected )
            {
                yield return null;
            }

            PhotonNetwork.JoinRandomRoom();

            if( !PhotonNetwork.inRoom )
            {
                yield return null;
            }

            if( i_callback != null )
            {
                i_callback();
            }
        }

    #endregion // Private


    } // class NetworkManager
    
} // namespace HimonoLib

