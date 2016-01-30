﻿//------------------------------------------------------------------------
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
            var fadeObj = GameSettingManager.Instance.InstantiateFadeObject();
            if( fadeObj == null )
            {
                return;
                
            }

            var fade    = fadeObj.AddComponent< FadeController >();

            var sceneObj    = new GameObject( "SceneManager", typeof( SceneController ) );
        }

    } // class RuntimeAsura
    
} // namespace HimonoLib

