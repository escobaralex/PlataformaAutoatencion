using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Api.DBUtils
{
    public class QueryXML
    {
        private static QueryXML instance = null;
        protected QueryXML() { }
        public XmlDocument Doc { get; set; }
        public static QueryXML Instance
        {
            get
            {
                if (instance == null)
                    instance = new QueryXML();
                instance.Doc = new XmlDocument();
                instance.Doc.Load(System.Configuration.ConfigurationManager.AppSettings["querys"]);

                return instance;
            }
        }
    }
}