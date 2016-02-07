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
        public event System.Action< int >        OnClearRate        = (rate) =>{};

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
            OnCollectArm = ( armList ) => {};
            OnChangeArm = ( armID, handID, power ) => {};
            OnClearRate = ( rate ) => {};
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

        public void Join( System.Action i_callback )
        {
            StartCoroutine( JoinCoroutine( i_callback ) );
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



        public void SetClearRate( int i_rate )
        {
            if( Connected )
            {
                photonView.RPC( "SetClearRateRPC", PhotonTargets.All, i_rate );
            }
        }
        [PunRPC]
        private void SetClearRateRPC( int i_rate )
        {
            OnClearRate( i_rate );
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

