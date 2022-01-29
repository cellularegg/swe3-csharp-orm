using System;
using if19b135.OrmFramework.Attributes;

namespace if19b135.TestConsole.Schema
{
    // [Entity(TableName = "PERSONS")]
    public class Person
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string SocialSecurityNumber { get; set; }

        public Person()
        {
            
        }
        
        public Person(int id, string firstName, string lastName, DateTime birthdate, string socialSecurityNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Birthdate = birthdate;
            SocialSecurityNumber = socialSecurityNumber;
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, " +
                $"{nameof(Birthdate)}: {Birthdate}, {nameof(SocialSecurityNumber)}: {SocialSecurityNumber}";
        }
    }
}