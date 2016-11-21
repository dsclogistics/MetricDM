using MetricDM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MetricDM.AppCode
{
    public class DataRetrieval
    {
        private string api_url = AppCode.Util.getAPIurl();

        //*******************************************************************************************
        //***************************    API HELPER FUNCTION   **************************************
        //** Just pass an endpoint and a payload to post and this helper will fetch the JSON data  **
        //*******************************************************************************************
        public static string executeAPI(string endPoint, string payload = "")
        {
            //If the Payload Parameter is empty we assume the API call is a GET, else it's a POST
            WebRequest request = WebRequest.Create(AppCode.Util.getAPIurl() + endPoint);        //Using Parameter passed
            request.Method = String.IsNullOrEmpty(payload) ? "GET" : "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = payload;                                    //Using Parameter passed
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        //***********************************************************************************
    }
}