using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataObjectProcessor.CsvProcessor
{
    public class CsvDataProcessor : IDataProcessor
    {
        private readonly CsvProcessorOptions _options;
        private readonly string _csvFileLocation;

        public CsvDataProcessor(
            string csvFileLocation,
            CsvProcessorOptions? options = null)
        {
            _options = (options is null) ? new CsvProcessorOptions() : options;
            _csvFileLocation = csvFileLocation;

            if (!File.Exists(_csvFileLocation))
                throw new FileNotFoundException($"Unable to find file '{_csvFileLocation}'");

            if (!File.OpenRead(_csvFileLocation).CanRead)
                throw new AccessViolationException($"Unable to open file '{_csvFileLocation}', access was denied.");
        }

        public int RowNumber { get; internal set; } = 0;

        public async Task<T[]> GetArrayAsync<T>()
        {
            List<T> values = new List<T>();
            await foreach (var i in GetAsyncEnumerable<T>())
                values.Add(i);
            return values.ToArray();
        }

        public async IAsyncEnumerable<T> GetAsyncEnumerable<T>()
        {
            RowNumber = (_options.FirstRowHasHeadings) ? 1 : 0;

            using (StreamReader streamReader = new StreamReader(_csvFileLocation))
            {
                if (_options.FirstRowHasHeadings)
                    SetupPropertyIndex<T>(await streamReader.ReadLineAsync());
                else
                    SetupPropertyIndex<T>();

                while (!streamReader.EndOfStream)
                {
                    RowNumber++;

                    T returnObect = Activator.CreateInstance<T>();
                    var csvValues = GetCsvValuesFromString(await streamReader.ReadLineAsync());

                    foreach (var i in PropertyIndex)
                    {
                        try
                        {
                            Type t = Nullable.GetUnderlyingType(i.Value.PropertyType) ?? i.Value.PropertyType;
                            object? safeValue = (string.IsNullOrEmpty(csvValues[i.Key]) || string.Equals(csvValues[i.Key], "\"\"")) ? null : Convert.ChangeType(csvValues[i.Key], t);
                            i.Value.SetValue(returnObect, safeValue, null);
                        }
                        catch (Exception ex)
                        {
                            throw new PropertyConversionException($"Unable to set property '{i.Value.Name}', with value '{csvValues[i.Key]}'. {ex.Message}");
                        }
                    }

                    yield return returnObect;
                }
            }
        }

        public async Task<IEnumerable<T>> GetEnumerableAsync<T>()
        {
            List<T> values = new List<T>();
            await foreach (var i in GetAsyncEnumerable<T>())
                values.Add(i);
            return values;
        }

        private Dictionary<int, PropertyInfo> PropertyIndex { get; set; } = new Dictionary<int, PropertyInfo>();

        private void SetupPropertyIndex<T>(string? CsvFirstLine = null)
        {
            PropertyIndex = new Dictionary<int, PropertyInfo>();

            Dictionary<string, int>? columnIndex = new Dictionary<string, int>();
            if (CsvFirstLine is not null)
            {
                var csvItems = GetCsvValuesFromString(CsvFirstLine);
                for (int i = 0; i < csvItems.Length; i++)
                    columnIndex.Add(csvItems[i], i);
            }


            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanWrite)
                    continue;

                if (prop.CustomAttributes.Any(o => o.AttributeType == typeof(CsvIndexAttribute)) &&
                    !PropertyIndex.TryAdd(prop.GetCustomAttribute<CsvIndexAttribute>().ColumnIndex, prop))
                {
                    var index = prop.GetCustomAttribute<CsvIndexAttribute>().ColumnIndex;
                    var propOne = PropertyIndex[prop.GetCustomAttribute<CsvIndexAttribute>().ColumnIndex].Name;
                    var propTwo = prop.Name;
                    throw new Exceptions.PropertyIndexException($"Index '{index}' is assigned to both '{propOne}' and '{propTwo}'.");
                }
                else
                {
                    if (columnIndex.ContainsKey(prop.Name) &&
                    !PropertyIndex.ContainsKey(columnIndex[prop.Name]))
                        PropertyIndex.Add(columnIndex[prop.Name], prop);
                }
            }
        }

        private string[] GetCsvValuesFromString(string InputString) =>
            (_options.FieldsAreEnclosedInQuotes)
                ? Regex.Split(InputString, $"{_options.ColumnSeperator}(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                : InputString
                        .Split(_options.ColumnSeperator)
                        .ToArray();

    }
}
