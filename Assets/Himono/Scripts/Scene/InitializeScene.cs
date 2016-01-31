//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace HimonoLib
{
    public class InitializeScene : MonoBehaviour
    {

        #region Variable

        #endregion // Variable


        #region Property

        #endregion // Property


        #region Public

        #endregion // Public


        #region UnityEvent

        void Start()
        {
            StartCoroutine( UpdateState() );
        }

        #endregion // UnityEvent


        #region Private

        private IEnumerator UpdateState()
        {
            while( NetworkManager.Instance == null )
            {
                yield return null;
            }

            NetworkManager.Instance.ActivateUI  = true;

            while( !NetworkManager.Instance.Connected )
            {
                yield return null;
            }

            NetworkManager.Instance.ChangeScene( EScene.Title );
        }

        #endregion // Private


    } // class InitializeScene
    
} // namespace HimonoLib

