using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Api.Host.Contracts.V1;
using Marten.Events.CodeGeneration;
using Marten.Linq;

namespace Api.Host.Features.Artist;

public class FindByArtistName : ICompiledListQuery<Contracts.V1.Artist, ArtistSlim>, IQueryPlanning
{
    public required string? Name { get; set; }
    [MartenIgnore] public int PageNumber { get; set; } = 1;
    public int SkipCount => (PageNumber - 1) * PageSize;
    public int PageSize { get; set; } = 20;

    public Expression<Func<IMartenQueryable<Contracts.V1.Artist>, IEnumerable<ArtistSlim>>> QueryIs() =>
        query => query
            .Where(x => string.IsNullOrEmpty(Name) || x.Name.Contains(Name, StringComparison.OrdinalIgnoreCase))
            .Skip(SkipCount)
            .Take(PageSize)
            .Select(x => new ArtistSlim(x.Id, x.Name));

    public void SetUniqueValuesForQueryPlanning()
    {
        PageNumber = 3;
        PageSize = 20;
        Name = Guid.NewGuid().ToString();
    }
}