﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ThumbnailSrv
{
    interface ILogger
    {
        void info(string trackingId, string topic, Func<string> getMsg, Func<object> getDetails = null);
        void error(string trackingId, string topic, Exception error, string errMsg = null, Func<object> getDetails = null);
        object Dump();
    }

    class Logger : ILogger
    {
        #region config

        public class Config
        {
            public bool Disabled { get; set; } = false;
            public int HistoryDepth { get; set; } = 250;
        }

        #endregion

        #region members

        struct Item
        {
            public string Severity { get; set; }
            public string TrackingId { get; set; }
            public string Topic { get; set; }
            public string Msg { get; set; }
            public object Details { get; set; }
            public Exception Error { get; set; }
        }

        private readonly Config _config;
        private readonly Queue<Item> _que;

        #endregion

        #region singleton

        public static ILogger Instance { get; } = new Logger(new Config());

        public static ILogger New(Config config = null)
        {
            return
                new Logger(config ?? new Config());
        }

        private Logger(Config config)
        {
            _config = config;
            _que = new Queue<Item>(config.HistoryDepth);
        }

        #endregion

        #region private

        private void enqueue(Item item)
        {
            lock (this)
            {
                if (_que.Count >= _config.HistoryDepth)
                    _que.Dequeue();

                _que.Enqueue(item);
            }
        }

        private void push(string severity, string trackingId, string topic, Exception error, 
            Func<string> getMsg, Func<object> getDetails)
        {
            string msg;
            object details = null;

            try
            {
                msg = getMsg();
            }
            catch (Exception ex)
            {
                msg = $"Failed to get a message ({ex.Message})";
            }

            try
            {
                if (getDetails != null)
                    details = getDetails();
            }
            catch
            {
                // nothing to do here
            }

            if (msg.IsEmpty())
            {
                msg = 
                    error != null ? error.Message : "No message is here";
            }

            var item = new Item {
                Severity = "I",
                TrackingId = trackingId,
                Topic = topic,
                Msg = msg,
                Details = details,
                Error = error
            };

            enqueue(item);
        }

        private static object formatMsg(Item item)
        {
            var msg = item.Msg ?? item.Error?.Message ?? "n/a";

            return new {
                Msg = $"{item.Severity} {item.TrackingId} {item.Topic} {msg}",
                Error = item.Error?.Message,
                Stack = item.Error?.StackTrace,
                Details = item.Details
            };
        }

        private static object makeDump(Item[] items)
        {
            var transform =
                from item in items
                select formatMsg(item);

            return new {
                Logger = transform.ToArray()
            };
        }

        #endregion

        #region interface

        void ILogger.info(string trackingId, string topic, Func<string> getMsg, Func<object> getDetails)
        {
            if (_config.Disabled)
                return;

            if (getMsg == null)
                return;

            push("I", trackingId, topic, null, getMsg, getDetails);
        }

        void ILogger.error(string trackingId, string topic, Exception error, string errMsg, Func<object> getDetails)
        {
            push("E", trackingId, topic, error, () => errMsg, getDetails);
        }

        object ILogger.Dump()
        {
            Item[] items;

            lock (this)
            {
                items = _que.Reverse().ToArray();
            }

            return
                makeDump(items);
        }

        #endregion
    }
}