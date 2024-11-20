using Moq;
using NUnit.Framework;
using SyncTask.Interfaces;
using SyncTask.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Services.Tests
{
    [TestFixture]
    public class SyncRuntimeManagerTests
    {

        private SyncRuntimeManager _cut;
        private Arguments _args;
        private Mock<IUserInputChecker> _inputChecker;

        [SetUp]
        public void Setup()
        {
            _args = new Arguments("source", "target", "log", 0.2f);
            _inputChecker = new Mock<IUserInputChecker>();
            _cut = new SyncRuntimeManager(_args, _inputChecker.Object);
            
        }

        #region StartSyncing method tests

        [Test]
        public void StartSyncing_ShouldCallSetupDependenciesOnlyOnce_WhenUserInputIsEnteredAfterThreeTries()
        {
            _inputChecker.SetupSequence(mock => mock.UserPressedKey())
                 .Returns(false)
                 .Returns(false)
                 .Returns(false)
                 .Returns(true);

            _cut.StartSyncing();

            _inputChecker.Verify(mock => mock.UserPressedKey(), Times.Exactly(4));
        }

        #endregion

    }
}
