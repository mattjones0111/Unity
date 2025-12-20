using Refit;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contracts;

public interface ISchedulingClient
{
    [Post("/programme-templates")]
    Task<HttpResponseMessage> CreateProgrammeTemplateAsync(
        ProgrammeTemplate programmeTemplate,
        CancellationToken ct = default);
    
    [Get("/programme-templates/{id}")]
    Task<ApiResponse<ProgrammeTemplate>> GetProgrammeTemplateAsync(
        Guid id,
        CancellationToken ct = default);
}
