using System;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using if19b135.OrmFramework.Exceptions;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Locking
{
    /// <summary>
    /// Uses the database for Locking
    /// </summary>
    public class DbLocking : ILocking
    {
        /// <summary>
        /// Unique session id (GUID set in the constructor)
        /// </summary>
        public string SessionKey { get; protected set; }
        
        /// <summary>
        /// Timeout for purging old locks (from all users)
        /// </summary>
        public int Timeout { get; set; } = 180;
        

        /// <summary>
        /// Constructor creates a new instance of this class. Also creates a LOCKS table in the DB if it does not exist.
        /// </summary>
        public DbLocking()
        {
            SessionKey = Guid.NewGuid().ToString();
            IDbCommand cmd = Orm.Connection.CreateCommand();

            try
            {
                cmd.CommandText = "CREATE TABLE LOCKS ( LCLASS VARCHAR(64) NOT NULL , LOBJECT VARCHAR(64) NOT NULL, " +
                                  "LTIME TIMESTAMP NOT NULL, LOWNER VARCHAR(64) NOT NULL)";
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = Orm.Connection.CreateCommand();
                cmd.CommandText = "CREATE UNIQUE INDEX UNQ_IDX_LOCKS ON LOCKS(LCLASS, LOBJECT)";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        /// <summary>
        /// Gets the case insensitive table name and the primary key for a given object
        /// </summary>
        /// <param name="obj">Object to retrieve table name and primary key</param>
        /// <returns>Case insensitive table name and primary key as string</returns>
        private (string ClassKey, string ObjectKey) _GetKeys(object obj)
        {
            Entity entity = obj.GetEntity();
            return (entity.TableName.ToUpper(),
                entity.PrimaryKey.ToColumnType(entity.PrimaryKey.GetValue(obj)).ToString());
        }

        /// <summary>
        /// Gets the owner of a locked object.
        /// </summary>
        /// <param name="obj">Object to get the owner for</param>
        /// <returns>Owner (GUID) or null if object is not locked</returns>
        private string _GetLock(object obj)
        {
            var keys = _GetKeys(obj);
            string result = null;
            IDbCommand cmd = Orm.Connection.CreateCommand();
            cmd.CommandText = "SELECT LOWNER FROM LOCKS WHERE LCLASS = :c AND LOBJECT = :o";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":c";
            p.Value = keys.ClassKey;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = ":o";
            p.Value = keys.ObjectKey;
            cmd.Parameters.Add(p);

            IDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetString(0);
            }

            reader.Close();
            reader.Dispose();
            cmd.Dispose();

            return result;
        }

        /// <summary>
        /// Creates lock entry in the DB's locking table
        /// </summary>
        /// <param name="obj">Object to lock</param>
        private void _CreateLock(object obj)
        {
            var keys = _GetKeys(obj);
            IDbCommand cmd = Orm.Connection.CreateCommand();
            cmd.CommandText =
                "INSERT INTO LOCKS (LCLASS, LOBJECT, LTIME, LOWNER) VALUES (:c, :o, Current_Timestamp, :s)";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":c";
            p.Value = keys.ClassKey;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = ":o";
            p.Value = keys.ObjectKey;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = ":s";
            p.Value = SessionKey;
            cmd.Parameters.Add(p);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
            }

            cmd.Dispose();
        }
        

        /// <summary>
        /// Releases all locks that are older than the Timeout
        /// </summary>
        public void Purge()
        {
            IDbCommand cmd = Orm.Connection.CreateCommand();
            cmd.CommandText =
                "DELETE FROM LOCKS WHERE ((JulianDay(Current_Timestamp) - JulianDay(LTIME)) * 86400) > :t";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":t";
            p.Value = Timeout;
            cmd.Parameters.Add(p);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        
        // Implementation of ILocking

        /// <summary>
        /// Lock a given object. Throws ObjectAlreadyLockedException if the object is locked.
        /// </summary>
        /// <param name="obj">Object to lock</param>
        public void Lock(object obj)
        {
            string owner = _GetLock(obj);
            if (owner == SessionKey)
            {
                return;
            }

            if (owner == null)
            {
                _CreateLock(obj);
                owner = _GetLock(obj);
            }

            if (owner != SessionKey)
            {
                throw new ObjectAlreadyLockedException();
            }
        }

        /// <summary>
        /// Release / Unlock a given object
        /// </summary>
        /// <param name="obj">Object to unlock/release</param>
        public void Release(object obj)
        {
            var keys = _GetKeys(obj);
            IDbCommand cmd = Orm.Connection.CreateCommand();
            cmd.CommandText =
                "DELETE FROM LOCKS WHERE LCLASS = :c AND LOBJECT = :o AND LOWNER = :s";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":c";
            p.Value = keys.ClassKey;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = ":o";
            p.Value = keys.ObjectKey;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = ":s";
            p.Value = SessionKey;
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }


    }
}