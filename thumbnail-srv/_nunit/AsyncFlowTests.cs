using NUnit.Framework;

namespace ThumbnailSrv
{
    [TestFixture]
    class common_use_cases
    {
        [Test]
        public void Notify_data_is_ready()
        {
            Assert.Fail();
        }

        [Test]
        public void Signal_on_task_completion()
        {
            Assert.Fail();
        }

        [Test]
        public void Cleanup()
        {
            Assert.Fail();
        }
    }

    [TestFixture]
    class spec_cases
    {
        [Test]
        public void A_OnReady_callback_is_called_whenever_it_is_registred()
        {
            Assert.Fail();
        }

        [Test]
        public void All_registered_callbacks_are_called()
        {
            Assert.Fail();
        }

        [Test]
        public void Already_registered_callbacks_are_called_after_Cleanup()
        {
            Assert.Fail();
        }
    }
}