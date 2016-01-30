//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Linq;

namespace HimonoLib
{
    public class GameScene : SceneBase
    {
    
    #region Variable

        private AsuraArm[]  m_armList   = null;

    #endregion // Variable

    
    #region Property

        private string HandPointTag
        {
            get
            {
                return GameSettingManager.Table.m_handPointTag;
            }

        }

    #endregion // Property

    
    #region Public

    #endregion // Public

    
    #region UnityEvent

        protected override void AwakeImpl()
        {
            NetworkManager.Instance.OnChangeArm  += OnChangePower;
            NetworkManager.Instance.OnCollectArm += OnCollectArm;
        }

        protected override void StartImpl()
        {
            if( NetworkManager.Instance.IsMasterClient )
            {
                InstantiateHands();
            }
        }

        protected override void UpdateImpl()
        {
            float hPowerL   = Mathf.Ceil( Input.GetAxis( "Horizontal" ) * 10.0f ) / 10.0f;
            float vPowerL   = Mathf.Ceil( Input.GetAxis( "Vertical" ) * 10.0f ) / 10.0f;
            float hPowerR   = Mathf.Ceil( Input.GetAxis( "HorizontalDPad" ) * 10.0f ) / 10.0f;
            float vPowerR   = Mathf.Ceil( Input.GetAxis( "VerticalDPad" ) * 10.0f ) / 10.0f;

            NetworkManager.Instance.SendArmPower( 1, hPowerL );
            NetworkManager.Instance.SendArmPower( 2, vPowerL );
            NetworkManager.Instance.SendArmPower( 4, hPowerR );
            NetworkManager.Instance.SendArmPower( 5, vPowerR );
        }

    #endregion // UnityEvent


    #region Private

        private void InstantiateHands()
        {
            var res     = GameSettingManager.Table.m_handResource;
            var list    = GameObject.FindGameObjectsWithTag( HandPointTag );
            for( int i = 0, size = list.Length; i < size; ++i )
            {
                var ts  = list[i].transform;
                var obj = PhotonNetwork.Instantiate( res, ts.position, ts.rotation, 0 );
                var arm = obj.GetComponent< AsuraArm >();
                arm.ID  = i;
            }

            NetworkManager.Instance.CollectArm();
        }

        private void OnCollectArm( AsuraArm[] i_armList )
        {
            m_armList   = i_armList;
        }

        private void OnChangePower( int i_armID, float i_power )
        {
            var arm = m_armList.FirstOrDefault( value => value.ID == i_armID );
            if( arm != null )
            {
                arm.transform.Rotate( new Vector3( 0.0f, 0.0f, i_power * 30.0f ) * Time.deltaTime );
            }
        }

    #endregion // Private




    } // class GameScene
    
} // namespace HimonoLib

