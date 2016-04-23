﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DatabaseAdapter;

namespace DatabaseAdapter
{
    public class XmlDatabaseAdapter: IDatabaseAdapter
    {
        string filename;
        XDocument doc;
        IEnumerable<XElement> data;
        int iRow;
        DataTable dataTable;

        public XmlDatabaseAdapter()
        {
            knownFileTypes["XML documents"] = "*.xml";
           
        }

        public string FileName { get { return filename; }  set { filename = value; } }

        public Dictionary<string, string> KnownFileTypes
        { get { return knownFileTypes; } }
        private Dictionary<string, string> knownFileTypes = new Dictionary<string, string>();

        public void Connect()
        {
            Connect(FileName);
        }

        
        public void Connect(string ifilename)
        {
           filename = ifilename;
           doc = XDocument.Load(filename);
            data = doc.Descendants();
        }

        public string[] GetTables()
        {
            Connect(FileName);
            List<string> tables = new List<string>();
            //XML First level nodes =  table rows 
            tables.Add(data.First().Value);
            return tables.ToArray();
        }

        public Dictionary<string, int> GetFields(string tablename)
        {
            //XML second level nodes = fields
            Dictionary<string, int> columnNames = new Dictionary<string, int>();
            XElement firstNode = data.First();
            int i = 0;
            foreach (var fieldNode in firstNode.Descendants())
            {
                string fieldName = fieldNode.Value;
                columnNames[fieldName] = i++;

            }
            return columnNames;
        }

        public DbDataReader Execute(string sql)
        {
            throw new NotImplementedException();
        }

        public void SetTable(string tablename)
        {
            //Set variables
            dataTable = new DataTable(tablename);
          
            Dictionary<string, int> fields = GetFields(tablename);
            foreach (var field in fields.Keys)
            {
                dataTable.Columns.Add(field);
            }
        }

        public string NormTableName(string tablename)
        {
            return tablename;
        }

        public void Write(DataTable table)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
           
        }

        public void FirstRecord()
        {
            iRow = 0; 
        }

        public void SeekRecord(int position)
        {
            iRow = position;
           
        }

        public IDataRecord CurrentRecord()
        {
            throw new NotImplementedException();
        }

        public DataRow CurrentRow()
        {
            DataRow row = dataTable.NewRow();

            for (int ic = 0; ic < dataTable.Columns.Count; ic++)
            {
                var colName = dataTable.Columns[ic].ColumnName;
                var dataRow = data.ElementAt(iRow);
                row[ic] = dataRow.Element(colName).Value;
            }
            return row;
        }

        public void UpdateRow(int irow, DataRow row)
        {
            throw new NotImplementedException();
        }

        public bool NextRecord()
        {
            iRow++;
            return iRow < data.Count();
        }

        public void Write(string filename)
        {
            throw new NotImplementedException();
        }

        public int GetRowsCount()
        {
            return data.Count();
        }
    }
}