//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Linq;

namespace HimonoLib
{

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
        [SerializeField]
        public float        m_gameTime      = 300.0f;
        [SerializeField]
        public ResultData[] m_resultList    = null;
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

        public AudioClip GetBGM( string i_scene )
        {
            var sceneList = m_table.m_sceneList;

            var data    = sceneList.FirstOrDefault( value => value.m_name == i_scene );
            return data .m_bgm;

        }

        public AsuraArm GetArm( EArmType i_type )
        {
            return m_table.m_armList[ (int)i_type ];
        }

        public Sprite GetScoreText( int i_score )
        {
            return m_table.m_resultList.FirstOrDefault( value => i_score <= value.m_score ).m_sprite;
        }

        #endregion // Public

    }

    [System.Serializable]
    public struct SceneData
    {
        [SerializeField]
        public string       m_name;
        [SerializeField]
        public AudioClip    m_bgm;
    }

    [System.Serializable]
    public struct ResultData
    {
        [SerializeField]
        public Sprite   m_sprite;
        [SerializeField]
        public int      m_score;
    }

} // namespace HimonoLib


