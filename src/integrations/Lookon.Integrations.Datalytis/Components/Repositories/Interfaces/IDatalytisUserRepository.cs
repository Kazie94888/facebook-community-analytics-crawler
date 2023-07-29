using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LookOn.Integrations.Datalytis.Models.Entities;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;

public interface IDatalytisUserRepository : IRepository<DatalytisUser, Guid>
{
    Task<List<DatalytisUser>> GetPageUsers(List<string>                        communityPageIds,
                                           IList<KeyValuePair<string, string>> phoneNosEmails,
                                           CancellationToken                   cancellationToken = default);

    Task<List<DatalytisUser>> GetUsersNotMetPhoneNosAndEmails(List<string>                        communityPageIds,
                                                              IList<KeyValuePair<string, string>> phoneNosEmails,
                                                              CancellationToken                   cancellationToken = default);

    Task<long> CountUsers(List<string> communityPageIds, CancellationToken cancellationToken = default);
}