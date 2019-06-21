using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIC.Logging.Common
{
    public class LoggingContext : IDisposable
    {
        #region == Fields ==

        /// <summary>
        /// Indicates whether the context was completed
        /// </summary>
        private bool _completed;

        /// <summary>
        /// Indicates whether the context should be automatically completed when disposed
        /// </summary>
        private bool _autoComplete;

        /// <summary>
        /// Used to detect redundant calls
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Synchronization object
        /// </summary>
        private static object _lockSync = new object();

        /// <summary>
        /// Context attributes collection
        /// </summary>
        private Dictionary<string, object> _contextAttributes;

        /// <summary>
        /// Context attributes collection
        /// </summary>
        private Dictionary<string, object> _replacedAttributes;

        /// <summary>
        /// Thread level attributes collection. This member is ThreadStatic.
        /// </summary>
        [ThreadStatic]
        private static Dictionary<string, object> _threadLevelAttributes;

        /// <summary>
        /// App Domain level attributes
        /// </summary>
        private static Dictionary<string, object> _appDomainLevelAttributes;

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        private static List<LoggingContext> _openedContexts;

        #endregion

        #region == Properties ==

        /// <summary>
        /// Gets thread level attributes collection.
        /// </summary>
        public static Dictionary<string, object> ThreadLevelAttributes
        {
            get
            {
                if (_threadLevelAttributes == null)
                    _threadLevelAttributes = new Dictionary<string, object>();

                return _threadLevelAttributes;
            }
        }

        /// <summary>
        /// Gets thread level attributes collection.
        /// </summary>
        public static Dictionary<string, object> AppDomainLevelAttributes
        {
            get
            {
                if (_appDomainLevelAttributes == null)
                    _appDomainLevelAttributes = new Dictionary<string, object>();

                return _appDomainLevelAttributes;
            }
        }

        /// <summary>
        /// Gets thread level attributes collection.
        /// </summary>
        private static List<LoggingContext> OpenedContexts
        {
            get
            {
                if (_openedContexts == null)
                    _openedContexts = new List<LoggingContext>();

                return _openedContexts;
            }
        }

        #endregion

        #region == Constructors ==

        private LoggingContext(Dictionary<string, object> attributes, bool autoComplete)
        {
            _contextAttributes = new Dictionary<string, object>();
            _replacedAttributes = new Dictionary<string, object>();
            _autoComplete = autoComplete;

            Add(attributes);
        }

        public static LoggingContext Start(Dictionary<string, object> attributes, bool autoComplete = false)
        {
            LoggingContext context = new LoggingContext(attributes, autoComplete);
            OpenedContexts.Add(context);

            return context;
        }

        public static LoggingContext Start(string key, string value, bool autoComplete = false)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>()
            {
                { key, value }
            };

            return Start(attributes, autoComplete);
        }

        public void Add(string key, object value)
        {
            if (_contextAttributes.ContainsKey(key))
                return;

            _contextAttributes[key] = value;

            if (ThreadLevelAttributes.ContainsKey(key))
                _replacedAttributes[key] = ThreadLevelAttributes[key];

            ThreadLevelAttributes[key] = value;
        }

        public void Add(Dictionary<string, object> attributes)
        {
            if (attributes == null)
                return;

            foreach (var attribute in attributes)
                Add(attribute.Key, attribute.Value);
        }

        #endregion

        #region == Methods ==

        private void CloseInnerContexts()
        {
            for (int i = _openedContexts.Count; i > 0; i--)
            {
                var context = _openedContexts[i - 1];
                if (context == this)
                    break;

                context.Close();
            }
        }

        private void Close()
        {
            foreach (var attribute in _contextAttributes)
                if (ThreadLevelAttributes.ContainsKey(attribute.Key))
                    ThreadLevelAttributes.Remove(attribute.Key);

            foreach (var attribute in _replacedAttributes)
                ThreadLevelAttributes[attribute.Key] = attribute.Value;

            OpenedContexts.Remove(this);

            if (OpenedContexts.Count == 0)
            {
                ThreadLevelAttributes.Clear();
            }
        }

        #endregion

        #region == IDisposable Implementation ==

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_completed || _autoComplete)
                    {
                        CloseInnerContexts();
                        Close();
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Clears context attributes
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public void Complete()
        {
            _completed = true;
        }

        #endregion
    }
}
