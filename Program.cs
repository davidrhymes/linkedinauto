using System;
using OpenQA.Selenium;
using System.IO;
using OpenQA.Selenium.Edge;
using System.Net;
using System.Threading;

namespace LinkedInAuto
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//File.Delete("Cookies.txt");

			EdgeOptions options = new EdgeOptions();

			options.AddArgument("--proxy-server=http://35.185.196.38:3128");

			var driver = new EdgeDriver(options);

			//var driver = new EdgeDriver();


			if (!File.Exists("Cookies.txt"))
			{
				Console.WriteLine("Please enter your email:");
				string email = Console.ReadLine();
				Console.WriteLine("Please enter your password:");
				string password = Console.ReadLine();

				driver.Navigate().GoToUrl("https://www.linkedin.com");


				IWebElement emailInput = driver.FindElement(By.Id("session_key"));
				IWebElement passwordInput = driver.FindElement(By.Id("session_password"));

				emailInput.SendKeys(email);
				passwordInput.SendKeys(password);

				passwordInput.SendKeys(Keys.Enter);

				var cookies = driver.Manage().Cookies.AllCookies;
				using (StreamWriter sw = new StreamWriter("Cookies.txt"))
				{
					foreach (var cookie in cookies)
					{
						sw.WriteLine($"{cookie.Name},{cookie.Value},{cookie.Domain},{cookie.Path},{cookie.Expiry}");
					}
				}
			}
			else
			{
				driver.Navigate().GoToUrl("https://www.linkedin.com");
				using (StreamReader sr = new StreamReader("Cookies.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						var parts = line.Split(',');

						DateTime? expiry = parts[4] == "" ? DateTime.Now.AddDays(7) : DateTime.Parse(parts[4]);

						driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(parts[0], parts[1], parts[2], parts[3], expiry));
					}
				}

				driver.Navigate().Refresh();
			}

			string img = driver.FindElement(By.ClassName("feed-identity-module__member-photo")).GetAttribute("src");

			if (img != null)
			{
				using (WebClient client = new WebClient())
				{
					client.DownloadFile(img, "img.png");
				}
			}

			driver.Quit();
		}
	}
}
