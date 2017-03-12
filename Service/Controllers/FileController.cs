using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Entities;
using Logic;
using Newtonsoft.Json.Linq;
using NLog;

namespace Service.Controllers
{
    public class FileController : ApiController
    {
        private FileLogic logic;
        private Logger logger;

        public FileController()
        {
            logic = new FileLogic();
            logger = LogManager.GetCurrentClassLogger();
        }

        public HttpResponseMessage Post()
        {
            try
            {
                //Get json string from body of request
                string json = Request.Content.ReadAsStringAsync().Result;
                //Replace single char '\' on double char '\\' for correct work JObject.Parse
                json = Regex.Replace(json, @"([^\\])\\([^\\])", m => m.Groups[1].Value + "\\\\" + m.Groups[2].Value);
                var jsonData = JObject.Parse(json);
                string path = jsonData["Path"].ToString();
                string format = jsonData["Format"].ToString();

                logger.Info("The request came. Method: '{0}', Request path: '{1}', File path: '{2}', File format: '{3}'",
                    Request.Method,
                    Request.RequestUri.AbsolutePath,
                    path,
                    format);

                var fileName = Path.GetFileNameWithoutExtension(path);
                var repoPath = GetRepoPath(fileName, format);

                logic.ExportToFormat(path, repoPath, format);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, e.Message);
            }
        }

        public HttpResponseMessage Get(string fileName, string format)
        {
            logger.Info("The request came. Method: '{0}', Request path: '{1}', File name: '{2}', File format: '{3}'",
                Request.Method,
                Request.RequestUri.AbsolutePath,
                fileName,
                format);

            var path = GetRepoPath(fileName, format);

            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(logic.GetStream(path));
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName + "." + format;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/" + format);

                return response;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, e.Message);
            }
        }

        public HttpResponseMessage GetById(string fileName, int id)
        {
            logger.Info("The request came. Method: '{0}', Request path: '{1}', File name: '{2}', Id: '{3}'", 
                Request.Method,
                Request.RequestUri.AbsolutePath,
                fileName,
                id);

            var path = GetRepoPath(fileName);

            try
            {
                return Request.CreateResponse<TradeRecord>(HttpStatusCode.OK, logic.GetById(path, id));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, e.Message);
            }

        }

        public HttpResponseMessage Delete(string fileName, string format)
        {
            logger.Info("The request came. Method: '{0}', Request path: '{1}', File name: '{2}', File format: '{3}'",
                Request.Method,
                Request.RequestUri.AbsolutePath,
                fileName,
                format);

            var path = GetRepoPath(fileName, format);

            try
            {
                logic.Delete(path);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, e.Message);
            }
        }

        private string GetRepoPath(string fileName, string format = "db")
        {
            return "repository/" + fileName + "." + format;
        }
    }
}
