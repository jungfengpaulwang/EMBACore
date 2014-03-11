using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Presentation;

namespace EMBACore.Filters
{
    internal class FilterService
    {
        private static Student _student = null;
        public static Student Student
        {
            get
            {
                if (_student == null)
                    _student = new Student(NLDPanels.Student);

                return _student;
            }
        }

        private static Teacher _teacher = null;
        public static Teacher Teacher
        {
            get
            {
                if (_teacher == null)
                    _teacher = new Teacher(NLDPanels.Teacher);

                return _teacher;
            }
        }

        private static Course _course = null;
        public static Course Course
        {
            get
            {
                if (_course == null)
                    _course = new Course(NLDPanels.Course);

                return _course;
            }
        }
    }
}
