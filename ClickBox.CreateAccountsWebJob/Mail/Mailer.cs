using Mandrill;
using Mandrill.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
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

            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("ClickBox.CreateAccounts.azureMail.png");
            var imgBytes = new byte[imageStream.Length];
            imageStream.Read(imgBytes, 0, (int)imageStream.Length);
            var base64 = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);

            var footer = Resource.HtmlFooter.ToString();

            var images = new[]{
                        new Image
                            {
                                Name = "qcatlogo",
                                Type = "image/png",
                                Content = base64
                            }};

            var email = new EmailMessage
            {
                To = new List<EmailAddress>(new[] { to }),
                FromEmail = msg.From,
                FromName = msg.FromName,
                Text = msg.MessageBody + "</br> " + msg.DowloadLink,
                Html = msg.MessageBody + "</br> </br> " + msg.DowloadLink + "</br> </br> " + footer,
                Subject = "QCAT PageMaker Trial, welcome!",
                
            };
            email.Images = images;

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
