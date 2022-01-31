using System;
using System.Data;
using System.Data.SQLite;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class LazyTests
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
        public void LazyListTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t0",
                FirstName = "Eva",
                Name = "Musterfrau",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1981, 1, 20),
                HireDate = new DateTime(2020, 12, 10),
                Salary = 30000
            };

            Orm.Save(teacher);
            Teacher retrievedTeacher = Orm.Get<Teacher>("t0");
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);


            Class myClass = new Class
            {
                ID = "c0",
                Name = "Class 0",
                Teacher = teacher
            };

            Orm.Save(myClass);

            Class retrievedClass = Orm.Get<Class>("c0");
            Assert.AreEqual(myClass.ID, retrievedClass.ID);

            Student student1 = new Student
            {
                ID = "s0",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1999, 1, 1),
                FirstName = "David",
                Name = "Maya",
                Grade = 1,
                Class = retrievedClass
            };

            Orm.Save(student1);

            Student student2 = new Student
            {
                ID = "s1",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1999, 1, 1),
                FirstName = "Wiegand",
                Name = "Eberhard",
                Grade = 1,
                Class = retrievedClass
            };
            Orm.Save(student2);


            Class updatedClass = Orm.Get<Class>("c0");
            Assert.AreEqual(2, updatedClass.Students.Count);

            Orm.Connection.Close();
        }

        [Test]
        public void LazyObjectTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t1",
                FirstName = "Eva",
                Name = "Musterfrau",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1981, 1, 20),
                HireDate = new DateTime(2020, 12, 10),
                Salary = 30000
            };

            Orm.Save(teacher);
            Teacher retrievedTeacher = Orm.Get<Teacher>("t1");
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);


            Class myClass = new Class
            {
                ID = "c1",
                Name = "Class 1",
                Teacher = teacher
            };

            Orm.Save(myClass);

            Class retrievedClass = Orm.Get<Class>("c1");
            Assert.AreEqual(myClass.ID, retrievedClass.ID);

            Student student1 = new Student
            {
                ID = "s2",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1999, 1, 1),
                FirstName = "David",
                Name = "Maya",
                Grade = 1,
                Class = retrievedClass
            };

            Orm.Save(student1);

            Student student2 = new Student
            {
                ID = "s3",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1999, 1, 1),
                FirstName = "Wiegand",
                Name = "Eberhard",
                Grade = 1,
                Class = retrievedClass
            };
            Orm.Save(student2);


            Class updatedClass = Orm.Get<Class>("c1");
            Assert.AreEqual(2, updatedClass.Students.Count);
            Assert.AreEqual(teacher.ID, retrievedClass.Teacher.ID);
            Assert.AreEqual(teacher.Name, retrievedClass.Teacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedClass.Teacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedClass.Teacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedClass.Teacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedClass.Teacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedClass.Teacher.Salary);
            
            Orm.Connection.Close();
        }
    }
}