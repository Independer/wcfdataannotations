using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Moq;

namespace Independer.WCFDataAnnotations.UnitTests.Helpers {
  public static class MoqParameter {
    public static T ShouldBeEquivalentTo<T>(T expectedParameter) => It.Is<T>((Expression<Func<T, bool>>)(parameter => MoqParameter.MatchParameter<T>(parameter, expectedParameter)));

    public static T ShouldBeEquivalentTo<T>(
      T expectedParameter,
      Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config) => It.Is<T>((Expression<Func<T, bool>>)(parameter => MoqParameter.MatchParameter<T>(parameter, expectedParameter, config)));

    public static IEnumerable<T> CollectionShouldBeEquivalentTo<T>(
      params T[] expectedParameters) => It.Is<IEnumerable<T>>((Expression<Func<IEnumerable<T>, bool>>)(parameter => MoqParameter.MatchParameter<T>(parameter, expectedParameters)));

    private static bool MatchParameter<T>(T parameter, T expectedParameter) {
      AssertionExtensions.Should((object)parameter).BeEquivalentTo<T>(expectedParameter, "");
      return true;
    }

    private static bool MatchParameter<T>(
      T parameter,
      T expectedParameter,
      Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config) {
      AssertionExtensions.Should((object)parameter).BeEquivalentTo<T>(expectedParameter, config, "");
      return true;
    }

    private static bool MatchParameter<T>(IEnumerable<T> parameter, params T[] expectedParameters) {
      AssertionExtensions.Should<T>(parameter).BeEquivalentTo<T>((IEnumerable<T>)expectedParameters, "");
      return true;
    }
  }
}
