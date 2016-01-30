//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public class Singleton< TMyClass > where TMyClass : class
    {

    #region Constructor

        protected Singleton()
        {
            
        }
        

    #endregion // Constructor


    #region Variable

        private static TMyClass    s_instance = null;

    #endregion // Variable
    

    #region Property

        public static TMyClass Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                s_instance  = value;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return Instance != null;
            }
        }

    #endregion // Property
    

    #region Public

        public static void CreateSingleton()
        {
            if( s_instance == null )
            {
                // where new()を使ってnewでインスタンス化しようとしても
                // privateなコンストラクタはジェネリックで使用できない(error CS0310)ためCreateInstanceを使用する
                //s_instance = new TMyClass();
                s_instance = System.Activator.CreateInstance( typeof( TMyClass ), true ) as TMyClass;
            }
        }

    #endregion // Public


    }
    
} // namespace HimonoLib

