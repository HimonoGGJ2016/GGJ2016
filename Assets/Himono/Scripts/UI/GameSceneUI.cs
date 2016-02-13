//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace HimonoLib
{
    public class GameSceneUI : MonoBehaviour
    {
    
    #region Variable

        [SerializeField]
        private Text        m_timeText  = null;
        [SerializeField]
        private GameObject  m_timePanel = null;
        [SerializeField]
        private Animator    m_doorAnimator  = null;

        [SerializeField]
        private RectTransform   m_answerParent      = null;
        [SerializeField]
        private Image           m_answerUIOrigin    = null;

        [SerializeField]
        private GameObject      m_hintCamera    = null;
        [SerializeField]
        private GameObject      m_hintPhoto     = null;

        [SerializeField]
        private GameObject      m_rememberUI    = null;
        [SerializeField]
        private GameObject      m_startUI       = null;

        private Image[]     m_answerUIList  = null;

    #endregion // Variable


    #region Property

        public bool ActivateTime
        {
            set
            {
                if( m_timePanel == null )
                {
                    return;
                }

                m_timePanel.SetActive( value );
            }
        }

        public float TimeText
        {
            set
            {
                if( m_timeText == null )
                {
                    return;
                }

                value   = Mathf.Max( 0.0f, value );
                m_timeText.text = value.ToString( "F2" );
            }
        }

    #endregion // Property

    
    #region Public

        public void PlayDoorAnimation( string i_name )
        {
            m_doorAnimator.Play( i_name );
        }

        public void ShowAnswer( EAnswer i_answer, Vector3 i_position, Camera i_camera = null )
        {
            var ui      = FindUnunseAnswerUI();
            var rect    = ui.GetComponent< RectTransform >();

            if( i_camera == null )
            {
                i_camera    = Camera.main;
            }


            var screenPos   = i_camera.WorldToScreenPoint( i_position );
            var localPos    = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( m_answerParent, screenPos, null, out localPos );
            rect.localPosition  = localPos;
            ui.sprite           = GetAnswerUISprite( i_answer );
            ui.gameObject.SetActive( true );

        }

        public void InstantiateAnswerUI( int i_count )
        {
            var list = new List< Image >( i_count );
            for( int i = 0; i < i_count; ++i )
            {
                var ui  = GameObject.Instantiate( m_answerUIOrigin ) as Image;
                ui.transform.SetParent( m_answerParent.transform, false );
                list.Add( ui );
            }

            m_answerUIList  = list.ToArray();
        }

        public void ShowRememberUI()
        {
            m_rememberUI.SetActive( true );
        }

        public void ShowStartUI()
        {
            m_startUI.SetActive( true );
        }

        public void SaveHint()
        {
            StartCoroutine( SaveHintState() );
        }

        public void ShowHint( float i_time )
        {
            StartCoroutine( HintFadeState( i_time ) );
        }

    #endregion // Public

    
    #region UnityEvent

    #endregion // UnityEvent

    
    #region Private

        private Image FindUnunseAnswerUI()
        {
            var ui  = m_answerUIList.FirstOrDefault( value => !value.gameObject.activeInHierarchy );
            return ui;
        }

        private Sprite GetAnswerUISprite( EAnswer i_answer )
        {
            switch( i_answer )
            {
                case EAnswer.Answer:
                    return GameSettingManager.Table.m_coreGameData.m_scoreData.m_answerSprite;
                case EAnswer.Hot:
                    return GameSettingManager.Table.m_coreGameData.m_scoreData.m_hotSprite;
                case EAnswer.Error:
                    return GameSettingManager.Table.m_coreGameData.m_scoreData.m_errorSprite;

                default:
                    break;
            }

            return null;
        }

        private IEnumerator SaveHintState()
        {
            if( m_hintCamera != null )
            {
                m_hintCamera.SetActive( true );
            }

            yield return null;

            yield return new WaitForEndOfFrame();

            if( m_hintCamera != null )
            {
                m_hintCamera.SetActive( false );
            }
        }

        private IEnumerator HintFadeState( float i_time )
        {
            if( m_hintPhoto != null )
            {
                m_hintPhoto.SetActive( true );
            }
            yield return new WaitForSeconds( i_time );

            if( m_hintPhoto != null )
            {
                m_hintPhoto.SetActive( false );
            }

        }
    #endregion // Private


    } // class GameSceneUI
    
} // namespace HimonoLib

