//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public enum EArmType
    {
        RightA,
        LeftA,
    }




    [CreateAssetMenu( menuName = "Asura/Create GameSettingTable", fileName = "GameSettingTable" )]
    public class GameSettingTable : ScriptableObject
    {
        [SerializeField]
        public GameObject   m_fadeResource  = null;
        [SerializeField, EnumListLabel( typeof( EScene ) )]
        public SceneData[]  m_sceneList     = null;
        [SerializeField]
        public string       m_handResource  = null;
        [SerializeField]
        public string       m_handPointLTag  = "";
        [SerializeField]
        public string       m_handPointRTag  = "";
        [SerializeField, EnumListLabel( typeof( EArmType ) )]
        public AsuraArm[]   m_armList       = null;
    }

    public class GameSettingManager : SingletonAuto< GameSettingManager >
    {

    #region Variable

        private GameSettingTable m_table = null;

    #endregion // Variable


    #region Property

        public static GameSettingTable Table
        {
            get
            {
                return Instance.m_table;
            }
        }


    #endregion // Property


    #region Constructor

        private GameSettingManager()
        {
            m_table = GameObject.Instantiate( Resources.Load( "Table/GameSettingTable" ) ) as GameSettingTable;
        }


    #endregion // Constructor


    #region Public

        public GameObject InstantiateFadeObject()
        {
            var res = m_table.m_fadeResource;
            if( res != null )
            {
                return GameObject.Instantiate( res ) as GameObject;
            }
            return null;
        }

        public string GetSceneName( EScene i_scene )
        {
            var sceneList   = m_table.m_sceneList;
            return sceneList[ (int)i_scene ].m_name;
        }

        public AsuraArm GetArm( EArmType i_type )
        {
            return m_table.m_armList[ (int)i_type ];
        }

        #endregion // Public

    }

    [System.Serializable]
    public struct SceneData
    {
        [SerializeField]
        public string   m_name;
    }

} // namespace HimonoLib


