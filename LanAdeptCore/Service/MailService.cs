using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Service
{
	public static class MailService
	{
		public static void SendMail(string emailTo, string subject, string content)
		{
			MailMessage message = new MailMessage();
			message.To.Add(new MailAddress(emailTo));
			message.From = new MailAddress("DoNotReply@LanAdept.ca");
			message.Subject = subject;
			message.IsBodyHtml = true;

			using (var smtp = new SmtpClient("localhost"))
			{
				smtp.Send(message);
				Console.WriteLine("TEST");
			}
		}
	}
}
