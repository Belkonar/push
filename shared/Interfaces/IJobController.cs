using shared.Models.Job;
using shared.UpdateModels;
using shared.Models;

namespace shared.Interfaces;

public interface IJobController
{
    public Task<List<Job>> GetJobByStatus(string status);
    public Task<List<Job>> GetSafeJobs(Nullable<Guid> id);
    public Task<Job> GetJob(Guid id);
    public Task<Job> GetSafeJob(Guid id);
    public Task UpdateStatus(Guid id, UpdateStatus status);
    public Task UpdateStepStatus(Guid id, int ordinal, UpdateStatus status);
    public Task<SimpleValue> GetStepOutput(Guid id, int ordinal);
    public Task UpdateStepOutput(Guid id, int ordinal, SimpleValue output);
    public Task ApproveStep(Guid id, int ordinal);
    public Task FinalizeStep(Guid id, int ordinal, ExecutorResponse response);
    public Task AddFeature(Guid id, JobFeature feature);
    public Task AddDeployment(DeploymentRecord deployment);
}
