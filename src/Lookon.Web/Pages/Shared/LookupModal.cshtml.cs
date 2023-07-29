using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LookOn.Web.Pages.Shared
{
    public class LookupModal : LookOnPageModel
    {
        public string CurrentId { get; set; }
        public string CurrentDisplayName { get; set; }

        public Task OnGetAsync(string currentId, string currentDisplayName)
        {
            CurrentId = currentId;
            CurrentDisplayName = currentDisplayName;
            return Task.CompletedTask;
        }
    }
}