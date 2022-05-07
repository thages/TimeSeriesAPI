using Microsoft.VisualBasic.FileIO;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using System.Data;

namespace ApiBackend.Models
{
    public class InputFile
    {
        private string? name;
        private string? fullPath;
        private string? fileName;
        private DataTable? timeSeries;
        public InputFile (IFormFile file, string newPath) 
        {
            try
            {

                fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString();
                name = fileName.Replace("\"", "");
                fullPath = Path.Combine(newPath, name);
                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);
                timeSeries = GetDataTableFromCSVFile(fullPath);
            }
            catch (Exception except)
            {
                throw new ArgumentException(except.Message);
            }
        }
        public static DataTable GetDataTableFromCSVFile(string filepath)
        {
            // TODO: Create input form columns validation.

            DataTable csvData = new();

            try
            {
                using TextFieldParser csvReader = new(filepath);
                csvReader.SetDelimiters(new string[] { ";" });
                csvReader.HasFieldsEnclosedInQuotes = false;

                string[]? colFields = csvReader.ReadFields();
                if (colFields != null)
                {
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                }


                while (!csvReader.EndOfData)
                {

                    string[]? fieldData = csvReader.ReadFields();
                    if (fieldData != null)
                    {
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception except)
            {
                throw new ArgumentException(except.Message);
            }

            return csvData;
        }
        public string getResponse()
        {
            try
            {
                if (timeSeries == null)
                {
                    throw new ArgumentException("Time Series table is null");
                }

                string? parKey;
                List<Dictionary<string, object>>? parentRow = new();
                Dictionary<string, object> childRow;
                foreach (DataRow row in timeSeries.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in timeSeries.Columns)
                    {
                        int pos = col.ColumnName.IndexOf("/");
                        if (pos > 0)
                        {
                            parKey = col.ColumnName[..pos];
                        }
                        else
                        {
                            parKey = col.ColumnName;
                        }

                        childRow.Add(parKey.ToUpper(), row[col]);
                    }
                    parentRow.Add(childRow);
                }

                string value = JsonSerializer.Serialize(parentRow);
                return value;
            
            }
            catch(Exception except)
            {
                throw new ArgumentException(except.Message);
            }

        }
    }
}
