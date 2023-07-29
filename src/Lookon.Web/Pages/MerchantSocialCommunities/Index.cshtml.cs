using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace LookOn.Web.Pages.MerchantSocialCommunities
{
    public class SocialCommunities : LookOnPageModel
    {
        public string MerchantIdFilter { get; set; }
        [SelectItems(nameof(HasCommunityIdFilterItems))]
        public string HasCommunityIdFilter { get;                    set; }
        public List<SelectListItem> HasCommunityIdFilterItems { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("", ""), new SelectListItem("Yes", "true"), new SelectListItem("No", "false"),
        };

        public SocialCommunities()
        {
        }

        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}