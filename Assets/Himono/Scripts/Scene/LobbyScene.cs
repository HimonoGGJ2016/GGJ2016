//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace HimonoLib
{
    public class LobbyScene : SceneBase
    {
    
    #region Variable

        [SerializeField]
        private Button  m_startButton   = null;


    #endregion // Variable

    
    #region Property

        public bool ShowStartButton
        {
            set
            {
                if( m_startButton == null )
                {
                    return;

                }
                m_startButton.gameObject.SetActive( value );
            }

        }

    #endregion // Property

    
    #region Public

        public void Join()
        {
            NetworkManager.Instance.Join( OnJoined );
        }

        public void StartGame()
        {
            NetworkManager.Instance.ChangeSceneAllPlayer( EScene.Game );
        }


        #endregion // Public


        #region UnityEvent

        protected override void AwakeImpl()
        {
            ShowStartButton = false;

            NetworkManager.Instance.ActivateUI = true;
        }

    #endregion // UnityEvent


    #region Callback

        private void OnJoined()
        {
            if( NetworkManager.Instance.Connected && NetworkManager.Instance.IsMasterClient )
            {
                ShowStartButton = true;
            }
        }

    #endregion // Callback

    
    #region Private

    #endregion // Private


    } // class LobbyScene
    
} // namespace HimonoLib

