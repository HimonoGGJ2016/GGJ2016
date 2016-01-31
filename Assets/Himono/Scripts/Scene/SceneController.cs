//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace HimonoLib
{

    public class SceneController : SingletonMonoBehaviour< SceneController >
    {
    
    #region Variable

        private FadeController  m_fade      = null;
        private AudioSource     m_bgmAudio  = null;

        private bool    m_changingScene = false;
        private bool    m_fading        = false;

    #endregion // Variable

    
    #region Property

    #endregion // Property

    
    #region Public

        public void ChangeScene( EScene i_scene )
        {
            var name = GameSettingManager.Instance.GetSceneName( i_scene );
            ChangeScene( name );
        }
        public void ChangeScene( string i_sceneName )
        {
            if( !m_changingScene )
            {
                StartCoroutine( ChangeSceneCoroutine( i_sceneName ) );
            }
        }

        public void InitializeFade( FadeController i_fade )
        {
            m_fade  = i_fade;
            m_fade.OnFinishedFade   += OnFinishedFade;
        }

    #endregion // Public

    
    #region UnityEvent

        protected override void AwakeImpl()
        {
            m_bgmAudio      = gameObject.AddComponent< AudioSource >();
            m_bgmAudio.loop = true;

            DontDestroyOnLoad( gameObject );
        }

        void Start()
        {
            InitializeFade( GameObject.FindObjectOfType< FadeController >() );
            // PlayBGM( Application.loadedLevelName );
        }

    #endregion // UnityEvent

    
    #region Private

        
        private void OnFinishedFade( FadeController.EFade i_fade )
        {
            m_fading    = false;
        }

        private void PlayBGM( string i_scene )
        {
            var audio   = GameSettingManager.Instance.GetBGM( i_scene );
            if( audio != null )
            {
                m_bgmAudio.clip = audio;
                m_bgmAudio.Play();
            }
            else
            {
                StopBGM();
            }
        }

        private void StopBGM()
        {
            m_bgmAudio.Stop();
            m_bgmAudio.clip = null;
        }



        private IEnumerator ChangeSceneCoroutine( string i_sceneName )
        {
            m_changingScene = true;


            if( m_fade != null )
            {
                m_fading    = true;
                m_fade.FadeOut();
            }
            while( m_fading )
            {
                yield return null;
            }

            StopBGM();

            SceneManager.LoadScene( i_sceneName );

            PlayBGM( i_sceneName );
            

            if( m_fade != null )
            {
                m_fading = true;
                m_fade.FadeIn();
            }
            while( m_fading )
            {
                yield return null;
            }


            m_changingScene = false;
        }

    #endregion // Private


    } // class SceneManager
    
} // namespace HimonoLib

