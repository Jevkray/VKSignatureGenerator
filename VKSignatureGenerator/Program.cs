using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VKSign.Request
{
    public class ApiRequestParams : SortedDictionary<string, string>
    {
        private int requestLength = 0;

        public ApiRequestParams()
        {
        }

        public ApiRequestParams(string value)
        {
            foreach (string part in value.Split('&'))
            {
                string[] keyValue = part.Split('=');
                this.Add(keyValue[0], keyValue[1]);
            }
        }

        public int GetRequestLength()
        {
            return this.requestLength;
        }

        public new void Add(string key, string value)
        {
            if (key == null || value == null)
            {
                throw new ArgumentException("Wrong request params");
            }
            requestLength += key.Length + value.Length + 2;
            base.Add(key, value);
        }
    }

    public abstract class BaseRequest
    {
        protected static readonly Encoding defaultEncoding = Encoding.UTF8;
        protected static readonly string applicationKey = "Pnxo7xcRjuXSldIh"; // Collected from official VK android app at res/values/strings.xml -> libverify_application_key
        protected string method;

        public BaseRequest(string method)
        {
            this.method = method;
        }

        public string BuildRequestUrl()
        {
            ApiRequestParams methodParams = GetMethodParams();
            if (methodParams.Count == 0)
            {
                return string.Format("{0}{1}", GetApiHost(), GetApiPath());
            }
            StringBuilder stringBuilder = new StringBuilder(methodParams.GetRequestLength());
            foreach (KeyValuePair<string, string> entry in methodParams)
            {
                if (!IsValid(entry.Value))
                {
                    throw new ArgumentException("Request arguments can't be empty!");
                }
                if (!IsValid(stringBuilder))
                {
                    stringBuilder.Append("&");
                }
                stringBuilder.Append(entry.Key).Append("=").Append(HttpUtility.UrlEncode(entry.Value, defaultEncoding));
            }
            return string.Format("{0}{1}?{2}&signature={3}", GetApiHost(), GetApiPath(), stringBuilder, GetSignature());
        }

        public string GetSignature(ApiRequestParams apiRequestParams)
        {
            SortedSet<string> sortedSet = new SortedSet<string>();
            StringBuilder stringBuilder = new StringBuilder(apiRequestParams.GetRequestLength());
            foreach (KeyValuePair<string, string> entry in apiRequestParams)
            {
                sortedSet.Add(entry.Key + HttpUtility.UrlEncode(entry.Value, defaultEncoding));
            }
            foreach (string set in sortedSet)
            {
                stringBuilder.Append(set);
            }
            return HttpUtility.UrlEncode(ToMD5(method + stringBuilder + string.Format("{0:x}", new BigInteger(defaultEncoding.GetBytes(applicationKey)))), defaultEncoding);
        }

        public string GetSignature()
        {
            return GetSignature(GetMethodParams());
        }

        public abstract ApiRequestParams GetMethodParams();

        public abstract string GetApiHost();

        public abstract string GetApiPath();

        private string ToMD5(string value)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(defaultEncoding.GetBytes(value));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return value;
            }
        }

        private bool IsValid(string value)
        {
            return value != null && value.Length > 0;
        }

        private bool IsValid(StringBuilder stringBuilder)
        {
            return IsValid(stringBuilder.ToString());
        }
    }

    public class PushRequest : BaseRequest
    {
        public PushRequest() : base("pushstatus")
        {
        }

        public override ApiRequestParams GetMethodParams()
        {
            ApiRequestParams apiRequestParams = new ApiRequestParams();
            apiRequestParams.Add("application", "VK");
            apiRequestParams.Add("application_id", "*");
            apiRequestParams.Add("capabilities", "*");
            apiRequestParams.Add("confirm_action", "*");
            apiRequestParams.Add("delivery_method", "*");
            apiRequestParams.Add("device_id", "*");
            apiRequestParams.Add("device_screen_active", "*");
            apiRequestParams.Add("env", "*");
            apiRequestParams.Add("os_version", "*");
            apiRequestParams.Add("platform", "*");
            apiRequestParams.Add("push_token_id", "*");
            apiRequestParams.Add("route_type", "*");
            apiRequestParams.Add("session_id", "*");
            apiRequestParams.Add("status", "*");
            apiRequestParams.Add("system_id", "*");
            apiRequestParams.Add("version", "*");
            apiRequestParams.Add("libverify_build", "231");
            apiRequestParams.Add("libverify_version", "2.0");
            return apiRequestParams;
        }

        public override string GetApiHost()
        {
            return "https://clientapi.mail.ru/".Split(';')[0];
        }

        public override string GetApiPath()
        {
            return string.Format("{0}/{1}", "fcgi-bin", method);
        }
    }
}