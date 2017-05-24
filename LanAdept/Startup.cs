using LanAdeptData.DAL;
using LanAdeptData.Model.Settings;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

[assembly: OwinStartupAttribute(typeof(LanAdept.Startup))]
namespace LanAdept
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Setting settings = UnitOfWork.Current.SettingRepository.GetCurrentSettings();
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (settings.IsPaypalActive)
            {
                config.AppSettings.Settings["clientId"].Value = settings.PaypalClientId;
                config.AppSettings.Settings["clientSecret"].Value = settings.PaypalSecretId;
            }
            config.Save();
        }
    }
}