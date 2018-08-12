using System;

namespace ThumbnailSrv
{
    interface ITopicLogger
    {
        bool disabled { get; set; }
        void info(string trackingId, Func<string> getMsg, Func<object> getDetails = null);
        void error(string trackingId, Exception error, Func<string> getMsg, Func<object> getDetails = null);
        void error(string trackingId, string errMsg, Func<object> getDetails = null);
    }

    class TopicLogger : ITopicLogger
    {
        #region members

        private readonly string _topic;
        private readonly ILogger _peer;

        private bool _disabled;

        #endregion

        #region construction

        public static ITopicLogger New(string topic, ILogger peer = null)
        {
            return
                new TopicLogger(topic, peer ?? Logger.Instance);
        }

        private TopicLogger(string topic, ILogger peer)
        {
            _topic = topic;
            _peer = peer;
            _disabled = false;
        }

        #endregion

        #region interface

        bool ITopicLogger.disabled
        {
            get => _disabled;
            set => _disabled = value;
        }

        void ITopicLogger.info(string trackingId, Func<string> getMsg, Func<object> getDetails)
        {
            if (_disabled)
                return;

            _peer.info(trackingId, _topic, getMsg, getDetails);
        }

        void ITopicLogger.error(string trackingId, Exception error, Func<string> getMsg, Func<object> getDetails)
        {
            if (_disabled)
                return;

            _peer.error(trackingId, _topic, error, getMsg, getDetails);
        }

        void ITopicLogger.error(string trackingId, string errMsg, Func<object> getDetails)
        {
            if (_disabled)
                return;

            _peer.error(trackingId, _topic, errMsg, getDetails);
        }

        #endregion
    }
}