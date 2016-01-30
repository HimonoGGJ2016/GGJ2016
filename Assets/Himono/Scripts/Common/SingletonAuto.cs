//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class SingletonAuto<TMyClass > : Singleton< TMyClass >  where TMyClass : class
    {

    #region Constructor

        protected SingletonAuto()
        {
            
        }

    #endregion // Constructor


    #region Property

        public static new TMyClass Instance
        {
            get
            {
                if( Singleton< TMyClass >.Instance == null )
                {
                    CreateSingleton();
                }

                return Singleton< TMyClass >.Instance;
            }
        }

    #endregion // Property

    }
    
} // namespace HimonoLib

