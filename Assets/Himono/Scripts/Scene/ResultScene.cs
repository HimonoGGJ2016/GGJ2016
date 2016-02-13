//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HimonoLib
{
    public class ResultScene : MonoBehaviour
    {

        #region Variable

        [SerializeField]
        private Text    m_scoreText = null;
        [SerializeField]
        private Image   m_textImage = null;

        #endregion // Variable


        #region Property

        private int Score
        {
            set
            {
                if( m_scoreText != null )
                {
                    m_scoreText.text    = value.ToString();
                }
            }
        }

        #endregion // Property


        #region Public

        void Start()
        {
            int score   = GameInformation.Instance.ClearRate;
            Score   = score;
            m_textImage.sprite  = GameSettingManager.Instance.GetScoreText( score );
            StartCoroutine( ResultState() );
        }

        #endregion // Public


        #region UnityEvent

        #endregion // UnityEvent


        #region Private

        private IEnumerator ResultState()
        {
            NetworkManager.Instance.Diconnect();

            yield return new WaitForSeconds( 5.0f );

            NetworkManager.Instance.ChangeScene( EScene.Title );

        }

        #endregion // Private


    } // class ResultScene
    
} // namespace HimonoLib

