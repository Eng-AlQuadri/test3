using Core.Entities;

namespace Core.Specifications;

public class SubjectStudentSpecification : BaseSpecefication<SubjectStudents>
{
    public SubjectStudentSpecification(int studentId, int subjectId) 
        : base(x => x.StudentId == studentId && x.SubjectId == subjectId)
    {

    } 
}
