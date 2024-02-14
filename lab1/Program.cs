using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

var driver = new ChromeDriver();

driver.Url = "https://www.google.com";

Thread.Sleep(1000);

driver.Quit();
