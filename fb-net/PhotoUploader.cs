using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace FbApi
{
    interface IPhotoUploader
    {
        void PhotoIsReady(int uploadId, FbPhoto photo);
        FbPhoto[] WaitFor(int[] uploadIds, TimeSpan timeout);
    }

    class PhotoUploader : IPhotoUploader
    {
        #region members

        class QueItem
        {
            public int uploadId { get; set; }
            public FbPhoto photo { get; set; }
        }

        private readonly BlockingCollection<QueItem> _que;
        private readonly Dictionary<int, FbPhoto> _photos;

        #endregion

        #region construction

        public static IPhotoUploader New()
        {
            return
                new PhotoUploader();
        }

        private PhotoUploader()
        {
            _que = new BlockingCollection<QueItem>();
            _photos = new Dictionary<int, FbPhoto>();
        }

        #endregion

        #region private

        private void takeReadyPhotos(TimeSpan timeout)
        {
            if (!_que.TryTake(out QueItem item, timeout))
                return;

            while (item != null)        // take them all if there are more
            {
                lock (_photos)
                    _photos[item.uploadId] = item.photo;

                _que.TryTake(out item, TimeSpan.Zero);
            }
        }

        FbPhoto[] getPhotos(int[] uploadIds, out int missingId)
        {
            missingId = 0;
            var photos = new List<FbPhoto>();

            foreach (var id in uploadIds)
            {
                FbPhoto photo;
                bool photoIsHere;

                lock (_photos)
                    photoIsHere = _photos.TryGetValue(id, out photo);

                if (!photoIsHere)
                {
                    missingId = id;
                    break;
                }

                photos.Add(photo);
            }

            return 
                photos.ToArray();
        }

        #endregion

        #region interface

        void IPhotoUploader.PhotoIsReady(int uploadId, FbPhoto photo)
        {
            var item = new QueItem {
                uploadId = uploadId,
                photo = photo
            };

            _que.Add(item);
        }

        FbPhoto[] IPhotoUploader.WaitFor(int[] uploadIds, TimeSpan timeout)
        {
            var tm = Stopwatch.StartNew();

            for (;;)
            {
                var photos = getPhotos(uploadIds, out int missingId);

                if (photos.Length == uploadIds.Length)
                    return photos;

                var elapsed = tm.Elapsed;

                if (elapsed > timeout)
                {
                    throw 
                        new ApplicationException($"Timeout ({timeout.TotalSeconds} seconds) while waiting for images to be uploaded; ({missingId} is missing)");
                }

                var leftSpan = timeout - elapsed;
                takeReadyPhotos(leftSpan);
            }
        }

        #endregion
    }
}