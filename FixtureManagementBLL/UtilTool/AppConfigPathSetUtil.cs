using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FixtureManagementBLL.UtilTool
{
    public class AppConfigPathSetUtil
    {
        private static string AppConfigPath = "FixtureManagementApp.exe.config";

        public static void AppSetValue(string AppKey, string AppValue)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppConfigPath);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//appSettings");
            XmlElement xmlElement = (XmlElement)xmlNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xmlElement != null)
            {
                xmlElement.SetAttribute("value", AppValue);
            }
            else
            {
                XmlElement xmlElement2 = xmlDocument.CreateElement("add");
                xmlElement2.SetAttribute("key", AppKey);
                xmlElement2.SetAttribute("value", AppValue);
                xmlNode.AppendChild(xmlElement2);
            }
            xmlDocument.Save(AppConfigPath);
        }
    }
}
