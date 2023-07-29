using System.Collections.Generic;

namespace LookOn.Emails;
public class EmailSendingArgs
{
    public EmailSendingArgs()
    {
        CCEmailAddress = new List<string>();
    }
    public EmailSendingArgs(string toEmailAddress, string subject, string body, bool isBodyHtml = true)
    {
        ToEmailAddress = toEmailAddress;
        Subject        = subject;
        Body           = body;
        IsBodyHtml     = isBodyHtml;
        
        CCEmailAddress = new List<string>();
    }
    public string       ToEmailAddress { get; set; }
    public List<string> CCEmailAddress { get; set; }
    public string       Subject        { get; set; }
    public string       Body           { get; set; }
    
    public bool IsBodyHtml { get; set; }
}