using Core.Entities;

namespace Core.Specifications;

public class StudentSpecification : BaseSpecefication<StudentUser>
{
    public StudentSpecification(StudentSpecParams specParams) : base(x =>
        string.IsNullOrEmpty(specParams.Search) || 
        x.AppUser.UserName!.ToLower().Contains(specParams.Search)
    ) 
    {
        AddInclude(x => x.AppUser);

        AddInclude(x => x.AppUser.Photos);

        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        switch (specParams.Sort)
        {
            case "nameAsc":
                AddOrderBy(x => x.AppUser.UserName!);
                break;

            case "nameDesc":
                AddOrderByDescending(x => x.AppUser.UserName!);
                break;

            case "lastActive":
                AddOrderByDescending(x => x.AppUser.LastActive);
                break;

            default:
                AddOrderBy(x => x.AppUser.CreatedAt);
                break;
        }
    }

    // to get student for admin
    public StudentSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude(x => x.AppUser);
        AddInclude(x => x.SubjectStudents);
    }

    // to get student profile for student
    public StudentSpecification(int id, bool flag) : base(x => x.Id == id)
    {
        AddInclude(x => x.AppUser);
        AddInclude(x => x.AppUser.Photos);
        AddInclude(x => x.AppUser.Photos);
    }
}
