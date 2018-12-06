using ClosedXML.Excel;
using System.IO;
using System.Web;

namespace TimeTracker.UI.Areas.OrgAdmins.Tools
{
    public class ExportTools
    {
        public void DownloadExcelFile(string fileName, XLWorkbook workbook, HttpResponseBase response)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("content-disposition", "attachment;filename=" + fileName);

                workbook.SaveAs(stream);
                stream.WriteTo(response.OutputStream);
                response.Flush();
                response.End();
            }
        }
    }
}