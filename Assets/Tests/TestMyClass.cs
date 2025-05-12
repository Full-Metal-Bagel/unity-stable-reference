using NUnit.Framework;

public class TestMyClass
{
    [Test]
    public void should_add_two_int()
    {
        Assert.That(MyClass.Add(0, 0), Is.EqualTo(0));
    }
}
