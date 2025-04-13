using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace StudentRepository
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ForegroundColor = ConsoleColor.DarkCyan;
            WriteLine("Project Of Student Repository:");
            WriteLine("------------------------------");

            WriteLine();
            ResetColor();
            try
            {
                //calling student repositury as singleton pattern. 
                using (IRepository<Student> Students = StudentRepository.Instance)
                {
                    #region Add student 

                    Students.Add(new Student(8) { MobileNo = "01580466994", FirstName = "Soniya", LastName = "Akter", Email = "soniya669@gamil.com", BirthDate = Convert.ToDateTime("01-Feb-1997"), Subject = Subject.Math });

                    #endregion

                    var c2 = Students.FindById(2);
                    c2.FirstName = "Abul";
                    c2.LastName = "Bashar";
                    Students.Update(c2);

                    WriteLine($"Student {c2.Id} updated successfully");
                    WriteLine(c2.ToString());

                    if (Students.Delete(c2))
                        WriteLine($"Student {c2.Id} deleted successfully");

                    #region Search from repository

                    var data = Students.Search("Akter");
                    Console.WriteLine();
                    Console.WriteLine($"Total Students name matching with Akter {data.Count()}");
                    Console.WriteLine("----------------------------------");

                    foreach (var c in data)
                    {
                        Console.WriteLine(c.ToString());
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex);
            }
            finally
            {
                ReadLine();
            }
        }
    }
    /// <summary>
    /// base interface for repository model
    /// </summary>
    public interface IEntity : IDisposable
    {
        int Id { get; }
        bool IsValid();
    }

    public interface IRepository<T> : IDisposable, IEnumerable<T> where T : IEntity
    {
        IEnumerable<T> Data { get; }
        void Add(T entity);
        bool Delete(T entity);
        void Update(T entity);
        T FindById(int Id);
        IEnumerable<T> Search(string value);
    }
    /// <summary>
    /// enum subject type
    /// </summary>
    public enum Subject
    {
        English,
        Math,
        Bangla
    }
    /// <summary>
    /// repository element model class
    /// </summary>
    public sealed class Student : IEntity
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Subject Subject { get; set; }

        public Student() { }

        public Student(int studentId)
        {
            Id = studentId;
            BirthDate = null;
            Subject = Subject.English;
        }

        public Student(int studentId, string mobileNo, string firstName, string lastName = null, string email = null, DateTime? birthDate = null, Subject subject = Subject.English)
        {
            Id = studentId;
            MobileNo = mobileNo;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Subject = subject;
        }
        /// <summary>
        /// Valid student object check  before save into repository
        /// </summary>
        /// <returns>
        /// boolean: student validity
        /// </returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(MobileNo) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || BirthDate?.Date > DateTime.Now)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"Student Info\nStudentID: {Id}\nName: {FullName}\nMobile: {MobileNo}\nEmail: {Email}\nDate of Birth: {BirthDate?.ToString("d")}\nGroup: {Subject}\n************************\n";
        }

        public void Dispose() { }
    }

    public sealed class StudentRepository : IRepository<Student>
    {
        private static StudentRepository _instance;
        public static StudentRepository Instance
        {
            get
            {
                return _instance ?? new StudentRepository(); ;
            }
        }

        private List<Student> Studentinfo;

        private StudentRepository()
        {
            Studentinfo = new List<Student>
            {
                new Student(1, "01865989632", "Abdur", "Zabbar", "abdur9966@gmail.com"),
                new Student(2, "01734567787", "Abul", "Bashar", "abul654@gmail.com"),
                new Student(3, "01847645671", "Ruhul", "Amin", "ruhul71@gmail.com"),
                new Student(4, "01836857496", "Hamidur", "Rahman", "hamid1971@gmail.com"),
                new Student(5, "01358415263", "Bilkis", "Akter", "bilkis994@gmail.com"),
                new Student(6, "01896758496", "Jorina", "Akter", "jori_na@gmail.com"),
                new Student(7, "01325968574", "Hasan", "Ali", "ali987@gmail.com")
            };
        }

        public IEnumerable<Student> Data => Studentinfo;

        public void Add(Student entity)
        {
            if (Studentinfo.Any(c => c.Id == entity.Id))
                throw new Exception("Duplicate Student Id, try another");

            if (entity.IsValid())
                Studentinfo.Add(entity);
            else
                throw new Exception("Student is invalid");
        }

        public bool Delete(Student entity) => Studentinfo.Remove(entity);

        public void Update(Student entity)
        {
            var index = Studentinfo.FindIndex(c => c.Id == entity.Id);
            if (index != -1)
                Studentinfo[index] = entity;
        }

        public Student FindById(int id) => Studentinfo.FirstOrDefault(r => r.Id == id);

        public IEnumerable<Student> Search(string value)
        {
            return Studentinfo.Where(r =>
                r.Id.ToString().Contains(value) ||
                (r.FirstName?.StartsWith(value) ?? false) ||
                (r.LastName?.StartsWith(value) ?? false) ||
                (r.MobileNo?.Contains(value) ?? false) ||
                (r.Email?.Contains(value) ?? false) ||
                (r.BirthDate?.ToString().Contains(value) ?? false))
                .OrderBy(r => r.FirstName);
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return Studentinfo.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Studentinfo.Clear();
        }
    }
}
