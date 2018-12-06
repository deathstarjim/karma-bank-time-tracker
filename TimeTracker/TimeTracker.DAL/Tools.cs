using System.Data;

namespace TimeTracker.DAL
{
    public class Tools
    {
        public static bool DataTableHasRows(DataTable result)
        {
            return result.Rows.GetEnumerator().MoveNext();
        }

    }
}
