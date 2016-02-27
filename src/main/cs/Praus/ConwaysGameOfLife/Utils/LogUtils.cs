using System;
using log4net;

namespace Praus.ConwaysGameOfLife.Utils {
    public static class LogUtils {
        public static ILog GetLogger(this ILogUtils cls) {
            return LogManager.GetLogger(cls.GetType());
        }
    }
    /// <summary>
    /// ILogUtils interface.
    /// Enables the implementing class to use the LogUtis extension
    /// methods. 
    /// </summary>
    public interface ILogUtils {
        // Empty interface just for enabling LogUtils
    }
}

