using Engine.Pipeline;

namespace Tests.Unit.Pipeline
{
    public class EngineGroupTestFixture
    {
        [Fact]
        public void TestIsInTime()
        {
            Assert.True(EngineGroup.IsInTime(new[] { 0, 0 }, 0, -4));
            Assert.False(EngineGroup.IsInTime(new[] { 0, 0 }, 0, -151));
            Assert.True(EngineGroup.IsInTime(new[] { 0, 0 }, 0, 4));
            Assert.False(EngineGroup.IsInTime(new[] { 0, 0 }, 0, 151));

            Assert.True(EngineGroup.IsInTime(new[] { 0, int.MinValue + 1 }, 0, int.MaxValue - 1));
            Assert.False(EngineGroup.IsInTime(new[] { 0, int.MinValue + 152 }, 0, int.MaxValue));
            Assert.True(EngineGroup.IsInTime(new[] { 0, int.MaxValue - 1 }, 0, int.MinValue + 1));
            Assert.False(EngineGroup.IsInTime(new[] { 0, int.MaxValue }, 0, int.MinValue + 152));
        }
    }
}