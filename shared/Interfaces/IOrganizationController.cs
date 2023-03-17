using shared.Models;
using shared.UpdateModels;
using shared.View;

namespace shared.Interfaces;

public interface IOrganizationController
{
    public Task<List<Organization>> GetOrgs();
    public Task<Organization> GetOrg(Guid id);
    public Task<Organization> Create(UpdateOrganization body);
    public Task<Organization> Update(Guid id, UpdateOrganization body);
    public Task<Organization> UpdateMetadata(Guid id, Dictionary<string,string> body);
    public Task<Organization> UpdatePrivateMetadata(Guid id, Dictionary<string,string> body);
    public Task<Organization> UpdatePolicy(Guid id, string body);
    public Task<string> UpdateVariable(Guid id, UpdateOrganizationVariable variable);
    public Task<List<Credential>> GetCredentials(Guid id);
    public Task<Credential> CreateCredential(Guid id, Credential credential);
    public Task<Credential> GetCredential(Guid id);
    public Task<CredentialBundle> GetCredentialBundle(Guid id);
    public Task UpdateCredentialData(Guid id, Dictionary<string,string> data);
}
