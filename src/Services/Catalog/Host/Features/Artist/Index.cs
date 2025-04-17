using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Contracts.V1;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authorization;

namespace Api.Host.Features.Artist;

[HttpGet("/artists")]
[AllowAnonymous]
public class Index : EndpointWithoutRequest<IEnumerable<ArtistSlim>>
{
    private readonly IDocumentSession session;

    public Index(IDocumentSession session)
    {
        this.session = session;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string>("search", isRequired: true);
        var pageNumber = Query<int?>("pn", isRequired: false) ?? 1;
        var pageSize = Query<int?>("ps", isRequired: false) ?? 20;

        var results = await session.QueryAsync(
            new FindByArtistName
            {
                Name = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            ct);

        await SendOkAsync(results, ct);
    }
}