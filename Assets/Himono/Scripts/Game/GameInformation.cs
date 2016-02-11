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

        private int         m_localPlayerCount  = 0;
        private EDifficulty m_difficulty        = default( EDifficulty );



        private List< PoseData >    m_targetPoseList    = new List<PoseData>();
        private int m_clearRate = 0;



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






        public int ClearRate
        {
            get
            {
                return m_clearRate;
            }
        }

        public float ClearAngle
        {
            get
            {
                return 10.0f;
            }
        }

    #endregion // Property


    #region Public

        public void Reset()
        {
            LocalPlayerCount    = 0;
            m_difficulty        = default( EDifficulty );

            m_targetPoseList.Clear();
            m_clearRate = 0;

        }

        public void SetTargetPose( AsuraArm[ ] i_armList )
        {
            foreach( var arm in i_armList )
            {
                m_targetPoseList.Add( new PoseData( arm.ID, arm.Angles ) );
            }
        }

        public int SetResultPose( AsuraArm[ ] i_armList )
        {
            int clearCount  = 0;
            foreach( var arm in i_armList )
            {
                var pose    = m_targetPoseList.Find( value => value.m_id == arm.ID );

//                 {
//                     float angle = arm.FrontAngle - pose.m_front;
//                     if( Mathf.Abs( angle ) <= ClearAngle )
//                     {
//                         clearCount++;
//                     }
//                 }
// 
//                 {
//                     float angle = arm.CenterAngle - pose.m_center;
//                     if( Mathf.Abs( angle ) <= ClearAngle )
//                     {
//                         clearCount++;
//                     }
//                 }
// 
                {
                    float angle = arm.Angles.x - pose.m_angles.x;
                    if( Mathf.Abs( angle ) <= ClearAngle )
                    {
                        clearCount++;
                    }
                }
            }

            int max = i_armList.Length;
            int rate = clearCount >= max ? 100 : Mathf.CeilToInt( Mathf.Clamp01( (float)clearCount / (float)max ) * 100.0f );
            return rate;
        }

        public void SetClearRate( int i_rate )
        {
            m_clearRate = i_rate;
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

