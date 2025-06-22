using System;
using Core.Entities;

namespace Core.Specifications;

public class StudentSubjectSpecification : BaseSpecefication<SubjectStudents>
{
    public StudentSubjectSpecification(int studentId) : base(x => x.Student.Id == studentId)
    {
        AddInclude(x => x.Subject);
    }

    // Load student IDs that have shared courses with specific student
    public StudentSubjectSpecification(int[] coursesIds) : base(x => coursesIds.Contains(x.SubjectId))
    {
        // AddInclude("Student.AppUser.Photos");
        AddInclude(x => x.Student.AppUser.Photos);
    }
}
