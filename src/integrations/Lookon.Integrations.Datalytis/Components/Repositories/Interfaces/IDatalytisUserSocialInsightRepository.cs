using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Integrations.Datalytis.Models.Entities;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;

public interface IDatalytisUserSocialInsightRepository : IRepository<DatalytisUserSocialInsight, Guid>
{
    Task<List<DatalytisUserSocialInsight>> Get(List<string> communityUids);
}