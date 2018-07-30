using System;
using System.Collections.Generic;

namespace FbApi
{
    interface IAppRepo
    {
        IFacebookApp GetApp(string sessionId);
        bool AddApp(string sessionId, IFacebookApp app);
    }

    class AppRepo : IAppRepo
    {
        #region members

        private readonly Dictionary<string, IFacebookApp> _db;

        #endregion

        #region construction

        public static IAppRepo New()
        {
            return 
                new AppRepo();
        }

        private AppRepo()
        {
            _db = new Dictionary<string, IFacebookApp>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region interface

        IFacebookApp IAppRepo.GetApp(string sessionId)
        {
            if (_db.TryGetValue(sessionId, out var app))
                return app;
            return null;
        }

        bool IAppRepo.AddApp(string sessionId, IFacebookApp app)
        {
            if (_db.ContainsKey(sessionId))
                return false;

            _db.Add(sessionId, app);
            return true;
        }

        #endregion
    }

    class SyncRepo : IAppRepo
    {
        private readonly object _mutex = new Object();
        private readonly IAppRepo _;

        public static IAppRepo New(IAppRepo target) { return new SyncRepo(target); }
        private SyncRepo(IAppRepo target) { _ = target; }

        IFacebookApp IAppRepo.GetApp(string sessionId) { lock (_mutex) return _.GetApp(sessionId); }
        bool IAppRepo.AddApp(string sessionId, IFacebookApp app) { lock (_mutex) return _.AddApp(sessionId, app); }
    }
}