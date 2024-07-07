using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Xsl;
using System.Xml;
using System.Linq;
using System;

namespace XMLtoPDF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }

        public FileResult XMLToPDF()
        {
            string xmlString = System.IO.File.ReadAllText(Server.MapPath("/XML/CDS.xml"));
            string xslPath = Server.MapPath("/XML/CDA.xsl");
            var htmlString = RenderXml(xmlString, xslPath);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            var pdfBytes = htmlToPdf.GeneratePdf(htmlString.ToString());
            string pdfName = RandomNumber() + "_" + DateTime.UtcNow + ".pdf";
            return File(pdfBytes.ToArray(), "application/pdf", pdfName);
        }

        private static HtmlString RenderXml(string xml, string xsltPath)
        {
            XsltArgumentList args = new XsltArgumentList();
            XslCompiledTransform t = new XslCompiledTransform();
            t.Load(xsltPath);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                StringWriter writer = new StringWriter();
                t.Transform(reader, args, writer);
                var htmlString = new HtmlString(writer.ToString());
                return htmlString;
            }
        }

        private static string RandomNumber()
        {
            Random generator = new Random();
            string num = generator.Next(0, 1000000).ToString("D6");
            return num;
        }

    }
}