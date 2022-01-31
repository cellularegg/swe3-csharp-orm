using System;
using System.Data;
using System.Data.SQLite;
using if19b135.OrmFramework.Model;
using NUnit.Framework;

namespace if19b135.OrmFramework.Tests
{
    [TestFixture]
    public class ForeignKeyTests
    {
        [SetUp]
        public void Init()
        {
            IDbConnection conn = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            conn.Open();
            IDbCommand cmd = conn.CreateCommand();

            Orm.Cache = null;
            Orm.Locking = null;
            
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
        public void ForeignKey1nTest()
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
        
        [Test]
        public void ForeignKey1nListTest()
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

            Class myClass = new Class
            {
                ID = "c2",
                Name = "Class 2",
                Teacher = teacher
            };

            Orm.Save(myClass);
            myClass = new Class
            {
                ID = "c3",
                Name = "Class 3",
                Teacher = teacher
            };
            Orm.Save(myClass);


            Teacher retrievedTeacher = Orm.Get<Teacher>("t2");

            Assert.AreEqual(2, retrievedTeacher.Classes.Count);

            Orm.Connection.Close();
        }

        [Test]
        public void ForeignKeyMNTest()
        {
            Orm.Connection = new SQLiteConnection("Data Source=if19b135.Orm.Tests.sqlite;Version=3;");
            Orm.Connection.Open();

            Teacher teacher = new Teacher
            {
                ID = "t3",
                FirstName = "Eva",
                Name = "Musterfrau",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1981, 1, 20),
                HireDate = new DateTime(2020, 12, 10),
                Salary = 30000
            };

            Orm.Save(teacher);

            Course course = new Course
            {
                ID = "course0",
                Name = "Course 1",
                Teacher = teacher
            };
            
            Student student1 = new Student
            {
                ID = "s1",
                Name = "Dora",
                FirstName = "Viktor",
                Gender = Gender.MALE,
                BirthDate = new DateTime(1998, 4, 12),
                Grade = 2
            };
            Orm.Save(student1);
            
            course.Students.Add(student1);
            
            Student student2 = new Student
            {
                ID = "s2",
                Name = "Valentin",
                FirstName = "Mathilde",
                Gender = Gender.FEMALE,
                BirthDate = new DateTime(1997, 2, 21),
                Grade = 1
            };
            Orm.Save(student2);
            
            course.Students.Add(student2);
            
            Orm.Save(course);

            Course retrievedCourse = Orm.Get<Course>("course0");
            Assert.AreEqual(2, retrievedCourse.Students.Count);

            Orm.Connection.Close();
        }
    }
}