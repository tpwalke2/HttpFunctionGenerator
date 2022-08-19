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
}