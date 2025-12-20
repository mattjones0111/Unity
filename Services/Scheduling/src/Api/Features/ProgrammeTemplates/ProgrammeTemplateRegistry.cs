using Contracts;
using Marten;

namespace Api.Features.ProgrammeTemplates;

public class ProgrammeTemplateRegistry : MartenRegistry
{
    public ProgrammeTemplateRegistry()
    {
        For<ProgrammeTemplate>()
            .DocumentAlias("programme_templates")
            .Identity(x => x.Id);
    }
}