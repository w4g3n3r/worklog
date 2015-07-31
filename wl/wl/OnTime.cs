using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxosoftAPI.NET;
using AxosoftAPI.NET.Models;

namespace wl
{
    public class OnTime
    {
        private readonly Proxy ontimeClient;
        private string token;
        private User currentUser;

        public OnTime(string url, string clientId, string clientSecret)
        {
            ontimeClient = new Proxy
            {
                Url = url,
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            token = null;
            currentUser = null;
        }

        public bool Login(string userName, string password)
        {
            try
            {
                token = ontimeClient.ObtainAccessTokenFromUsernamePassword(userName, password, ScopeEnum.ReadWrite);
                var result = ontimeClient.Me.Get();
                if (!result.IsSuccessful) return false;

                currentUser = result.Data;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool CreateWorkLog(WorkLog wl)
        {
            AxosoftAPI.NET.Models.WorkLog log = new AxosoftAPI.NET.Models.WorkLog
            {
                User = currentUser,
                DateTime = wl.Begin.ToUniversalTime(),
                WorkDone = new DurationUnit { TimeUnit = new TimeUnit { Id = 1 }, Duration = (decimal)wl.Minutes },
                Item = new Item { Id = wl.TaskId, ItemType = Enum.GetName(typeof(WorkLogType), wl.Type).ToLower() },
                Description = wl.Message
            };

            var result = ontimeClient.WorkLogs.Create(log);

            return result.IsSuccessful;
        }

        //public IEnumerable<int> GetGenericTasks()
        //{
        //    var releasesResult = ontimeClient.Releases.Get();
        //    if (releasesResult.IsSuccessful)
        //    {
        //        int? releaseId = releasesResult.Data.Where(r => r.Name == "_worklog").Select(r => r.Id);
        //        if (releaseId.HasValue)
        //        {
        //            var parameters = new Dictionary<string, string>();
        //            parameters.Add("release_id", releaseId.Value.ToString());
        //            var featuresResult = ontimeClient.Features.Get(parameters);

        //            if (featuresResult.IsSuccessful
        //        }
        //    }
        //}
    }
}
