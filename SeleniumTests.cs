using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Metrics;
using static System.Net.Mime.MediaTypeNames;

namespace SeleniumWebdriverTask1
{
    [TestFixture]
    public class Tests
    {
        private IWebDriver driver { get; set; }
        private WebDriverWait wait;
        public static TestContext TestContext { get; set; }

        public static string ApplicationUrl => TestContext.Parameters.Get("ApplicationUrl").ToString();

        [SetUp]
        public void Setup()
        {
            // Initialize Chrome driver
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); // Set implicit wait for elements
            
            // Initialize explicit wait
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestCase("C#", "All Locations")]
        [TestCase("Java", "All Locations")]
        [TestCase("Python", "All Locations")]
        public void SearchForJobPosition(string keyword, string location)
        {
            // Navigate to the website
            driver.Navigate().GoToUrl(ApplicationUrl);

            // Find the "Accept All" cookies button using ID locator and click on it
            IWebElement acceptButton = wait.Until<IWebElement>((d) => d.FindElement(By.Id("onetrust-accept-btn-handler")));
            acceptButton.Click();

            //Find the "Careers" link using LinkText locator and click on it
            IWebElement careersLink = driver.FindElement(By.LinkText("Careers"));
            careersLink.Click();

            // Enter the keyword in the "Keywords" field using ID locator locator
            IWebElement keywordInput = driver.FindElement(By.Id("new_form_job_search-keyword"));
            keywordInput.Clear();
            keywordInput.SendKeys(keyword);

            // Select location "All Locations" using XPath locator
            // Step 1: Locate the <span> element.
            IWebElement locationSpan = driver.FindElement(By.XPath("//span[@class='select2-selection__rendered' and @title='Kyiv']"));
            // Step 2: Click the <span> to open the dropdown.
            locationSpan.Click();
            // Step 3: Wait for the "All Locations" option to be clickable.
            IWebElement allLocationsOption = wait.Until<IWebElement>((d) => d.FindElement(By.XPath("//li[contains(.,'" + location + "')]")));
            allLocationsOption.Click();
            keywordInput.SendKeys(Keys.Escape);

            // Select "Remote" option using XPath locator with operator
            IWebElement remoteOption = wait.Until<IWebElement>((d) => d.FindElement(By.XPath("//label[contains(text(),'Remote')]")));
            remoteOption.Click();

            // Click on the "Find" button using CssSelector locator
            IWebElement findButton = driver.FindElement(By.CssSelector("button.job-search-button-transparent-23"));
            findButton.Click();

            //Find the latest element in the list of results
            // Step 1: Locate all elements with the class 'search-result__item'
            IList<IWebElement> searchResultItems = driver.FindElements(By.CssSelector("li.search-result__item"));
            // Step 2: Select the last item in the list
            IWebElement lastItem = searchResultItems.Last();
            // Step 3: Click the 'View and apply' button within the last item
            IWebElement viewAndApplyButton = lastItem.FindElement(By.CssSelector("a.search-result__item-apply-23"));
            viewAndApplyButton.Click();

            // Validate keyword is present on the page
            IWebElement element = driver.FindElement(By.XPath("//*[contains(.,'" + keyword + "')]"));
            Assert.That(element.Displayed);
        }

        [TearDown]
        public void Teardown()
        {
            driver.Close();
            driver.Quit();
        }
    }
}