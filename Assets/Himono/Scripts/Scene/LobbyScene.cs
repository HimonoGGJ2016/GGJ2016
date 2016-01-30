//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class LobbyScene : SceneBase
    {
    
    #region Variable

    #endregion // Variable

    
    #region Property

    #endregion // Property

    
    #region Public

        public void Join()
        {
            NetworkManager.Instance.Join( OnJoined );
        }

    #endregion // Public

    
    #region UnityEvent

    #endregion // UnityEvent


    #region Callback

        private void OnJoined()
        {

        }

    #endregion // Callback

    
    #region Private

    #endregion // Private


    } // class LobbyScene
    
} // namespace HimonoLib

