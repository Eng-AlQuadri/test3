using Core.Entities;

namespace Core.Specifications;

public class SubjectSpecification : BaseSpecefication<Subject, string>
{
    public SubjectSpecification()
    {
        AddSelect(x => x.Name);
        ApplyDistinct();
    }


    public SubjectSpecification(string name) : base(x => x.Name == name) // to check if subject exists
    {
        AddSelect(x => x.Name);
    }
}

