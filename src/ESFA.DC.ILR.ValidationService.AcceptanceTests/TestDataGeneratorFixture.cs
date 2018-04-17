using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class TestDataGeneratorFixture
    {
        private List<ILearnerMultiMutator> _functors = new List<ILearnerMultiMutator>(100);
        private const int UKPRN = 90000064;
        private DataCache _dataCache;

        public TestDataGeneratorFixture()
        {
            this._functors = new List<ILearnerMultiMutator>(100);
            this._dataCache = new DataCache();
            this.FunctorParser = new RuleToFunctorParser(this._dataCache);
            this.FunctorParser.CreateFunctors(this.AddFunctor);
            this.Generator = new XmlGenerator(this.FunctorParser, UKPRN);
        }

        public XmlGenerator Generator { get; private set; }

        public RuleToFunctorParser FunctorParser { get; private set; }

        private void AddFunctor(ILearnerMultiMutator i)
        {
            this._functors.Add(i);
        }
    }
}
