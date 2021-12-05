using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms
{
    internal class Class1
    {
        public void Whatever()
        {
            WebBrowser wb = new WebBrowser();
            wb.DocumentCompleted += Wb_DocumentCompleted;

            wb.ScriptErrorsSuppressed = true;
            wb.Navigate("http://stackoverflow.com");
        }

        private void Wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var wb = (WebBrowser)sender;

            var html = wb.Document.GetElementsByTagName("HTML")[0].OuterHtml;
            var domd = wb.Document.GetElementById("copyright").InnerText;
            /* ... */
        }
    }
}
