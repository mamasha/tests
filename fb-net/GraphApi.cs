using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FbApi
{
    interface IGraphApi
    {
        string Code2Token(string code);
        FbUser Me(string token);
        FbPhoto UploadPhoto(PhotoRqst photo, string token, Stream fileStream);
        FbPost PublishPost(PostRqst post, FbPhoto[] photos, string token);
    }

    class GraphApi : IGraphApi
    {
        #region members

        private readonly FbConfig _config;
        private readonly IWebApi _web;

        #endregion

        #region construction

        public static IGraphApi New(FbConfig config, IWebApi web)
        {
            return 
                new GraphApi(config, web);
        }

        private GraphApi(FbConfig config, IWebApi web)
        {
            _config = config;
            _web = web;
        }

        #endregion

        #region private

        private void onFbError(WebApiExceptionDetail detail)
        {
            var sb = new StringBuilder("Bad HTTP status while calling GraphApi");
            FbError fb = null;

            try
            {
                fb = detail.response.text.Parse<FbError>();
                sb.Append($"[GraphApi: {fb.error.message}]");
            }
            catch
            {}

            var message = sb.ToString();

            var ex = new WebApiException(message, detail);

            if (fb != null)
                ex.AddDetail("GraphApi", fb);

            throw ex;
        }

        #endregion

        #region interface

        string IGraphApi.Code2Token(string code)
        {
            var fb = _config.ApiBaseUrl;
            var appId = _config.AppId;
            var redirectUrl = _config.RedirectUrl;
            var appSecret = _config.AppSecret;

            var url = $"{fb}/oauth/access_token?client_id={appId}&redirect_uri={redirectUrl}&client_secret={appSecret}&code={code}";

            var result = _web.Get(url, onFbError, new {
                access_token = "",
                token_type = ""
            });

            return 
                result.access_token;
        }

        FbUser IGraphApi.Me(string token)
        {
            var fb = _config.ApiBaseUrl;

            var me = _web.Get($"{fb}/me?access_token={token}", onFbError, new {
                name = "",
                id = ""
            });

            var accounts = _web.Get($"{fb}/me/accounts/?access_token={token}", onFbError, new {
                data = new[] {
                    new {
                        name = "",
                        id = "",
                        perms = new string[0],
                        access_token = ""
                    }
                }
            });

            var pages =
                from page in accounts.data
                select new FbPage {
                    Name = page.name,
                    Id = page.id,
                    Permissions = page.perms,
                    Token = page.access_token
                };

            var user = new FbUser {
                UserName = me.name,
                UserId = me.id,
                Pages = pages.ToArray()
            };

            return user;
        }

        FbPhoto IGraphApi.UploadPhoto(PhotoRqst photo, string token, Stream fileStream)
        {
            var fb = _config.ApiBaseUrl;
            var pageId = photo.pageId;

            var url = $"{fb}/{pageId}/photos?published=false&access_token={token}";

            using (var mcontent = new MultipartFormDataContent("---------------------------" + DateTime.Now.Ticks.ToString("x")))
            {
                var fcontent = new StreamContent(fileStream);
                fcontent.Headers.ContentType = new MediaTypeHeaderValue(photo.fileType);
                mcontent.Add(fcontent, "source", photo.fileName);

                var info = _web.Post(url, onFbError, mcontent, new {
                    id = ""
                });

                return new FbPhoto {
                    Id = info.id
                };
            }
        }

        FbPost IGraphApi.PublishPost(PostRqst post, FbPhoto[] photos, string token)
        {
            var sb = new StringBuilder(512);

            var fb = _config.ApiBaseUrl;
            var pageId = post.pageId;
            var message = post.message;

            sb.Append($"{fb}/{pageId}/feed?method=post&message={message}&access_token={token}");

            for (int index = 0; index < photos.Length; index++)
            {
                var fbid = photos[index].Id;
                sb
                    .Append("&")
                    .Append($"attached_media[{index}]=")
                    .Append($"{{\"media_fbid\":\"{fbid}\"}}");
            }

            var url = sb.ToString();

            var info = _web.Get(url, onFbError, new {
                id = ""
            });

            return new FbPost {
                Id = info.id
            };
        }

        #endregion
    }
}