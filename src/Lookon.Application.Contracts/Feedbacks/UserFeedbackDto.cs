using System;
using System.Collections.Generic;
using LookOn.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Feedbacks;

public class UserFeedbackDto
{
    public UserFeedbackDto()
    {
        Categories = new List<FeedbackCategoryType>();
    }
    public Guid?                      UserId     { get; set; }
    public decimal                    Score      { get; set; }
    public List<FeedbackCategoryType> Categories { get; set; }
    public FeedbackPage               Page       { get; set; }
    public string                     Content    { get; set; }
    public DateTime?                  CreatedAt  { get; set; }
}