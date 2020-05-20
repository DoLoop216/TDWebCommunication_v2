using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDWebCommunication
{
    public class TDWebCommunication
    {
        public static string ConnectionString;

        /// <summary>
        /// Log or not to log changes
        /// </summary>
        public static bool LogChanges
        {
            get { return _LogChanges; }
        }
        /// <summary>
        /// Path to log file
        /// </summary>
        public static string LogFile
        {
            get
            {
                return _LogFile;
            }
            set
            {
                _LogFile = value;
                if (_LogFile.Length > 0)
                    _LogChanges = true;
                else
                    _LogChanges = false;
            }
        }
        public static bool Initialized { get { return _Initialized; } }


        /// <summary>
        /// Path to log file
        /// </summary>
        private static string _LogFile = "";
        /// <summary>
        /// Log or not to log changes
        /// </summary>
        private static bool _LogChanges = false;
        private static bool _Initialized = false;

        public TDWebCommunication(string ConnectionString)
        {
            TDWebCommunication.ConnectionString = ConnectionString;

            _Initialized = true;
        }
    }
}
