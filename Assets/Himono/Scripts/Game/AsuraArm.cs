//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class AsuraArm : Photon.MonoBehaviour
    {
    
    #region Variable

        private int m_id    = 0;

    #endregion // Variable

    
    #region Property

        public int ID
        {
            get
            {
                return m_id;
            }
            set
            {
                if( NetworkManager.Instance.IsMasterClient )
                {
                    photonView.RPC( "SetID_RPC", PhotonTargets.All, value );
                }
            }
        }


    #endregion // Property

    
    #region Public


    #endregion // Public

    
    #region UnityEvent

        void Update()
        {

        }

    #endregion // UnityEvent

    
    #region Private

        [PunRPC]
        private void SetID_RPC( int i_id )
        {
            m_id    = i_id;
        }

    #endregion // Private


    } // class AsuraArm
    
} // namespace HimonoLib

