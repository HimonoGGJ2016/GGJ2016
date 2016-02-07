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


    #endregion // Property

    
    #region Public



    #endregion // Public


    #region UnityEvent

        void Awake()
        {
            GameInformation.Instance.Reset();

            SetMenuUI( EMenu.Mode );
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

            int index   = (int)i_menu;
            for( int i = 0, size = m_modeUIList.Length; i < size; ++i )
            {
                var ui  = m_modeUIList[ i ];
                if( ui != null )
                {
                    ui.SetActive( i == index );
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

            // NetworkManager.Instance.Join( OnJoined );

        }



    #endregion // State


    } // class LobbyScene
    
} // namespace HimonoLib

