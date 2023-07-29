using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Feedbacks;

public interface IUserFeedbackAppService : IApplicationService
{
    public Task CreateUserFeedback(UserFeedbackDto input);
}