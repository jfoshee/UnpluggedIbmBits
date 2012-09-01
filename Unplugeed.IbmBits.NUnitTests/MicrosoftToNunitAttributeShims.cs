// TODO: Fork this to another assembly
using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClassAttribute : NUnit.Framework.TestFixtureAttribute {}
    public class TestMethodAttribute : NUnit.Framework.TestAttribute {}
    public class ExpectedExceptionAttribute : NUnit.Framework.ExpectedExceptionAttribute 
    {
        public ExpectedExceptionAttribute(Type exceptionType) : base(exceptionType) { }
    }
}
