using Core.Entities;

namespace Core.Specifications;

public class SubjectSpecification2 : BaseSpecefication<Subject>
{
    public SubjectSpecification2(string name) : base(x => x.Name == name)
    {
        
    }
}
