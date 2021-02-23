using System.Collections.Generic;

namespace System.Data.SQLite
{

    public class SQLiteColumnList : IList<SQLiteColumn>
    {
        private readonly List<SQLiteColumn> _lst = new List<SQLiteColumn>();

        private void CheckColumnName(string colName)
        {
            for (int i = 0; i < this._lst.Count; i++)
            {
                if (this._lst[i].ColumnName == colName)
                {
                    throw new Exception("Column name of \"" + colName + "\" is already existed.");
                }
            }
        }

        public int IndexOf(SQLiteColumn item) => this._lst.IndexOf(item);

        public void Insert(int index, SQLiteColumn item)
        {
            CheckColumnName(item.ColumnName);

            this._lst.Insert(index, item);
        }

        public void RemoveAt(int index) => this._lst.RemoveAt(index);

        public SQLiteColumn this[int index]
        {
            get => this._lst[index];
            set
            {
                if (this._lst[index].ColumnName != value.ColumnName)
                {
                    CheckColumnName(value.ColumnName);
                }

                this._lst[index] = value;
            }
        }

        public void Add(SQLiteColumn item)
        {
            CheckColumnName(item.ColumnName);

            this._lst.Add(item);
        }

        public void Clear() => this._lst.Clear();

        public bool Contains(SQLiteColumn item) => this._lst.Contains(item);

        public void CopyTo(SQLiteColumn[] array, int arrayIndex) => this._lst.CopyTo(array, arrayIndex);

        public int Count => this._lst.Count;

        public bool IsReadOnly => false;

        public bool Remove(SQLiteColumn item) => this._lst.Remove(item);

        public IEnumerator<SQLiteColumn> GetEnumerator() => this._lst.GetEnumerator();

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() => this._lst.GetEnumerator();
    }

}
