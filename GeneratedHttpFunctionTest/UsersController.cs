using HttpFunction.Attributes;
using HttpFunction.Models;
using System.Threading.Tasks;

namespace GeneratedHttpFunctionTest;

[HttpFunction]
public class UsersController
{
    public Outcome BuildSomething()
    {
        return new Outcome(Status.Success);
    }

    public Task<Outcome<bool>> GetSomething()
    {
        return Task.FromResult(new Outcome<bool>(Status.Success, true));
    }

    public Task<Outcome<GetUserResult>> GetUser(GetUserQuery query)
    {
        return Task.FromResult(new Outcome<GetUserResult>(Status.Success,
            new GetUserResult
            {
                Name = "John Doe"
            }));
    }
}

public class GetUserResult
{
    public string Name { get; set; }
}
    
public class GetUserQuery
{
    [FromRoute]
    public int Id { get; set; }
}