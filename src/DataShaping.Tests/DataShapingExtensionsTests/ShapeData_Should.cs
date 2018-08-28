using AutoFixture;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Xunit;
using DataShaping;

namespace DataShaping.Tests.DataShapingExtensionsTests.IEnumerableExtensionsTests
{
    public class ShapeData_Should
    {
        private Sample _sample;

        public ShapeData_Should()
        {
            var fixture = new Fixture();
            _sample = fixture.Create<Sample>();
        }

        [Fact]
        public void Return_Empty_Result()
        {
            var shape = DataShapingExtensions.ShapeData<Sample>(null,null);

            shape.ShouldBeEmpty();
        }

        [Fact]
        public void Return_Dictionary_For_All_Fields_If_Fields_Empty()
        {
            var shape = DataShapingExtensions.ShapeData(_sample,string.Empty);

            shape.ShouldNotBeEmpty();
            shape.Count.ShouldBe(3);
        }

        [Fact]
        public void Return_Dictionary_For_Required_Field()
        {
            var shape = DataShapingExtensions.ShapeData(_sample,nameof(_sample.BirthDate));

            shape.ShouldNotBeEmpty();
            shape.Count.ShouldBe(1);
            shape.Single().Key.ShouldBe(nameof(_sample.BirthDate));
            shape.Single().Value.ShouldBe(_sample.BirthDate);
        }

        [Fact]
        public void Return_Dictionary_For_Required_Fields()
        {
            var shape = DataShapingExtensions.ShapeData(_sample, $"{nameof(_sample.Id)},{nameof(_sample.BirthDate)}");

            shape.ShouldNotBeEmpty();
            shape.Count.ShouldBe(2);
            shape.Keys.ShouldContain(nameof(_sample.Id));
            shape.Keys.ShouldContain(nameof(_sample.BirthDate));
        }

        [Fact]
        public void Return_No_Data_For_Required_Non_Existing_Field()
        {
            var shape = DataShapingExtensions.ShapeData(_sample, $"NonExistingProperty");

            shape.ShouldBeEmpty();
        }

        [Fact]
        public void Return_No_Data_For_Required_Field_With_Invalid_Case()
        {
            var shape = DataShapingExtensions.ShapeData(_sample,nameof(_sample.Name).ToUpperInvariant());

            shape.ShouldBeEmpty();
        }
    }
}
