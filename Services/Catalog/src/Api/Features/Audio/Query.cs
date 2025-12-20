using Api.Data;
using Contracts;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Audio;

public class Query(CatalogDbContext db)
    : EndpointWithoutRequest<AudioSlim[]>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/audio");
    }

    public override async Task<AudioSlim[]> ExecuteAsync(
        CancellationToken ct)
    {
        var pn = Route<int>("pn", false);
        var ps = Route<int>("ps", false);

        if (pn < 1)
        {
            pn = 1;
        }

        if (ps < 1 || ps > 50)
        {
            ps = 50;
        }

        var result = (await db.AudioItems
                .Skip((pn - 1) * ps)
                .Take(ps)
                .ToArrayAsync(ct))
            .Select(x => new AudioSlim(x.Id, x.Categories, x.Title, x.Artist, x.Duration))
            .ToArray();

        return result;
    }
}
