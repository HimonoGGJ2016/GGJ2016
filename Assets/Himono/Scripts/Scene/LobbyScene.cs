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
        [SerializeField]
        private Button  m_loginButton   = null;

        #endregion // Variable


        #region Property

        public bool ShowLoginButton
        {
            set
            {
                if( m_loginButton == null )
                {
                    return;

                }
                m_loginButton.gameObject.SetActive( value );
            }
        }

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
            ShowLoginButton = false;
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

            GameInformation.Instance.Initialize();
        }

    #endregion // UnityEvent


    #region Callback

        private void OnJoined()
        {
            if( NetworkManager.Instance.Connected )
            {
                ShowStartButton = true;
            }
        }

    #endregion // Callback

    
    #region Private

    #endregion // Private


    } // class LobbyScene
    
} // namespace HimonoLib

