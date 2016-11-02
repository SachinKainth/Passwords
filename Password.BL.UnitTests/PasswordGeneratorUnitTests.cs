using System;
using Moq;
using NUnit.Framework;
using Password.DAL;

namespace Password.BL.UnitTests
{
    [TestFixture]
    class PasswordGeneratorUnitTests
    {
        private PasswordGenerator _passwordGenerator;
        private Mock<IPasswordDAL> _passwordDALMock;
        private Mock<UserPasswordRecord> _userPasswordRecordMock;

        [SetUp]
        public void Setup()
        {
            _passwordDALMock = new Mock<IPasswordDAL>();
            _passwordGenerator = new PasswordGenerator(_passwordDALMock.Object);
            _userPasswordRecordMock = new Mock<UserPasswordRecord>();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("         ")]
        [TestCase(null)]
        public void Generate_InvalidUserName_ThrowsException(string userName)
        {
            var ex = Assert.Throws<BadUserNameException>(() => _passwordGenerator.Generate(userName));

            Assert.That(ex.Message, Is.EqualTo("User name '" + userName + "' is not valid."));
        }

        [Test]
        public void Generate_NonExistantUserName_ThrowsException()
        {
            var userName = "badusername";
            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns((UserPasswordRecord)null);

            var ex = Assert.Throws<BadUserNameException>(() => _passwordGenerator.Generate(userName));

            Assert.That(ex.Message, Is.EqualTo("User name '" + userName + "' does not exist."));
        }

        [Test]
        public void Generate_ValidUserName_GeneratesPassword()
        {
            var userName = "validusername";
            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(new UserPasswordRecord(userName));

            Guid guid;
            var password = _passwordGenerator.Generate(userName);
            var isValid = Guid.TryParse(password, out guid);

            Assert.True(isValid);
            _passwordDALMock.Verify(m => m.AddPassword(userName, password));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("         ")]
        [TestCase(null)]
        public void Verify_InvalidUserName_ThrowsException(string userName)
        {
            var ex = Assert.Throws<BadUserNameException>(() => _passwordGenerator.Verify(userName, null));
            Assert.That(ex.Message, Is.EqualTo("User name '" + userName + "' is not valid."));
        }

        [Test]
        public void Verify_NonExistantUserName_ThrowsException()
        {
            var userName = "badusername";
            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns((UserPasswordRecord)null);

            var ex = Assert.Throws<BadUserNameException>(() => _passwordGenerator.Verify(userName, null));
            Assert.That(ex.Message, Is.EqualTo("User name '" + userName + "' does not exist."));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("         ")]
        [TestCase(null)]
        public void Verify_ValidUserNameInvalidPassword_ThrowsException(string password)
        {
            var userName = "validusername";
            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(new UserPasswordRecord(userName));

            var ex = Assert.Throws<BadPasswordException>(() => _passwordGenerator.Verify(userName, password));
            Assert.That(ex.Message, Is.EqualTo("Password '" + password + "' is not valid."));
        }

        [Test]
        public void Verify_ValidUserNameNonGuidPassword_ThrowsException()
        {
            var userName = "validusername";
            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(new UserPasswordRecord(userName));

            var nonguidpassword = "nonguidpassword";
            var ex = Assert.Throws<BadPasswordException>(() => _passwordGenerator.Verify(userName, nonguidpassword));
            Assert.That(ex.Message, Is.EqualTo("Password '" + nonguidpassword + "' must be a valid Guid."));
        }

        [Test]
        public void Verify_ValidUserNameNonMatchingPassword_ReturnsFalse()
        {
            var guid1 = Guid.NewGuid().ToString();
            var guid2 = Guid.NewGuid().ToString();

            var userName = "validusername";

            _userPasswordRecordMock.SetupGet(m => m.UserName).Returns(userName);
            _userPasswordRecordMock.SetupGet(m => m.Password).Returns(guid1);

            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(_userPasswordRecordMock.Object);

            var password = guid2;
            Assert.False(_passwordGenerator.Verify(userName, password));
        }

        [Test]
        public void Verify_ValidUserNameMatchingButExpiredPassword_ReturnsFalse()
        {
            var guid1 = Guid.NewGuid().ToString();
            var userName = "validusername";

            _userPasswordRecordMock.SetupGet(m => m.TimeStamp).Returns(DateTime.UtcNow.AddSeconds(-31));
            _userPasswordRecordMock.SetupGet(m => m.UserName).Returns(userName);
            _userPasswordRecordMock.SetupGet(m => m.Password).Returns(guid1);

            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(_userPasswordRecordMock.Object);

            var password = guid1;
            Assert.False(_passwordGenerator.Verify(userName, password));
        }

        [Test]
        public void Verify_ValidUserNameMatchingNonExpiredPassword_ReturnsTrue()
        {
            var userName = "validusername";
            var guid1 = Guid.NewGuid().ToString();

            _userPasswordRecordMock.SetupGet(m => m.TimeStamp).Returns(DateTime.UtcNow.AddSeconds(-29));
            _userPasswordRecordMock.SetupGet(m => m.UserName).Returns(userName);
            _userPasswordRecordMock.SetupGet(m => m.Password).Returns(guid1);

            _passwordDALMock.Setup(m => m.GetUser(userName)).Returns(_userPasswordRecordMock.Object);

            var password = guid1;
            Assert.True(_passwordGenerator.Verify(userName, password));
        }
    }
}