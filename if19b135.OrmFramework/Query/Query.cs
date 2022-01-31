using System;
using System.Collections;
using System.Collections.Generic;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Query
{
    /// <summary>
    /// Fluent implementation for queries. The results are lazily loaded
    /// </summary>
    /// <typeparam name="T">Type to query</typeparam>
    public sealed class Query<T> : IEnumerable<T>
    {
        /// <summary>
        /// Previous query element
        /// </summary>
        private Query<T> _Previous;

        /// <summary>
        /// Operation of the query
        /// </summary>
        private QueryOperation _Operation = QueryOperation.NO_OPERATION;

        /// <summary>
        /// arguments of the query
        /// </summary>
        private object[] _Arguments = null;

        /// <summary>
        /// Result of the query
        /// </summary>
        private List<T> _InternalValues = null;


        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="prev">Previous query</param>
        internal Query(Query<T> prev)
        {
            _Previous = prev;
        }

        /// <summary>
        /// Fills the Result list of this query
        /// </summary>
        /// <param name="t">Type to query</param>
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
            }

            sql += closeBrackets;
            Orm._FillList(t, _InternalValues, sql, parameters, localCache);
        }

        /// <summary>
        /// Gets the query result
        /// </summary>
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

        /// <summary>
        /// Setter for the Operation and arguments
        /// </summary>
        /// <param name="operation">Operation of the query</param>
        /// <param name="args">Arguments of the query</param>
        /// <returns>New instance of Query<T> where the previous query contains the operation and arguments</returns>
        private Query<T> _SetOp(QueryOperation operation, params object[] args)
        {
            _Operation = operation;
            _Arguments = args;

            return new Query<T>(this);
        }

        /// <summary>
        /// Adds not operation to the query
        /// </summary>
        /// <returns>New Query where the previous query has the not operation</returns>
        public Query<T> Not()
        {
            return _SetOp(QueryOperation.NOT);
        }

        /// <summary>
        /// Adds and operation to the query
        /// </summary>
        /// <returns>New Query where the previous query has the and operation</returns>
        public Query<T> And()
        {
            return _SetOp(QueryOperation.AND);
        }

        /// <summary>
        /// Adds or operation to the query
        /// </summary>
        /// <returns>New Query where the previous query has the or operation</returns>
        public Query<T> Or()
        {
            return _SetOp(QueryOperation.OR);
        }

        /// <summary>
        /// Adds begin group operation to the query
        /// </summary>
        /// <returns>New Query where the previous query has the begin group operation</returns>
        public Query<T> BeginGroup()
        {
            return _SetOp(QueryOperation.BEGIN_GROUP);
        }

        /// <summary>
        /// Adds end group operation to the query
        /// </summary>
        /// <returns>New Query where the previous query has the end group operation</returns>
        public Query<T> EndGroup()
        {
            return _SetOp(QueryOperation.END_GROUP);
        }

        /// <summary>
        /// Adds equals operation to the query
        /// </summary>
        /// <param name="field">Field that should match a given value</param>
        /// <param name="value">Value that the field should match</param>
        /// <param name="ignoreCase">Flag for ignoring case</param>
        /// <returns>New Query where the previous query has the equals operation</returns>
        public Query<T> Equals(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOperation.EQUALS, field, value, ignoreCase);
        }

        /// <summary>
        /// Adds like operation to the query
        /// </summary>
        /// <param name="field">Field that should match a given value (with wildcards)</param>
        /// <param name="value">Value with wildcards that the field should match</param>
        /// <param name="ignoreCase">Flag for ignoring case</param>
        /// <returns>New Query where the previous query has the like operation</returns>
        public Query<T> Like(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOperation.LIKE, field, value, ignoreCase);
        }

        /// <summary>
        /// Adds in operation to the query
        /// </summary>
        /// <param name="field">Field that should be in the given values</param>
        /// <param name="values">Values that should contain the field's value</param>
        /// <returns>New Query where the previous query has the in operation</returns>
        public Query<T> In(string field, params object[] values)
        {
            List<object> list = new List<object>(values);
            list.Insert(0, field);
            return _SetOp(QueryOperation.IN, list.ToArray());
        }

        /// <summary>
        /// Adds greater than operation to the query
        /// </summary>
        /// <param name="field">Field that should be greater than the given value</param>
        /// <param name="value">Value that the field should be greater than</param>
        /// <returns>New Query where the previous query has the greater than operation</returns>
        public Query<T> GreaterThan(string field, object value)
        {
            return _SetOp(QueryOperation.GREATER_THAN, field, value);
        }

        /// <summary>
        /// Adds less than operation to the query
        /// </summary>
        /// <param name="field">Field that should be less than the given value</param>
        /// <param name="value">Value that the field should be less than</param>
        /// <returns>New Query where the previous query has the less than operation</returns>
        public Query<T> LessThan(string field, object value)
        {
            return _SetOp(QueryOperation.LESS_THAN, field, value);
        }

        /// <summary>
        /// Returns the result of the query as a List
        /// </summary>
        /// <returns>List containing the result</returns>
        public List<T> ToList()
        {
            return new List<T>(_Values);
        }

        // Implementation of IEnumerable<T>

        /// <summary>
        /// Gets the Enumerator of the results
        /// </summary>
        /// <returns>Enumerator of the result list</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the Enumerator of the results
        /// </summary>
        /// <returns>Enumerator of the result list</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}