using System;
using System.Data;
using System.Data.SQLite;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class SaveGetTests
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
        public void SaveGetTeacherTest()
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
            Teacher retrievedTeacher = Orm.Get<Teacher>("t0");
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
        public void SaveGetClassTest()
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
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Class myClass = new Class
            {
                ID = "c0",
                Name = "Class 1",
                Teacher = teacher
            };

            Orm.Save(myClass);

            Class retrievedClass = Orm.Get<Class>("c0");

            Assert.AreEqual(myClass.ID, retrievedClass.ID);
            Assert.AreEqual(myClass.Name, retrievedClass.Name);
            Assert.AreEqual(myClass.Teacher.ID, retrievedClass.Teacher.ID);

            Orm.Connection.Close();
        }

        [Test]
        public void SaveGetStudentTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t2",
                FirstName = "Eva",
                Name = "Musterfrau",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1981, 1, 20),
                HireDate = new DateTime(2020, 12, 10),
                Salary = 30000
            };

            Orm.Save(teacher);
            Teacher retrievedTeacher = Orm.Get<Teacher>("t2");
            Assert.AreEqual(teacher.ID, retrievedTeacher.ID);
            Assert.AreEqual(teacher.Name, retrievedTeacher.Name);
            Assert.AreEqual(teacher.FirstName, retrievedTeacher.FirstName);
            Assert.AreEqual(teacher.Gender, retrievedTeacher.Gender);
            Assert.AreEqual(teacher.BirthDate, retrievedTeacher.BirthDate);
            Assert.AreEqual(teacher.HireDate, retrievedTeacher.HireDate);
            Assert.AreEqual(teacher.Salary, retrievedTeacher.Salary);
            Assert.AreEqual(teacher.Classes, retrievedTeacher.Classes);

            Class myClass = new Class
            {
                ID = "c1",
                Name = "Class 1",
                Teacher = teacher
            };

            Orm.Save(myClass);

            Class retrievedClass = Orm.Get<Class>("c1");

            Assert.AreEqual(myClass.ID, retrievedClass.ID);
            Assert.AreEqual(myClass.Name, retrievedClass.Name);
            Assert.AreEqual(myClass.Teacher.ID, retrievedClass.Teacher.ID);

            Student student = new Student
            {
                ID = "s0",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1999, 1, 1),
                FirstName = "David",
                Name = "Maya",
                Grade = 1,
                Class = retrievedClass
            };

            Orm.Save(student);
            Student retrievedStudent = Orm.Get<Student>("s0");
            Assert.AreEqual(student.ID, retrievedStudent.ID);
            Assert.AreEqual(student.Name, retrievedStudent.Name);
            Class updatedClass = Orm.Get<Class>("c1");
            Assert.AreEqual(updatedClass.Students[0].ID, student.ID);

            Orm.Connection.Close();
        }
    }
}