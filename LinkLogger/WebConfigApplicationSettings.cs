using System.Configuration;

namespace LinkLogger
{
    public class WebConfigApplicationSettings : IApplicationSettings
    {
        private string _postLinkAccessToken;

        public string PostLinkAccessToken
        {
            get
            {
                return _postLinkAccessToken ??
                       (_postLinkAccessToken = ConfigurationManager.AppSettings["PostLinkAccessToken"]);
            }
        }
    }
}