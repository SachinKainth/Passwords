using System;
using System.Threading;
using NUnit.Framework;
using Password.DAL;

namespace Password.BL.IntegrationTests
{
    [TestFixture]
    class PasswordGeneratorIntegrationTests
    {
        private PasswordGenerator _passwordGenerator;
        private PasswordDAL _passwordDAL;

        [SetUp]
        public void Setup()
        {
            _passwordDAL = new PasswordDAL();
            _passwordGenerator = new PasswordGenerator(_passwordDAL);
        }

        [Test]
        public void Generate_ValidUser_ReturnsPassword()
        {
            var userName = "validuser";

            _passwordDAL.AddUser(userName);

            var password = _passwordGenerator.Generate(userName);
            Guid guid;
            var isValid = Guid.TryParse(password, out guid);

            Assert.True(isValid);
        }

        [Test]
        public void Verify_NonMatchingPassword_ReturnsFalse()
        {
            var guid1 = Guid.NewGuid().ToString();
            var guid2 = Guid.NewGuid().ToString();

            var userName = "validusername";

            _passwordDAL.AddUser(userName);
            _passwordDAL.AddPassword(userName, guid1);

            var password = guid2;
            Assert.False(_passwordGenerator.Verify(userName, password));
        }

        [Test]
        public void Verify_MatchingPassword_ReturnsTrue()
        {
            var guid1 = Guid.NewGuid().ToString();

            var userName = "validusername";

            _passwordDAL.AddUser(userName);
            _passwordDAL.AddPassword(userName, guid1);

            var password = guid1;
            Assert.True(_passwordGenerator.Verify(userName, password));
        }

        [Test]
        public void Verify_PasswordExpires_ReturnsFalse()
        {
            var passwordValidityLengthSeconds = 1;
            _passwordGenerator = new PasswordGenerator(_passwordDAL, passwordValidityLengthSeconds);

            var guid1 = Guid.NewGuid().ToString();

            var userName = "validusername";

            _passwordDAL.AddUser(userName);
            _passwordDAL.AddPassword(userName, guid1);

            Thread.Sleep(passwordValidityLengthSeconds * 1000);

            var password = guid1;
            Assert.False(_passwordGenerator.Verify(userName, password));
        }
    }
}