using System;
using System.Text;

namespace MobilePozitivApp
{
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "MobileAppPozitivSoapBinding", Namespace = "http://1c.pozitivtelecom.ru")]
    public class DataSetWS : MobileAppPozitiv.MobileAppPozitiv
    {
        private string Login;
        private string Password;

        public DataSetWS()
        {
            Login = AppVariable.Variable.Login;
            Password = AppVariable.Variable.Password;
            Url = AppVariable.Variable.WsURL;
        }

        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(Login + ":" + Password)));
            return request;
        }
    }
}