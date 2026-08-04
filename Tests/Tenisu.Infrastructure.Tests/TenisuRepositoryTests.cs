using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Tests
{
    [Category("Integration")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class TenisuRepositoryTests : BaseRepositoryTests
    {
    }
}
