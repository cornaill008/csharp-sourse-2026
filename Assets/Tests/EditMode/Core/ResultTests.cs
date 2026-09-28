using App.Core;
using NUnit.Framework;

namespace App.Tests.EditMode.Core
{
    public class ResultTests
    {
        [Test]
        public void Success_exposes_data()
        {
            var result = new Result<int, string>.Success(42);

            Assert.AreEqual(42, result.data);
        }

        [Test]
        public void Error_exposes_error()
        {
            var result = new Result<int, NetworkError>.Error(NetworkError.ServerError);

            Assert.AreEqual(NetworkError.ServerError, result.error);
        }

        [Test]
        public void Success_with_equal_data_are_structurally_equal()
        {
            var a = new Result<int, string>.Success(1);
            var b = new Result<int, string>.Success(1);

            Assert.AreEqual(a, b);
        }

        [Test]
        public void Success_with_different_data_are_not_equal()
        {
            var a = new Result<int, string>.Success(1);
            var b = new Result<int, string>.Success(2);

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Success_is_not_error()
        {
            Result<int, string> result = new Result<int, string>.Success(1);

            Assert.IsFalse(result is Result<int, string>.Error);
        }

        [Test]
        public void Pattern_matching_distinguishes_success_and_error()
        {
            Result<int, string> success = new Result<int, string>.Success(1);
            Result<int, string> error = new Result<int, string>.Error("boom");

            Assert.IsTrue(success is Result<int, string>.Success);
            Assert.IsTrue(error is Result<int, string>.Error);
        }
    }
}
