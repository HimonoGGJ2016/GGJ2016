//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace HimonoLib
{
    public class GameInformation : SingletonAuto< GameInformation >
    {

    #region Variable

        private int         m_localPlayerCount  = 1;
        private EDifficulty m_difficulty        = default( EDifficulty );

        private List< PoseData >    m_targetPoseList    = new List<PoseData>();
        private List< EAnswer >     m_answerList        = new List<EAnswer>();

    #endregion // Variable


    #region Property

        public int LocalPlayerCount
        {
            get
            {
                return m_localPlayerCount;
            }
            set
            {
                m_localPlayerCount  = value;
            }
        }

        public EDifficulty Difficulty
        {
            get
            {
                return m_difficulty;
            }
            set
            {
                m_difficulty    = value;
            }
        }

        public float AnswerAngle
        {
            get
            {
                return GameSettingManager.Table.m_coreGameData.m_scoreData.m_answerAngle;
            }
        }

        public float HotAngle
        {
            get
            {
                return GameSettingManager.Table.m_coreGameData.m_scoreData.m_hotAngle;
            }
        }

        public int ClearRate
        {
            get
            {
                if( m_answerList.Count == 0 )
                {
                    return 0;
                }

                float   score   = 100.0f;
                var     single  = 100.0f / m_answerList.Count;
                foreach( var answer in m_answerList )
                {
                    switch( answer )
                    {
                        case EAnswer.Hot:
                            score   -= single * 0.5f;
                            break;

                        case EAnswer.Error:
                            score   -= single;
                            break;

                        default:
                            break;
                    }
                }


                return Mathf.CeilToInt( score );
            }
        }

    #endregion // Property


    #region Public

        public void Reset()
        {
            LocalPlayerCount    = 0;
            m_difficulty        = default( EDifficulty );

            m_targetPoseList.Clear();
            m_answerList.Clear();
        }

        public void SetTargetPose( AsuraArm[ ] i_armList )
        {
            foreach( var arm in i_armList )
            {
                m_targetPoseList.Add( new PoseData( arm.ID, arm.Angles ) );
            }
        }

        public EAnswer IsAnswerPose( AsuraArm i_arm )
        {
            var target          = m_targetPoseList.Find( value => value.m_id == i_arm.ID );
            float curAngle      = i_arm.Angles.x;
            float targetAngle   = target.m_angles.x;
            float angle         = Mathf.Abs( curAngle - targetAngle );
            if( angle <= AnswerAngle )
            {
                return EAnswer.Answer;
            }
            if( angle <= HotAngle )
            {
                return EAnswer.Hot;
            }
            return EAnswer.Error;
        }

        public void AddAnswerResult( EAnswer i_answer )
        {
            m_answerList.Add( i_answer );
        }

    #endregion // Public


    #region UnityEvent

        #endregion // UnityEvent


    #region Private

        #endregion // Private

        private struct PoseData
        {
            public PoseData( int i_id, Vector3 i_angles )
            {
                m_id        = i_id;
                m_angles    = i_angles;
            }

            public int      m_id;
            public Vector3  m_angles;
        }


    } // class GameInformation

    
} // namespace HimonoLib

