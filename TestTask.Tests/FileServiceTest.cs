using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace TestTask.Tests
{
    [TestClass]
    public class FileServiceTest
    {
        private FileService service;
        private HttpClient client;
        string url;

        [TestInitialize]
        public void Initialize()
        {
            url = "http://localhost:8090";
            service = new FileService(url);
            client = new HttpClient();
            service.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            service.Stop();
            client.Dispose();
        }

        [TestMethod]
        public void PostTestPositiv()
        {
            string path = "test files/test.dat";
            string fullPath = Path.GetFullPath(path);
            string formatCsv = "csv";
            string formatSQLite = "db";
            BinaryHelper binaryHelper = new BinaryHelper();

            CustomFile file = new CustomFile();
            file.Header = new Header()
            {
                version = 1,
                type = "dat"
            };

            for (int i = 0; i < 10000; i++)
            {
                file.Trades.Add(new TradeRecord()
                {
                    id = i,
                    account = i * 2,
                    volume = i * 2.2,
                    comment = "new"
                });
            }

            binaryHelper.Write(path, file);

            string stringForCsv = string.Format("{{Path: '{0}', Format: '{1}'}}", fullPath, formatCsv);
            string stringForSQLite = string.Format("{{Path: '{0}', Format: '{1}'}}", fullPath, formatSQLite);
            var contentForCsv = new StringContent(stringForCsv, Encoding.UTF8, "application/json");
            var contentForSQLite = new StringContent(stringForSQLite, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> resultPostCsv = client.PostAsync(string.Format("{0}/api/file", url), contentForCsv);
            resultPostCsv.Wait();

            Task<HttpResponseMessage> resultPostSQLite = client.PostAsync(string.Format("{0}/api/file", url), contentForSQLite);
            resultPostSQLite.Wait();

            Assert.AreEqual(HttpStatusCode.OK, resultPostCsv.Result.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, resultPostSQLite.Result.StatusCode);
        }

        [TestMethod]
        public void PostTestNegativ()
        {
            string path = "test files/test.dat";
            string fullPath = Path.GetFullPath(path);
            string pathNonexistent = "test files/nonexistent file.dat";
            string fullPathNonexistent = Path.GetFullPath(pathNonexistent);
            string formatCsv = "csv";
            string formatUnsupported = "pdf";

            string stringForPathNonexistent = string.Format("{{Path: '{0}', Format: '{1}'}}", fullPathNonexistent, formatCsv);
            string stringForFormatUnsupported = string.Format("{{Path: '{0}', Format: '{1}'}}", fullPath, formatUnsupported);
            var contentForPathNonexistent = new StringContent(stringForPathNonexistent, Encoding.UTF8, "application/json");
            var contentForFormatUnsupported = new StringContent(stringForFormatUnsupported, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> resultPostWithPathNonexistent = client.PostAsync(string.Format("{0}/api/file", url), contentForPathNonexistent);
            resultPostWithPathNonexistent.Wait();

            Task<HttpResponseMessage> resultPostWithFormatUnsupported = client.PostAsync(string.Format("{0}/api/file", url), contentForFormatUnsupported);
            resultPostWithFormatUnsupported.Wait();

            Assert.AreEqual(HttpStatusCode.BadRequest, resultPostWithPathNonexistent.Result.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, resultPostWithFormatUnsupported.Result.StatusCode);
        }

        [TestMethod]
        public void GetByIdPositiv()
        {
            string fileName = "test";
            int id = 1;

            Task<HttpResponseMessage> resultGet = client.GetAsync(string.Format("{0}/api/file?fileName={1}&id={2}", url, fileName, id));
            resultGet.Wait();

            Assert.AreEqual(HttpStatusCode.OK, resultGet.Result.StatusCode);
        }

        [TestMethod]
        public void GetByIdNegativ()
        {
            string fileName = "test";
            int id = 1;
            string fileNameNonexistent = "nonexistent file";
            int idNonexistent = -5;

            Task<HttpResponseMessage> resultGetWithNonexistentFile = client.GetAsync(string.Format("{0}/api/file?fileName={1}&id={2}", url, fileNameNonexistent, id));
            resultGetWithNonexistentFile.Wait();

            Task<HttpResponseMessage> resultGetWithNonexistentId = client.GetAsync(string.Format("{0}/api/file?fileName={1}&id={2}", url, fileName, idNonexistent));
            resultGetWithNonexistentId.Wait();

            Assert.AreEqual(HttpStatusCode.BadRequest, resultGetWithNonexistentFile.Result.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, resultGetWithNonexistentId.Result.StatusCode);
        }

        [TestMethod]
        public void DeletePositiv()
        {
            string fileName = "test";
            string formatCsv = "csv";
            string formatSQLite = "db";

            Task<HttpResponseMessage> resultDeleteCsv = client.DeleteAsync(string.Format("{0}/api/file?fileName={1}&format={2}", url, fileName, formatCsv));
            resultDeleteCsv.Wait();

            Task<HttpResponseMessage> resultDeleteSQLite = client.DeleteAsync(string.Format("{0}/api/file?fileName={1}&format={2}", url, fileName, formatSQLite));
            resultDeleteSQLite.Wait();

            Assert.AreEqual(HttpStatusCode.OK, resultDeleteCsv.Result.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, resultDeleteSQLite.Result.StatusCode);
        }

        [TestMethod]
        public void DeleteNegativ()
        {
            string fileNameNonexistent = "nonexistent file";
            string formatUnsupported = "pdf";

            Task<HttpResponseMessage> resultDelete = client.DeleteAsync(string.Format("{0}/api/file?fileName={1}&format={2}", url, fileNameNonexistent, formatUnsupported));
            resultDelete.Wait();

            Assert.AreEqual(HttpStatusCode.BadRequest, resultDelete.Result.StatusCode);
        }
    }
}
