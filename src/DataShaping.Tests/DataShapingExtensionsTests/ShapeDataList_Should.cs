using AutoFixture;
using DataShaping;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Xunit;
// ReSharper disable All

namespace DataShaping.Tests.DataShapingExtensionsTests.IEnumerableExtensionsTests
{
    public class ShapeDataList_Should
    {
        private IEnumerable<Sample> _samples;

        public ShapeDataList_Should()
        {
            var fixture = new Fixture();
            _samples = fixture.CreateMany<Sample>(5);
        }

        [Fact]
        public void Return_Empty_Result()
        {
            var shape = DataShapingExtensions.ShapeDataList<Sample>(null, null);

            shape.ShouldBeEmpty();
        }

        [Fact]
        public void Return_List_Of_Dictionary_With_All_Fields_If_Fields_Empty()
        {
            var shape = DataShapingExtensions.ShapeDataList(_samples, string.Empty);

            shape.ShouldNotBeEmpty();
            shape.Count.ShouldBe(1);

            var shapedList = shape.Single().Value as IEnumerable<IDictionary<string,object>>;

            shapedList.ShouldNotBeEmpty();

            var list = shapedList.ToList();

            list.Count.ShouldBe(5);

            list.First().Values.Count.ShouldBe(3);
        }

        [Fact]
        public void Return_List_Of_Dictionary_With_Required_Field()
        {
            var shape = DataShapingExtensions.ShapeDataList(_samples, nameof(Sample.BirthDate));

            shape.ShouldNotBeEmpty();
            shape.Count.ShouldBe(1);

            var shapedList = shape.Single().Value as IEnumerable<IDictionary<string,object>>;

            shapedList.ShouldNotBeEmpty();

            var list = shapedList.ToList();

            list.Count.ShouldBe(5);

            list.First().Values.Count.ShouldBe(1);
            list.First().Keys.Single().ShouldBe(nameof(Sample.BirthDate));
        }

        [Fact]
        public void Return_List_Of_Dictionary_With_Required_Fields()
        {
            var shape = (DataShapingExtensions.ShapeDataList(_samples, $"{nameof(Sample.Name)},{nameof(Sample.BirthDate)}")
                .Single().Value as IEnumerable<IDictionary<string,object>>).ToList();

            shape.ShouldNotBeEmpty();
            shape.First().Keys.Count.ShouldBe(2);
            shape.First().Keys.ShouldContain(nameof(Sample.Name));
            shape.First().Keys.ShouldContain(nameof(Sample.BirthDate));
        }

        [Fact]
        public void Return_Empty_Dictionary_With_Required_Non_Existing_Field()
        {
            var shape = (DataShapingExtensions.ShapeDataList(_samples, $"NonExistingProperty")
                .Single().Value as IEnumerable<IDictionary<string,object>>).ToList();

            shape.ShouldBeEmpty();
        }

        [Fact]
        public void Return_Empty_Dictionary_With_Required_Field_With_Invalid_Case()
        {
            var shape = (DataShapingExtensions.ShapeDataList(_samples,nameof(Sample.Name).ToUpperInvariant())
                .Single().Value as IEnumerable<IDictionary<string,object>>).ToList();

            shape.ShouldBeEmpty();
        }
    }
}
