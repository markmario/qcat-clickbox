using ClickBox.Email;
using Mandrill;
using Mandrill.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ClickBox.Mail
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
            var imageStream = assembly.GetManifestResourceStream("ClickBox.Email.azureMail.png");
            var imgBytes = new byte[imageStream.Length];
            imageStream.Read(imgBytes, 0, (int)imageStream.Length);
            var base64 = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);

            var html = Resources.TrialSuccessHtml;

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
                                BccAddress = msg.From,
                                FromEmail = msg.From,
                                FromName = msg.FromName,
                                Text =
                                    msg.MessageBody + Environment.NewLine + msg.Instructions
                                    + Environment.NewLine + msg.DowloadLink + Environment.NewLine
                                    + Environment.NewLine + "License Name: " + msg.LicenseName
                                    + Environment.NewLine + "Password: " + msg.Password + Environment.NewLine,
                                Html = html,
                                Subject = "QCAT " + msg.ProductName + " " + msg.LicenseName + "!",
                                Images = images,
                                MergeLanguage = "handlebars"
                            };

            email.AddGlobalVariable("welcome", msg.MessageBody);
            email.AddGlobalVariable("instructions", msg.Instructions);
            email.AddGlobalVariable("downloadlink", msg.DowloadLink);
            email.AddGlobalVariable("password", msg.Password);
            email.AddGlobalVariable("licenseName", msg.To);
            email.AddGlobalVariable("paymentReceived", msg.PaymentReceived);

            var response = await api.SendMessage(new Mandrill.Requests.Messages.SendMessageRequest(email));

            return response[0].Status == EmailResultStatus.Sent;
        }

        public static async Task<bool> SendMonthlyOdesReports<T>(T msg) where T : IAmTheWeeklyStatsForTheMonthOdesReport
        {
            var api = new MandrillApi(Program.MandrillKey, true);

            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("ClickBox.Email.azureMail.png");
            var imgBytes = new byte[imageStream.Length];
            imageStream.Read(imgBytes, 0, (int)imageStream.Length);
            var base64 = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);

            var html = Resources.OdesWeeklyStatsReport;

            var images = new[]{
                        new Image
                            {
                                Name = "qcatlogo",
                                Type = "image/png",
                                Content = base64
                            }};

            var email = new EmailMessage
            {
                To = new List<EmailAddress>(msg.To),
                BccAddress = msg.From,
                FromEmail = msg.From,
                FromName = msg.FromName,
                Text = "Usage Report",
                Html = html,
                Subject = "QCAT Objective Koding Weekly Stastistic Report",
                Images = images,
                MergeLanguage = "handlebars"
            };

            email.AddGlobalVariable("allBatchCount", msg.AllBatchesCount);
            email.AddGlobalVariable("allDocCount", msg.AllDocumentsCount);
            email.AddGlobalVariable("monthlyBatches", msg.MonthlyBatchLines);
            email.AddGlobalVariable("monthlyDocuments", msg.MonthlyDocumentLines);
            email.AddGlobalVariable("monthlyBatchesAsString", msg.MonthlyBatchLinesAsString);
            email.AddGlobalVariable("monthlyDocumentsAsString", msg.MontlyDocumentLinesAsString);

            var response = await api.SendMessage(new Mandrill.Requests.Messages.SendMessageRequest(email));

            return response[0].Status == EmailResultStatus.Sent;
        }

    }
}
