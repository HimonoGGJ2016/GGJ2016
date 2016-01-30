//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace HimonoLib
{

    public class FadeController : MonoBehaviour
    {

    #region Variable
        
        public enum EFade
        {
            In,
            Out,
        }

        private Animator    m_animator  = null;

    #endregion // Variable


    #region Event

        public System.Action< EFade >   OnFinishedFade  = ( fade ) => {};

    #endregion // Event

    
    #region Property

    #endregion // Property

    
    #region Public

        public void FadeIn()
        {
            if( m_animator != null )
            {
                m_animator.Play( "In" );
            }
        }

        public void FadeOut()
        {
            if( m_animator != null )
            {
                m_animator.Play( "Out" );
            }
        }

    #endregion // Public

    
    #region UnityEvent

        void Awake()
        {
            m_animator  = GetComponent< Animator >();
            DontDestroyOnLoad( gameObject );
        }

        void Start()
        {

        }

        void Update()
        {

        }

    #endregion // UnityEvent


    #region Callback

        private void OnFinishedFadeInAnimation()
        {
            OnFinishedFade( EFade.In );
        }

        private void OnFinishedFadeOutAnimation()
        {
            OnFinishedFade( EFade.Out );
        }

    #endregion // Callback


    }

} // namespace HimonoLib


