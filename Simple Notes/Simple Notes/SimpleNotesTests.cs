using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace Simple_Notes
{
    public class SimpleNotesTests
    {
        private IWebDriver driver;
        private Random random;
        private static readonly string BaseUrl = "https://d5wfqm7y6yb3q.cloudfront.net/";

        string lastCreatedNoteName = "";
        string lastCreatedNoteDescription = "";


        [OneTimeSetUp]

        public void Setup()
        {
            random = new Random();

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl(BaseUrl);

            var joinButton = driver.FindElement(By.CssSelector("[class='btn btn-outline-light btn-lg mt-5']"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(joinButton).Perform();
            joinButton.Click();


            driver.Navigate().GoToUrl(BaseUrl + "User/LoginRegister");

            var loginButton = driver.FindElement(By.XPath("//a[@id='tab-login']"));
            actions = new Actions(driver);
            actions.MoveToElement(loginButton).Perform();
            loginButton.Click();

            driver.FindElement(By.XPath("//input[@id='loginName']")).SendKeys("test777@test.bg");
            driver.FindElement(By.XPath("//input[@id='loginPassword']")).SendKeys("Doidohvidqhpobedih5");
            driver.FindElement(By.CssSelector("[class='btn btn-primary btn-block mb-4']")).Click();
        }

        [Test, Order(1)]
        public void AddNoteWithInvalidData()
        {
            //Arrange
            var title = "";
            var description = "";


            //Act
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var noteAddedButtonCards = wait.Until(driver => driver.FindElements(By.CssSelector("[class='btn btn-info']")));
            driver.FindElement(By.CssSelector("[class='btn btn-info']")).Click();
            driver.FindElement(By.CssSelector("[name='Title']")).SendKeys(title);
            driver.FindElement(By.CssSelector("[name='Description']")).SendKeys(description);

            driver.FindElement(By.XPath("//button[@class='btn btn-info btn-block mb-4 col-6']")).Click();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            //Assert
            Assert.That(errorMessage, Is.EqualTo("The Title field is required. The Description field is required."));
            Assert.That(driver.Url, Is.EqualTo(BaseUrl + "Note/Create"));
        }

        [Test, Order(2)]

        public void AddRandomNote()
        {
            //Arrange
            lastCreatedNoteName = "Title_" + random.Next(999, 99999).ToString();
            lastCreatedNoteDescription = "AddedSomeLongNoteDescription_" + random.Next(999, 99999).ToString();

            driver.Navigate().GoToUrl(BaseUrl + "Note/New");
            driver.FindElement(By.CssSelector("[class='btn btn-info']")).Click();

            //Act
            driver.FindElement(By.CssSelector("[name='Title']")).SendKeys(lastCreatedNoteName);
            driver.FindElement(By.CssSelector("[name='Description']")).SendKeys(lastCreatedNoteDescription);
            new SelectElement(driver.FindElement(By.Name("Status"))).SelectByText("New");

            driver.FindElement(By.XPath("//button[@class='btn btn-info btn-block mb-4 col-6']")).Click();

            //Asserts

            var successMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;
            Assert.That(successMessage, Is.EqualTo("Note created successfully!"));
        }

        [Test, Order(3)]

        public void EditLastAddedNote()
        {
            //Arrange
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");
            Assert.IsNotNull(lastCreatedNoteName, "No title set for the last created idea.");

            string newTitle = "Changed Title: " + lastCreatedNoteName;

            driver.FindElement(By.XPath("(//section[@class='p-4 d-flex justify-content-center text-center w-100'])[last()]"));


            var lastNoteEditButton = driver.FindElement(By.XPath("(//a[@class='btn btn-info'])[last()]"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(lastNoteEditButton).Perform();

            lastNoteEditButton.Click();

            //Act
            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(newTitle);
            driver.FindElement(By.XPath("//button[text()='Edit']")).Click();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var homePageElement = wait.Until(ExpectedConditions.UrlToBe(BaseUrl + "Home/Main"));

            //Assert
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");
            var lastCreatedNoteNameText = driver.FindElement(By.XPath("(//div[@class='card-body'])[last()]//h5")).Text;
            Assert.That(lastCreatedNoteNameText, Is.EqualTo(newTitle));
        }

        [Test, Order(4)]

        public void MoveEditedNotetoDone()
        {
            //Arrange
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");
            Assert.IsNotNull(lastCreatedNoteName, "No title set for the last created idea.");

            driver.FindElement(By.XPath("(//section[@class='p-4 d-flex justify-content-center text-center w-100'])[last()]"));

            //Act

            var lastNoteSetToDoneButton = driver.FindElement(By.XPath("(//a[@class='btn btn-primary'])[last()]"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(lastNoteSetToDoneButton).Perform();

            lastNoteSetToDoneButton.Click();

            //Asserts

            var successMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;
            Assert.That(successMessage, Is.EqualTo("Note status changed successfully!"));
        }

        [Test, Order(5)]

        public void DeleteEditedNote()
        {
            //Arrange
            driver.Navigate().GoToUrl(BaseUrl + "Note/Done");
            Assert.IsNotNull(lastCreatedNoteName, "No title set for the last created idea.");

            driver.FindElement(By.XPath("(//section[@class='p-4 d-flex justify-content-center text-center w-100'])[last()]"));

            //Act

            var lastNoteSetToDoneButton = driver.FindElement(By.XPath("(//a[@class='btn btn-danger'])[last()]"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(lastNoteSetToDoneButton).Perform();

            lastNoteSetToDoneButton.Click();

            //Asserts

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var deleteConfirmPage = wait.Until(driver => driver.FindElements(By.CssSelector("[class='display-4']")));

            driver.FindElement(By.CssSelector("[class='btn btn-info btn-block mb-4 col-6']")).Click();

            var deletedMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(deletedMessage, Is.EqualTo("Note deleted successfully!"));
        }

        [Test, Order(6)]

        public void Logout()
        {
            //Arrange 
            //Act
            driver.FindElement(By.XPath("//i[@class='fa-solid fa-right-from-bracket']")).Click();

            //Assert
            Assert.That(driver.Url, Is.EqualTo(BaseUrl));
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            var deniedMessage = driver.FindElement(By.TagName("pre")).Text;

            Assert.That(deniedMessage, Is.EqualTo("Access Denied"));
        }

        private string GenerateRandomString(string prefix)
        {
            random = new Random();

            return prefix + random.Next(999, 99999).ToString();
        }

        [OneTimeTearDown]

        public void TearDown()
        {

            driver.Close();
            driver.Dispose();
        }
    }
}