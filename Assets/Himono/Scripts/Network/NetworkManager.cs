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


    #region Event

        public event System.Action< AsuraArm[] >    OnCollectArm        = (armList) => {};
        public event System.Action< int, int, int > OnChangeArm     = (armID, handID, power) => {};

    #endregion // Event


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

        public void ChangeSceneAllPlayer( EScene i_scene )
        {
            if( IsMasterClient && Connected )
            {
                photonView.RPC( "ChangeScene_RPC", PhotonTargets.All, (int)i_scene );
                return;
            }
            ChangeScene( i_scene );

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


        public void CollectArm()
        {
            if( Connected )
            {
                photonView.RPC( "CollectArmRPC", PhotonTargets.All );
            }
            
        }
        [PunRPC]
        private void CollectArmRPC()
        {
            OnCollectArm( GameObject.FindObjectsOfType< AsuraArm >() );
        }

        public void SendArmPower( int i_armID, int i_handID, int i_power )
        {
            if( Connected )
            {
                photonView.RPC( "SendArmPowerRPC", PhotonTargets.All, i_armID, i_handID, i_power );
            }
            
        }
        [PunRPC]
        private void SendArmPowerRPC( int i_armID, int i_handID, int i_power )
        {
            OnChangeArm( i_armID, i_handID, i_power );
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
            // Debug.LogFormat( "Horizontal={0}", Input.GetButtonDown( "SelectR" ) );
            
//             Debug.LogFormat( "Horizontal={0}", Input.GetAxis( "HorizontalDPad" ) );
//             Debug.LogFormat( "Vertical={0}", Input.GetAxis( "VerticalDPad" ) );

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

            yield return new WaitForSeconds( 2.0f );

            if( i_callback != null )
            {
                i_callback();
            }
        }

    #endregion // Private


    } // class NetworkManager
    
} // namespace HimonoLib

