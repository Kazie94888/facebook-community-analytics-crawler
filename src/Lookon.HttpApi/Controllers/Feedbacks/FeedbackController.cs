using System.Threading.Tasks;
using LookOn.Feedbacks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.Feedbacks;

[RemoteService]
[Area("app")]
[ControllerName("Feedback")]
[Route("api/app/feedbacks")]

public class FeedbackController : AbpController, IUserFeedbackAppService
{
    private readonly IUserFeedbackAppService _userFeedbackAppService;

    public FeedbackController(IUserFeedbackAppService userFeedbackAppService)
    {
        _userFeedbackAppService = userFeedbackAppService;
    }

    [HttpPost]
    [Route("create-feedback")]
    public Task CreateUserFeedback(UserFeedbackDto input)
    {
        return _userFeedbackAppService.CreateUserFeedback(input);
    }
}