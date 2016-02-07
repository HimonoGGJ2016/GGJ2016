//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class RuntimeAsura
    {
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeAsura()
        {
            var daemon = new GameObject( "GameDaemon" );
            GameObject.DontDestroyOnLoad( daemon );

            {
                var nwObj   = GameSettingManager.Instance.InstantiateNetworkObject();
                if( nwObj != null )
                {
                    nwObj.name  = nwObj.name.Replace( "(Clone)", "" );
                    nwObj.transform.SetParent( daemon.transform, false );
                }
            }

            {
                var fadeObj = GameSettingManager.Instance.InstantiateFadeObject();
                if( fadeObj != null )
                {
                    fadeObj.name    = fadeObj.name.Replace( "(Clone)", "" );
                    fadeObj.AddComponent< FadeController >();
                    fadeObj.transform.SetParent( daemon.transform, false );
                }
            }

            {
                var scene   = new GameObject( "SceneController" );
                scene.AddComponent< SceneController >();
                scene.transform.SetParent( daemon.transform, false );
            }
        }

    } // class RuntimeAsura
    
} // namespace HimonoLib

