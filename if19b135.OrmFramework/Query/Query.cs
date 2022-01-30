using System;
using System.Collections;
using System.Collections.Generic;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Query
{
    public sealed class Query<T> : IEnumerable<T>
    {
        private Query<T> _Previous;
        private QueryOperation _Operation = QueryOperation.NO_OPERATION;
        private object[] _Arguments = null;

        private List<T> _InternalValues = null;

        internal Query(Query<T> prev)
        {
            _Previous = prev;
        }

        private void _Fill(Type t)
        {
            List<Query<T>> operations = new List<Query<T>>();
            // reverse order
            Query<T> q = this;
            while (q != null)
            {
                operations.Insert(0, q);
                q = q._Previous;
            }

            Entity entity = t.GetEntity();

            string sql = entity.GetSql();

            List<Tuple<string, object>> parameters = new List<Tuple<string, object>>();
            bool not = false;
            string conj = " WHERE ";
            string openBrackets = "";
            string closeBrackets = "";
            int n = 0;
            string op;
            ICollection<object> localCache = null;
            Field field;

            foreach (Query<T> query in operations)
            {
                switch (query._Operation)
                {
                    case QueryOperation.OR:
                        if (conj != " WHERE ")
                        {
                            conj = " OR ";
                        }

                        break;
                    case QueryOperation.NOT:
                        not = true;
                        break;
                    case QueryOperation.BEGIN_GROUP:
                        openBrackets += "(";
                        break;
                    case QueryOperation.END_GROUP:
                        closeBrackets += ")";
                        break;
                    case QueryOperation.EQUALS:
                    case QueryOperation.LIKE:
                        field = entity.GetFieldByName((string)query._Arguments[0]);

                        if (query._Operation == QueryOperation.LIKE)
                        {
                            op = not ? " NOT LIKE " : " LIKE ";
                        }
                        else
                        {
                            op = not ? " := " : " = ";
                        }

                        sql += closeBrackets + conj + openBrackets;
                        sql += ((bool)query._Arguments[2]
                                   ? $"Lower({field.ColumnName})"
                                   : field.ColumnName)
                               + op +
                               ((bool)query._Arguments[2]
                                   ? $"Lower(:p{n})"
                                   : $":p{n}");
                        parameters.Add(new Tuple<string, object>($":p{n++}",
                            field.ToColumnType(query._Arguments[1])));
                        openBrackets = "";
                        closeBrackets = "";
                        conj = " AND ";
                        not = false;
                        break;
                    case QueryOperation.IN:
                        field = entity.GetFieldByName((string)query._Arguments[0]);
                        sql += closeBrackets + conj + openBrackets;
                        sql += field.ColumnName + (not ? " NOT IN (" : " IN (");
                        for (int i = 1; i < query._Arguments.Length; i++)
                        {
                            if (i >= 2)
                            {
                                sql += ", ";
                            }

                            sql += $":p{n}";
                            parameters.Add(new Tuple<string, object>($"p{n++}",
                                field.ToColumnType(query._Arguments[i])));
                        }

                        sql += ")";
                        openBrackets = "";
                        closeBrackets = "";
                        conj = " AND ";
                        not = false;
                        break;
                    case QueryOperation.GREATER_THAN:
                    case QueryOperation.LESS_THAN:
                        field = entity.GetFieldByName((string)query._Arguments[0]);
                        if (query._Operation == QueryOperation.GREATER_THAN)
                        {
                            op = (not ? " <= " : " > ");
                        }
                        else
                        {
                            op = (not ? " >= " : " < ");
                        }

                        sql += closeBrackets + conj + openBrackets;

                        sql += $"{field.ColumnName}{op}:p{n}";
                        parameters.Add(new Tuple<string, object>($"p{n++}",
                            field.ToColumnType(query._Arguments[1])));
                        openBrackets = "";
                        closeBrackets = "";
                        conj = " AND ";
                        not = false;
                        break;
                }

                Orm._FillList(t, _InternalValues, sql, parameters, localCache);
            }
        }

        private List<T> _Values
        {
            get
            {
                if (_InternalValues == null)
                {
                    _InternalValues = new List<T>();
                    if (typeof(T).IsAbstract)
                    {
                        // ICollection<object> localCache = null;
                        foreach (Type type in typeof(T).GetChildTypes())
                        {
                            _Fill(type);
                        }
                    }
                    else
                    {
                        _Fill(typeof(T));
                    }
                }

                return _InternalValues;
            }
        }


        private Query<T> _SetOp(QueryOperation operation, params object[] args)
        {
            _Operation = operation;
            _Arguments = args;

            return new Query<T>(this);
        }

        public Query<T> Not()
        {
            return _SetOp(QueryOperation.NOT);
        }

        public Query<T> And()
        {
            return _SetOp(QueryOperation.AND);
        }

        public Query<T> Or()
        {
            return _SetOp(QueryOperation.OR);
        }

        public Query<T> BeginGroup()
        {
            return _SetOp(QueryOperation.BEGIN_GROUP);
        }

        public Query<T> EndGroup()
        {
            return _SetOp(QueryOperation.END_GROUP);
        }

        public Query<T> Equals(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOperation.EQUALS, field, value, ignoreCase);
        }

        public Query<T> Like(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOperation.LIKE, field, value, ignoreCase);
        }

        public Query<T> Like(string field, params object[] values)
        {
            List<object> list = new List<object>(values);
            list.Insert(0, field);
            return _SetOp(QueryOperation.IN, list.ToArray());
        }

        public Query<T> GreaterThan(string field, object value)
        {
            return _SetOp(QueryOperation.GREATER_THAN, field, value);
        }

        public Query<T> LessThan(string field, object value)
        {
            return _SetOp(QueryOperation.LESS_THAN, field, value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}