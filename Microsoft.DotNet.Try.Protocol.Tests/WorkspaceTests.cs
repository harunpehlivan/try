﻿using System;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Try.Project;
using Microsoft.DotNet.Try.TestSupport;
using Xunit;

namespace Microsoft.DotNet.Try.Protocol.Tests
{
    public class WorkspaceTests
    {
        [Fact]
        public void I_can_extracts_viewPorts_when_files_declare_region()
        {
            var ws = new Workspace(files: new[]
            {
                new File("Program.cs", SourceCodeProvider.ConsoleProgramSingleRegion)
            });

            var viewPorts = ws.ExtractViewPorts();
            viewPorts.Should().NotBeEmpty();
            viewPorts.Select(p => p.BufferId.ToString()).Should().BeEquivalentTo("Program.cs@alpha");
        }

        [Fact]
        public void ViewPort_ids_must_be_uinique_within_a_file()
        {
            var ws = new Workspace(files: new[]
            {
                new File("Program.cs", SourceCodeProvider.ConsoleProgramCollidingRegions)
            });

            Action extraction = () => ws.ExtractViewPorts().ToList();
            extraction.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ViewPort_ids_must_be_unique_inside_the_workspace()
        {
            var ws = new Workspace(files: new[]
            {
                new File("ProgramA.cs", SourceCodeProvider.ConsoleProgramSingleRegion),
                new File("ProgramB.cs", SourceCodeProvider.ConsoleProgramSingleRegion)
            });

            Action extraction = () => ws.ExtractViewPorts();
            extraction.Should().NotThrow<InvalidOperationException>();
        }
        
        [Fact]
        public void ViewPort_extraction_fails_with_null_workspace()
        {
            Action extraction = () => ((Workspace)null).ExtractViewPorts().ToList();
            extraction.Should().Throw<ArgumentNullException>();
        }

    }
}