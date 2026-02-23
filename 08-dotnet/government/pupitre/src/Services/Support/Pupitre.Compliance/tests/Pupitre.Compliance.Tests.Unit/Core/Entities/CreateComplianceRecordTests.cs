using System.Collections.Generic;
using System.Linq;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Domain.Events;
using Pupitre.Compliance.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Compliance.Tests.Unit.Core.Entities;

public class CreateComplianceRecordTests
{
    private ComplianceRecord Act(EntityId id, string name, IEnumerable<string> tags) => ComplianceRecord.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_compliancerecord_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var compliancerecord = Act(id, name, tags);
        
        // Assert
        compliancerecord.ShouldNotBeNull();
        compliancerecord.Id.ToString().ShouldBe(id.ToString());
        compliancerecord.Tags.ShouldBe(tags);
        compliancerecord.Events.Count().ShouldBe(1);

        var @event = compliancerecord.Events.Single();
        @event.ShouldBeOfType<ComplianceRecordCreated>();
    }

    [Fact]
    public void given_empty_tags_compliancerecord_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingComplianceRecordTagsException>();
    }
}