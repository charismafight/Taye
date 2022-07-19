using Taye.Utilities;

namespace Taye.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestVideoHelper()
        {
            //var result = VideoHelper.TestGetFrame(@"C:\Users\Lee\source\repos\Taye\Taye.Tests\TestFiles\StartMenu_Win10.mp4", @"D:\test.png");
            VideoHelper.GetFrame(@"C:\Users\Lee\source\repos\Taye\Taye.Tests\TestFiles\StartMenu_Win10.mp4", @"D:\test.png");
        }
    }
}