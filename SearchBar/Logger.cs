using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using System.Reflection;

namespace LibronToolbar
{
    /// <summary>
    /// This class is used for logging application info and errors.
    /// </summary>
    public sealed class Logger
    {
        #region Ctor(s)
        static Logger()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string filePath = Path.GetDirectoryName(executingAssembly.Location);
            string asmName = Path.GetFileName(executingAssembly.Location);
            string configFileName = Path.Combine(filePath, asmName + ".config");
            System.Diagnostics.Debug.WriteLine("SearchBar configuration file path." + configFileName);
            XmlConfigurator.Configure(new FileInfo(configFileName));
        }

        #endregion

        #region Public Methods
       /// <summary>
        /// The name of the logger to get, usually this
        /// will be the type name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            ILog log = LogManager.GetLogger(name);
            return log;
        }
    
        /// <summary>
        /// Get a logger based on the type name, the type name is used
        /// to infer the name of the logger
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            ILog log = LogManager.GetLogger(type);
            return log;
        }
        #endregion
    }

}
