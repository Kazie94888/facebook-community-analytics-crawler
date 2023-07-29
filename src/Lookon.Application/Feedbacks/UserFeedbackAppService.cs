using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Feedbacks;

public class UserFeedbackAppService : LookOnAppService, IUserFeedbackAppService
{
    private readonly IRepository<UserFeedback> _userFeedbackRepository;

    public UserFeedbackAppService(IRepository<UserFeedback> userFeedbackRepository)
    {
        _userFeedbackRepository = userFeedbackRepository;
    }

    public async Task CreateUserFeedback(UserFeedbackDto input)
    {
        var userFeedback = ObjectMapper.Map<UserFeedbackDto, UserFeedback>(input);
        userFeedback.CreatedAt = DateTime.UtcNow;
        await _userFeedbackRepository.InsertAsync(userFeedback);
    }
}