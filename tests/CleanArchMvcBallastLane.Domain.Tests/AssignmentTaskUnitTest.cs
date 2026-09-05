using Bogus;
using CleanArchMvcBallastLane.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Domain.Tests
{
    public class AssignmentTaskUnitTest
    {
        private readonly Faker _faker;

        public AssignmentTaskUnitTest()
        {
            _faker = new Faker();
        }

        [Fact]
        public void Should_CreateAssignmentTask_When_ParametersAreValid()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
            action.Should()
                 .NotThrow<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>();
        }

        [Fact]
        public void Should_ThrowDomainException_When_DescriptionIsTooShort()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(2), _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid description. Too short");
        }
        [Fact]
        public void Should_ThrowDomainException_When_DescriptionIsTooLong()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(300), _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid descriptionle. Too long");
        }

        [Fact]
        public void Should_ThrowDomainException_When_DescriptionIsMissing()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(10), "", _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid description. description is required");
        }


        [Fact]
        public void Should_ThrowDomainException_When_TitleIsTooShort()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(2), _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid Title. Too short");
        }
        [Fact]
        public void Should_ThrowDomainException_When_TitleIsTooLong()
        {
            Action action = () => new AssignmentTask(_faker.Random.AlphaNumeric(300), _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid Title. Too long");
        }

        [Fact]
        public void Should_ThrowDomainException_When_TitleIsMissing()
        {
            Action action = () => new AssignmentTask("", _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
            action.Should()
                .Throw<CleanArchMvcBallastLane.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid Title. Title is required");
        }
    }
}
