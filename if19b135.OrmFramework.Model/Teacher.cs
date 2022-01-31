using System;
using System.Collections.Generic;
using if19b135.OrmFramework.Attributes;


namespace if19b135.OrmFramework.Model
{
    /// <summary>This is a teacher implementation (from School example).</summary>
    [Entity(TableName = "TEACHERS")]
    public class Teacher: Person
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the teacher's salary.</summary>
        public int Salary { get; set; }


        /// <summary>Gets or sets the teacher's hire date.</summary>
        [Field(ColumnName = "HDATE")]
        public DateTime HireDate { get; set; }


        /// <summary>Gets the teacher's courses.</summary>
        [ForeignKey(ColumnName = "KTEACHER")]
        public List<Class> Classes { get; private set; } = new List<Class>();
    }
}
