using System;
using if19b135.OrmFramework.Attributes;
using if19b135.OrmFramework.LazyLoading;


namespace if19b135.OrmFramework.Model
{
    /// <summary>This class represents a class in the school model.</summary>
    [Entity(TableName = "CLASSES")]
    public class Class
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>Creates a new instance of this class.</summary>
        public Class()
        {
            Students = new LazyList<Student>(this, "Students");
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // private properties                                                                                               //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the teacher with lazy loading.</summary>
        [ForeignKey(ColumnName = "KTEACHER")]
        private LazyObject<Teacher> _Teacher { get; set; } = new LazyObject<Teacher>();


        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the class ID.</summary>
        [PrimaryKey]
        public string ID { get; set; }


        /// <summary>Gets or sets the class name.</summary>
        public string Name { get; set; }


        /// <summary>Gets or sets the class teacher.</summary>
        [Ignore]
        public Teacher Teacher
        {
            get { return _Teacher.Value; }
            set { _Teacher.Value = value; }
        }


        /// <summary>Gets the class students.</summary>
        [ForeignKey(ColumnName = "KCLASS")]
        public LazyList<Student> Students
        {
            get; set;
        }
    }
}
