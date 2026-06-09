namespace MZSimpleDynamicLinq.Core.Test
{
    public class ToExpressionTest
    {
        [Fact]
        public void Test1()
        {
            var f = new Filter()
            {
                Logic = "and",
                Filters = [
                    new Filter(){
                        Field="CardStateTitle",
                        Operator="contains",
                        Value="فعال"
                    }
                    ]
            };
            var filters = f.GetFlat();
            var h=f.ToExpression(filters);
            string expected = "CardStateTitle contains 'فعال'";
            Assert.True(expected== h, $"Expected `{expected}` but received `{h}`");
        }
    }
}
