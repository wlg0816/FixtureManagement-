using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.UtilTool
{
    public class HttpClientUtil
    {
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>
        /// 创建GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetRequest(string url)
        {
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();
            url=configPathSetService.getAppConfigValue("seviceUrl")+ url;
            //配置SSL
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                | SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;  //超时时间
            request.KeepAlive = true; //解决GetResponse操作超时问题
            request.Method = "GET";
            request.ContentType = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            string result = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream rs = response.GetResponseStream();
                StreamReader sr = new StreamReader(rs, Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                rs.Close();
            }
            catch(Exception ex)
            {
                NetLogUtil.WriteTextLog("HTTP_Get", ex.Message);
            }
            
            return result;
        }
        /// <summary>
        /// 创建POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetPost(string url, string param)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            //创建post请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";//请求方式post
            request.ContentType = "application/json;charset=UTF-8";//链接类型
            byte[] payload = Encoding.UTF8.GetBytes(param);//参数编码
            request.ContentLength = payload.Length;

            //发送post的请求,写入参数
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            string value = "";
            try
            {
                //接受返回来的数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                value = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                NetLogUtil.WriteTextLog("HTTP_Post", ex.Message);
            }

            return value;
        }
    }
}
