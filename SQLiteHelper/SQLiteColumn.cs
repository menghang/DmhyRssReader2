namespace System.Data.SQLite
{
    public class SQLiteColumn
    {
        public string ColumnName = "";
        public bool PrimaryKey = false;
        public ColType ColDataType = ColType.Text;
        public bool AutoIncrement = false;
        public bool NotNull = false;
        public string DefaultValue = "";

        public SQLiteColumn()
        { }

        public SQLiteColumn(string colName)
        {
            this.ColumnName = colName;
            this.PrimaryKey = false;
            this.ColDataType = ColType.Text;
            this.AutoIncrement = false;
        }

        public SQLiteColumn(string colName, ColType colDataType)
        {
            this.ColumnName = colName;
            this.PrimaryKey = false;
            this.ColDataType = colDataType;
            this.AutoIncrement = false;
        }

        public SQLiteColumn(string colName, bool autoIncrement)
        {
            this.ColumnName = colName;

            if (autoIncrement)
            {
                this.PrimaryKey = true;
                this.ColDataType = ColType.Integer;
                this.AutoIncrement = true;
            }
            else
            {
                this.PrimaryKey = false;
                this.ColDataType = ColType.Text;
                this.AutoIncrement = false;
            }
        }

        public SQLiteColumn(string colName, ColType colDataType, bool primaryKey, bool autoIncrement, bool notNull, string defaultValue)
        {
            this.ColumnName = colName;

            if (autoIncrement)
            {
                this.PrimaryKey = true;
                this.ColDataType = ColType.Integer;
                this.AutoIncrement = true;
            }
            else
            {
                this.PrimaryKey = primaryKey;
                this.ColDataType = colDataType;
                this.AutoIncrement = false;
                this.NotNull = notNull;
                this.DefaultValue = defaultValue;
            }
        }
    }
}
