using FinanceHub.Api.Application.Common;

namespace FinanceHub.Tests.Builders;

public class CurrentUserBuilder
{
    private Guid _id = Guid.NewGuid();

    private string _nome = "Igor";

    private string _email = "igor@email.com";

    public CurrentUserBuilder ComId(Guid id)
    {
        _id = id;
        return this;
    }

    public CurrentUser Build()
    {
        return new CurrentUser
        {
            Id = _id,
            Nome = _nome,
            Email = _email
        };
    }
}