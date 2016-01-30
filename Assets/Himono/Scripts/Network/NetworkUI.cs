//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace HimonoLib
{
    public class NetworkUI : MonoBehaviour
    {
    
    #region Variable

        [SerializeField]
        private Text    m_connectText   = null;
        [SerializeField]
        private Text    m_lobbyText     = null;
        [SerializeField]
        private Text    m_roomText      = null;

    #endregion // Variable


    #region Property

        public bool Connect
        {
            set
            {
                if( m_connectText != null )
                {
                    m_connectText.text = value ? "True" : "False";
                }
            }
        }

        public string Lobby
        {
            set
            {
                if( m_lobbyText != null )
                {
                    m_lobbyText.text = value;
                }
            }
        }

        public string Room
        {
            set
            {
                if( m_roomText != null )
                {
                    m_roomText.text = value;
                }
            }
        }


    #endregion // Property


    #region Public

    #endregion // Public


    #region UnityEvent

    #endregion // UnityEvent


    #region Private

    #endregion // Private


    } // class NetworkUI
    
} // namespace HimonoLib

