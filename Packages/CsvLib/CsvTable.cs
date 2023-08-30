using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CsvLib
{
    /// <summary>
    /// Simple implementation of CSV parsing/serialization.
    /// Partially follows RFC-4180 (CSV file) specification: https://www.rfc-editor.org/rfc/rfc4180
    /// </summary>
    public class CsvTable
    {
        public class Row
        {
            private List<string> _columns;

            public int length => _columns.Count;
            public string this[int index]
            {
                get => _columns[index];
                set => _columns[index] = value;
            }

            public Row()
            {
                _columns = new List<string>();
            }

            public void Add(string item) => _columns.Add(item);
            public void Clear() => _columns.Clear();

            internal void WriteToStream(StreamWriter writer)
            {
                for (int i = 0; i < _columns.Count; i++)
                {
                    writer.Write(_columns[i]);
                    if (i < _columns.Count - 1)
                        writer.Write(',');
                }
                writer.Write("\r\n"); // Enforce CRLF
            }
        }

        public int rowCount => _rows.Count;
        public int columnCount => _headerRow.length;
        public Row this[int index] => _rows[index];

        private Row _headerRow;
        private List<Row> _rows;

        public CsvTable()
        {
            _headerRow = new Row();
            _rows = new List<Row>();
        }

        public CsvTable(string filePath) : this()
        {
            ReadFromFile(filePath);
        }

        public string GetColumnName(int index)
        {
            return _headerRow[index];
        }

        /// <summary>
        /// Searches for the zero-based column index by column name (case-sensitive). Returns -1 if the name is not found.
        /// </summary>
        public int GetColumnIndex(string name)
        {
            for (int x = 0; x < _headerRow.length; x++)
            {
                if (_headerRow[x] == name)
                    return x;
            }

            return -1;
        }

        public void Clear()
        {
            _headerRow.Clear();

            foreach (Row row in _rows)
                row.Clear();
            _rows.Clear();
        }

        /// <summary>
        /// Clears and populates a table from a CSV text file.
        /// </summary>
        public void ReadFromFile(string filePath)
        {
            Clear();

            using (var reader = new StreamReader(filePath))
            {
                bool isHeaderRow = true;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    Row targetRow;
                    if (isHeaderRow)
                    {
                        targetRow = _headerRow;
                    }
                    else
                    {
                        targetRow = new Row();
                        _rows.Add(targetRow);
                    }

                    // Use regex matching to work properly with quoted values (such as needed when values contain commas).
                    Regex regexParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    Match match = regexParser.Match(line, 0);

                    // Parse comma-separated values in row without use of string.Split.
                    // If there are more columns than what the header row defines, ignore the extra columns.
                    int valueStart = 0, valueEnd = 0;
                    while (valueEnd < line.Length && (isHeaderRow || targetRow.length < columnCount))
                    {
                        int nextComma = -1;
                        if (match.Success)
                        {
                            nextComma = match.Index;
                            match = match.NextMatch();
                        }
                        valueEnd = (nextComma > -1) ? nextComma : line.Length;
                        string value = line.Substring(valueStart, valueEnd - valueStart);
                        targetRow.Add(value);
                        valueStart = valueEnd + 1;
                    }

                    // Fill missing columns (if any) with blank values.
                    if (!isHeaderRow && targetRow.length < columnCount)
                    {
                        for (int i = 0; i < columnCount - targetRow.length; i++)
                            targetRow.Add(null);
                    }

                    isHeaderRow = false;
                }
            }
        }

        public void WriteToFile(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                _headerRow.WriteToStream(writer);
                for (int i = 0; i < _rows.Count; i++)
                    _rows[i].WriteToStream(writer);
            }
        }
    }
}