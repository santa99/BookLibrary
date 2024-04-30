using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Api.Tests;

public abstract class TestBase
{
    protected readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization
        { ConfigureMembers = true });
}