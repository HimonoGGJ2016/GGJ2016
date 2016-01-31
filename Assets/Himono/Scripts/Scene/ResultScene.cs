//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace HimonoLib
{
    public class ResultScene : MonoBehaviour
    {

        #region Variable

        #endregion // Variable


        #region Property

        #endregion // Property


        #region Public

        void Start()
        {
            StartCoroutine( ResultState() );
        }

        #endregion // Public


        #region UnityEvent

        #endregion // UnityEvent


        #region Private

        private IEnumerator ResultState()
        {
            NetworkManager.Instance.Diconnect();

            yield return new WaitForSeconds( 0.5f );

            NetworkManager.Instance.Connect();

            while( !NetworkManager.Instance.Connected )
            {
                yield return null;
            }

            NetworkManager.Instance.ChangeScene( EScene.Title );

        }

        #endregion // Private


    } // class ResultScene
    
} // namespace HimonoLib

