using FluentAssertions;
using Moq;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Model.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.FilterModels;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.FilterModels;

public class EntityFilterInfoTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        // Arrange
        var structure = new CommonFormStructure();
        var form = Mock.Of<Form>();
        var filterValues = new ElasticObject();
        var sortFields = "Field1,Field2";

        // Act
        var filterInfo = new EntityFilterInfo(structure, form, filterValues, sortFields);

        // Assert
        filterInfo.Structure.Should().Be(structure);
        filterInfo.Form.Should().Be(form);
        filterInfo.FilterValues.Should().Be(filterValues);
        filterInfo.SortFields.Should().Be(sortFields);
    }

    [Xunit.Fact]
    public void Properties_ShouldBeReadOnly()
    {
        // Arrange
        var structure = new CommonFormStructure();
        var form = Mock.Of<Form>();
        var filterValues = new ElasticObject();
        var sortFields = "Field1";

        // Act
        var filterInfo = new EntityFilterInfo(structure, form, filterValues, sortFields);

        // Assert
        filterInfo.Structure.Should().Be(structure);
        filterInfo.Form.Should().Be(form);
        filterInfo.FilterValues.Should().Be(filterValues);
        filterInfo.SortFields.Should().Be(sortFields);
    }
}

