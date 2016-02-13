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
            GamepadInput.GamePad.AxisButtonTime     = 0.2f;
            GamepadInput.GamePad.EnableAxisButton   = 0.7f;

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

//             while( !NetworkManager.Instance.Connected )
//             {
//                 yield return null;
//             }

            NetworkManager.Instance.ActivateUI  = false;

            NetworkManager.Instance.ChangeScene( EScene.Title );
        }

    #endregion // Private


    } // class InitializeScene
    
} // namespace HimonoLib

