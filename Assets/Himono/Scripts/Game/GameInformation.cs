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

        private List< PoseData >    m_targetPoseList    = new List<PoseData>();
        private int m_clearRate = 0;



        #endregion // Variable


        #region Property

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

        public void Initialize()
        {
            m_targetPoseList.Clear();
            m_clearRate = 0;
        }

        public void SetTargetPose( AsuraArm[ ] i_armList )
        {
            foreach( var arm in i_armList )
            {
                m_targetPoseList.Add( new PoseData( arm.ID, arm.FrontAngle, arm.CenterAngle, arm.BackAnglet ) );
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
                    float angle = arm.BackAnglet - pose.m_back;
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
            public PoseData( int i_id, float i_fornt, float i_center, float i_back )
            {
                m_id    = i_id;
                m_front = i_fornt;
                m_center = i_center;
                m_back = i_back;
            }

            public int      m_id;
            public float    m_front;
            public float    m_center;
            public float    m_back;
        }


    } // class GameInformation

    
} // namespace HimonoLib

