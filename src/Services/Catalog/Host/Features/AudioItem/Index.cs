using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Contracts.V1;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;

namespace Api.Host.Features.AudioItem;

[HttpGet("/audio-items")]
[AllowAnonymous]
public class Index : EndpointWithoutRequest<IEnumerable<AudioItemSlim>>
{
    private readonly IDocumentSession session;

    public Index(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var categoryFilter = Query<string>("category");
        var titleFilter = Query<string?>("title", isRequired: false);
        var pageNumber = Query<int?>("pn", isRequired: false) ?? 1;
        var pageSize = Query<int?>("ps", isRequired: false) ?? 20;

        var mappedResults = await session.QueryAsync(
            new FindByTitle
            {
                Title = titleFilter,
                Category = categoryFilter!,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            ct);
        
        await SendOkAsync(mappedResults, ct);
    }
}