using System;
using System.Data;
using System.Data.SQLite;
using if19b135.OrmFramework.Caches;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class CacheTests
    {
        [OneTimeSetUp]
        public void Init()
        {
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
        public void DefaultCacheTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Orm.Cache = null;
            
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
            Teacher retrievedTeacher = Orm.Get<Teacher>("t0");
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            int lastInstanceNumber = retrievedTeacher.InstanceNumber;
            int numberOfRequest = 10;
            
            for (int i = 0; i < numberOfRequest; i++)
            {
                retrievedTeacher = Orm.Get<Teacher>("t0");
            }

            Assert.AreEqual(lastInstanceNumber + numberOfRequest, retrievedTeacher.InstanceNumber);
            lastInstanceNumber = retrievedTeacher.InstanceNumber;

            Orm.Cache = new DefaultCache();

            for (int i = 0; i < numberOfRequest; i++)
            {
                retrievedTeacher = Orm.Get<Teacher>("t0");
            }

            Assert.AreEqual(lastInstanceNumber + 1, retrievedTeacher.InstanceNumber);

            Orm.Connection.Close();
        }
        
        
        [Test]
        public void TrackingCacheTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Orm.Cache = null;

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
            Teacher retrievedTeacher = Orm.Get<Teacher>("t0");
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            int lastInstanceNumber = retrievedTeacher.InstanceNumber;
            int numberOfRequest = 10;

            for (int i = 0; i < numberOfRequest; i++)
            {
                retrievedTeacher = Orm.Get<Teacher>("t0");
            }

            Assert.AreEqual(lastInstanceNumber + numberOfRequest, retrievedTeacher.InstanceNumber);
            lastInstanceNumber = retrievedTeacher.InstanceNumber;
            Orm.Cache = new DefaultCache();

            for (int i = 0; i < numberOfRequest; i++)
            {
                retrievedTeacher = Orm.Get<Teacher>("t0");
            }

            Assert.AreEqual(lastInstanceNumber + 1, retrievedTeacher.InstanceNumber);

            Orm.Connection.Close();
        }
    }
}