using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using if19b135.OrmFramework.Caches;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class QueryTests
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
        public void QueryTeacherTest()
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

            List<Teacher> retrievedTeachers = Orm.From<Teacher>().ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryBaseClassTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t1",
                FirstName = "Yolanda",
                Name = "Roy",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Person> retrievedPersons = Orm.From<Person>().ToList();
            Assert.AreEqual(1, retrievedPersons.Count);
            Person retrievedPerson = retrievedPersons.First();
            Assert.AreEqual(teacher.ID, retrievedPerson.ID);
            Assert.AreEqual(teacher.Name, retrievedPerson.Name);
            Assert.AreEqual(teacher.FirstName, retrievedPerson.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedPerson.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedPerson.BirthDate);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryEqualsStringTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers = Orm.From<Teacher>().Equals("Name", "Heath").ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);
            Orm.Connection.Close();
        }

        [Test]
        public void QueryEqualsIntTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers = Orm.From<Teacher>().Equals("Salary", 10001).ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryEqualsEnumTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers = Orm.From<Teacher>().Equals("Gender", Gender.FEMALE).ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryEqualsDateTimeTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().Equals("BirthDate", new DateTime(1989, 1, 20)).ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryLikeTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().Like("FirstName", "F%").ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            retrievedTeachers =
                Orm.From<Teacher>().Like("FirstName", "Ft%").ToList();
            Assert.AreEqual(0, retrievedTeachers.Count);

            Orm.Connection.Close();
        }

        [Test]
        public void QueryGreaterThanTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().GreaterThan("Salary", 10000).ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            retrievedTeachers =
                Orm.From<Teacher>().GreaterThan("Salary", 20000).ToList();
            Assert.AreEqual(0, retrievedTeachers.Count);
            Orm.Connection.Close();
        }

        [Test]
        public void QueryLessThanTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().LessThan("Salary", 10002).ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            retrievedTeachers =
                Orm.From<Teacher>().LessThan("Salary", 10001).ToList();
            Assert.AreEqual(0, retrievedTeachers.Count);
            Orm.Connection.Close();
        }

        [Test]
        public void QueryInTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().In("Name", "Peter", "Maya", "Heath").ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            retrievedTeachers =
                Orm.From<Teacher>().In("Name", "Peter", "Maya").ToList();
            Assert.AreEqual(0, retrievedTeachers.Count);
            Orm.Connection.Close();
        }

        [Test]
        public void QueryComplexTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();
            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Faith",
                Name = "Heath",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1989, 1, 20),
                HireDate = new DateTime(2021, 12, 10),
                Salary = 10001
            };

            Orm.Save(teacher);

            List<Teacher> retrievedTeachers =
                Orm.From<Teacher>().BeginGroup().In("Name", "Peter", "Maya").And().Equals("Gender", Gender.FEMALE)
                    .EndGroup().Or().BeginGroup().Equals("FirstName", "Faith").And().Equals("Salary", 10001).EndGroup()
                    .ToList();
            Assert.AreEqual(1, retrievedTeachers.Count);
            Teacher retrievedTeacher = retrievedTeachers.First();
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            retrievedTeachers =
                Orm.From<Teacher>().In("Name", "Peter", "Maya").ToList();
            Assert.AreEqual(0, retrievedTeachers.Count);
            Orm.Connection.Close();
        }
    }
}