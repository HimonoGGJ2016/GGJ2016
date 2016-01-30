//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class SceneBase : MonoBehaviour
    {
    
    #region Variable

    #endregion // Variable

    
    #region Property

    #endregion // Property

    
    #region Public

    #endregion // Public

    
    #region UnityEvent

        protected void Awake()
        {
            AwakeImpl();
        }
        protected virtual void AwakeImpl()
        {

        }

        protected void Start()
        {
            StartImpl();
        }
        protected virtual void StartImpl()
        {

        }

        protected void Update()
        {
            UpdateImpl();
        }
        protected virtual void UpdateImpl()
        {

        }

    #endregion // UnityEvent


    #region Private

    #endregion // Private


    } // class SceneBase
    
} // namespace HimonoLib

