using Contracts;
using FastEndpoints;
using FluentValidation;
using Marten;

namespace Api.Features.Channels;

public class ChannelRegistry : MartenRegistry
{
    public ChannelRegistry()
    {
        For<Channel>()
            .DocumentAlias("channels")
            .Identity(x => x.Id);
    }
}

public class ChannelValidator : Validator<Channel>
{
    public ChannelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
