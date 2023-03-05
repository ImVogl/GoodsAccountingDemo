using GoodsAccounting.Services.Password;
using GoodsAccounting.Services.Validator;
using NUnit.Framework;

namespace GoodsAccounting.Tests
{
    public class PasswordServiceTests
    {
        private static readonly IPassword PasswordService = new PasswordService(new Validator());

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Description("Testing for password generation")]
        public void GeneratePasswordTest()
        {
            var generatedPassword = PasswordService.GeneratePassword();
            Assert.IsNotNull(generatedPassword);
            Assert.IsNotEmpty(generatedPassword);
            Assert.That(generatedPassword.Length == 12);
        }

        [Test]
        [Description("Verify password function argument checking")]
        public void VerifyArgumentsTest()
        {
#pragma warning disable CS8625
            const string validPassword = "Az!2.sssA";
            Assert.Throws<ArgumentNullException>(() => PasswordService.VerifyPassword(null, validPassword, "1234"));
            Assert.Throws<ArgumentNullException>(() => PasswordService.VerifyPassword(new byte[] { 1, 2, 3 }, validPassword, null));
#pragma warning restore CS8625
            Assert.Throws<ArgumentNullException>(() => PasswordService.VerifyPassword(Array.Empty<byte>(), validPassword, "1234"));
            Assert.Throws<ArgumentNullException>(() => PasswordService.VerifyPassword(new byte[]{ 1, 2, 3}, validPassword, string.Empty));
        }

        [Test]
        [Description("Test hash function calculate value")]
        public void HashCalculatingTest()
        {
            const string salt = "26774df3-e5ac-4d09-bcc9-39e8811fc36e";
            const string password = "56769cAf-ae4e-43ba-8c73-119fd0baef76";
            const string notSuitablePassword = "Az!2.sssA";
            var expectedHash = new byte[]
            {
                236, 241, 119, 4, 227, 152, 147, 88, 5, 102, 69, 178, 84, 222, 162, 125, 98, 199, 204, 17, 245, 33, 112,
                249, 200, 70, 161, 122, 26, 169, 200, 228
            };

            var (_, hash) = PasswordService.Hash(password, salt);
            Assert.That(hash.Length, Is.EqualTo(expectedHash.Length));
            for (var i = 0; i < expectedHash.Length; i++)
                Assert.That(hash[i], Is.EqualTo(expectedHash[i]));

            var (_, newHash) = PasswordService.Hash(notSuitablePassword, salt);
            for (var i = 0; i < expectedHash.Length; i++)
                Assert.That(newHash[i], Is.Not.EqualTo(expectedHash[i]));
        }

        [Test]
        [Description("Test password verification")]
        public void VerificationTest()
        {
            const string salt = "26774df3-e5ac-4d09-bcc9-39e8811fc36e";
            const string password = "56769cAf-ae4e-43ba-8c73-119fd0baef76";
            const string notSuitablePassword = "Az!2.sssA";
            var hash = new byte[]
            {
                236, 241, 119, 4, 227, 152, 147, 88, 5, 102, 69, 178, 84, 222, 162, 125, 98, 199, 204, 17, 245, 33, 112,
                249, 200, 70, 161, 122, 26, 169, 200, 228
            };

            Assert.True(PasswordService.VerifyPassword(hash, password, salt));
            Assert.False(PasswordService.VerifyPassword(hash, notSuitablePassword, salt));
        }
    }
}