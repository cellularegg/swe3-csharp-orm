using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using if19b135.OrmFramework.Exceptions;
using if19b135.OrmFramework.Locking;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class LockingTests
    {
        [SetUp]
        public void Init()
        {
            Orm.Cache = null;
            Orm.Locking = null;

            IDbConnection conn = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            conn.Open();
            IDbCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"DELETE FROM STUDENT_COURSES;
DELETE FROM COURSES;
DELETE FROM STUDENTS;
DELETE FROM CLASSES;
DELETE FROM TEACHERS;";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }


        [Test]
        public void NullLockingTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Max",
                Name = "Mustermann",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1980, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10000
            };

            Orm.Save(teacher);

            Orm.Lock(teacher);

            Orm.Lock(teacher);

            Orm.Connection.Close();
        }

        [Test]
        public void DbLockingTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            DbLocking lock1 = new DbLocking();
            Orm.Locking = lock1;

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Max",
                Name = "Mustermann",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1980, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10000
            };

            Orm.Save(teacher);

            Orm.Lock(teacher);

            DbLocking lock2 = new DbLocking();
            Orm.Locking = lock2;

            Assert.Throws<ObjectAlreadyLockedException>(() => Orm.Lock(teacher));
            Orm.Locking = lock1;
            Orm.Release(teacher);
            Orm.Locking = lock2;
            Orm.Lock(teacher);
            Orm.Release(teacher);
            Orm.Connection.Close();
        }
        
        [Test]
        public void DbLockingPurgeTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            DbLocking lock1 = new DbLocking();
            Orm.Locking = lock1;

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Max",
                Name = "Mustermann",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1980, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10000
            };

            Orm.Save(teacher);

            Orm.Lock(teacher);

            DbLocking lock2 = new DbLocking();
            lock2.Timeout = 0;
            Orm.Locking = lock2;

            Assert.Throws<ObjectAlreadyLockedException>(() => Orm.Lock(teacher));
            lock2.Purge();
            
            Orm.Lock(teacher);
            Orm.Release(teacher);
            Orm.Connection.Close();
        }
    }
}