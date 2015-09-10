using ClickBox.CreateAccounts.Messages;
using Mandrill;
using Mandrill.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickBox.CreateAccounts.Mail
{
    public class Mailer
    {
        public static async Task<bool> SendMail<T>(T msg) where T : IHaveDataForMail
        {
            var api = new MandrillApi(Program.MandrillKey, true);
            var to = new EmailAddress();
            to.Email = msg.To;
            to.Name = msg.ContactName;

            var email = new EmailMessage
            {
                To = new List<EmailAddress>(new[] { to }),
                FromEmail = msg.From,
                FromName = msg.FromName,
                Text = msg.MessageBody + "</br>" + msg.DowloadLink,
                Html = msg.MessageBody + "</br>" + msg.DowloadLink,
                Subject = "QCAT PageMaker Trial, welcome!",

            };
            var response = await api.SendMessage(new Mandrill.Requests.Messages.SendMessageRequest(email));
            if (response[0].Status == EmailResultStatus.Sent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
