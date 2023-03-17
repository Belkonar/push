using shared.Models.Pipeline;
using shared.Models;

namespace shared.Interfaces;

public interface IPipelineController
{
    public Task<List<Pipeline>> GetPipelines(Nullable<Guid> org);
    public Task<Pipeline> GetPipeline(Guid id);
    public Task<SimpleValue> GetLatestMajor(Guid id);
    public Task<List<string>> GetVersions(Guid id);
    public Task<PipelineVersion> GetVersionByConstraint(Guid id, string constraint);
    public Task<Dictionary<string,string>> GetFiles(Guid id, string version);
    public Task<Pipeline> CreatePipeline(Pipeline data);
    public Task<Pipeline> UpdatePipeline(Guid id, Pipeline data);
    public Task<PipelineVersion> UpdatePipelineVersion(Guid id, string key, PipelineVersion data);
}
