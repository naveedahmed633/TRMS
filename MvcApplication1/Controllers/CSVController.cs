using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class CSVController : Controller
    {
        //
        // GET: /CSV/

        public ActionResult Index()
        {
            return View();
        }



        private string decodeBase64String(string toDecode)
        {
            try
            {
                byte[] data = Convert.FromBase64String(toDecode);
                string decodedString = Encoding.UTF8.GetString(data);

                //File.WriteAllText("newFile.csv", decodedString);
                return decodedString;
            }
            catch (Exception exception)
            {
                //throw new Exception("Error in base64Encode" + exception.Message);
            }


            return "";

        }

        [Authorize]
        [HttpPost]
        public FileResult getFile()
        {
            string encodedString = Request.Form["base64stream"];
            string nameOfFile = Request.Form["fileName"];



            string decodedString = decodeBase64String(encodedString);

            return File(Encoding.UTF8.GetBytes(decodedString),
                 "text/plain",
                  nameOfFile);

        }

    }
}
