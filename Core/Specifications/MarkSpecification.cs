using Core.Entities;

namespace Core.Specifications;

public class MarkSpecification : BaseSpecefication<Mark>
{
    public MarkSpecification(int subjectId, int studentId) 
        : base(x => x.SubjectId == subjectId && x.StudentId == studentId)
    {
        
    }

    public MarkSpecification(int studentId) : base(x => x.StudentId == studentId)
    {

    }
}
