using HttpFunction.Attributes;
using HttpFunction.Models;

namespace GeneratedHttpFunctionTest;

[HttpFunction]
public class UsersController
{
    public Outcome BuildSomething()
    {
        return new Outcome(Status.Success);
    }
}