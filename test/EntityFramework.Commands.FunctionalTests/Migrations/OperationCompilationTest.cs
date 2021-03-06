﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Data.Entity.Commands.TestUtilities;
using Microsoft.Data.Entity.Commands.Utilities;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using Microsoft.Data.Entity.Utilities;
using Xunit;

namespace Microsoft.Data.Entity.Commands.Migrations
{
    public class OperationCompilationTest
    {
        private static string EOL => Environment.NewLine;

        [Fact]
        public void AddColumnOperation_required_args()
        {
            Test(
                new AddColumnOperation
                {
                    Name = "Id",
                    Table = "Post",
                    Type = "int",
                },
                "mb.AddColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: false);" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("int", o.Type);
                });
        }

        [Fact]
        public void AddColumnOperation_all_args()
        {
            Test(
                new AddColumnOperation
                {
                    Name = "Id",
                    Schema = "dbo",
                    Table = "Post",
                    Type = "int",
                    IsNullable = true,
                    DefaultValue = 1
                },
                "mb.AddColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: true," + EOL +
                "    defaultValue: 1);" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("int", o.Type);
                    Assert.True(o.IsNullable);
                    Assert.Equal(1, o.DefaultValue);
                });
        }

        [Fact]
        public void AddColumnOperation_DefaultExpression()
        {
            Test(
                new AddColumnOperation
                {
                    Name = "Id",
                    Table = "Post",
                    Type = "int",
                    DefaultExpression = "1"
                },
                "mb.AddColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: false," + EOL +
                "    defaultExpression: \"1\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("int", o.Type);
                    Assert.Equal("1", o.DefaultExpression);
                });
        }

        [Fact]
        public void AddForeignKeyOperation_required_args()
        {
            Test(
                new AddForeignKeyOperation
                {
                    Table = "Post",
                    Name = "FK_Post_Blog_BlogId",
                    Columns = new[] { "BlogId" },
                    ReferencedTable = "Blog",
                },
                "mb.AddForeignKey(" + EOL +
                "    name: \"FK_Post_Blog_BlogId\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"BlogId\"," + EOL +
                "    referencedTable: \"Blog\");" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("FK_Post_Blog_BlogId", o.Name);
                    Assert.Equal(new[] { "BlogId" }, o.Columns);
                    Assert.Equal("Blog", o.ReferencedTable);
                });
        }

        [Fact]
        public void AddForeignKeyOperation_all_args()
        {
            Test(
                new AddForeignKeyOperation
                {
                    Schema = "dbo",
                    Table = "Post",
                    Name = "FK_Post_Blog_BlogId",
                    Columns = new[] { "BlogId" },
                    ReferencedSchema = "my",
                    ReferencedTable = "Blog",
                    ReferencedColumns = new[] { "Id" },
                    OnUpdate = ReferentialAction.Restrict,
                    OnDelete = ReferentialAction.Cascade
                },
                "mb.AddForeignKey(" + EOL +
                "    name: \"FK_Post_Blog_BlogId\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"BlogId\"," + EOL +
                "    referencedSchema: \"my\"," + EOL +
                "    referencedTable: \"Blog\"," + EOL +
                "    referencedColumn: \"Id\"," + EOL +
                "    onUpdate: ReferentialAction.Restrict," + EOL +
                "    onDelete: ReferentialAction.Cascade);" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("FK_Post_Blog_BlogId", o.Name);
                    Assert.Equal(new[] { "BlogId" }, o.Columns);
                    Assert.Equal("Blog", o.ReferencedTable);
                    Assert.Equal("my", o.ReferencedSchema);
                    Assert.Equal(new[] { "Id" }, o.ReferencedColumns);
                    Assert.Equal(ReferentialAction.Cascade, o.OnDelete);
                });
        }

        [Fact]
        public void AddForeignKeyOperation_composite()
        {
            Test(
                new AddForeignKeyOperation
                {
                    Name = "FK_Post_Blog_BlogId1_BlogId2",
                    Table = "Post",
                    Columns = new[] { "BlogId1", "BlogId2" },
                    ReferencedTable = "Blog",
                    ReferencedColumns = new[] { "Id1", "Id2" }
                },
                "mb.AddForeignKey(" + EOL +
                "    name: \"FK_Post_Blog_BlogId1_BlogId2\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    columns: new[] { \"BlogId1\", \"BlogId2\" }," + EOL +
                "    referencedTable: \"Blog\"," + EOL +
                "    referencedColumns: new[] { \"Id1\", \"Id2\" });" + EOL,
                o =>
                {
                    Assert.Equal("FK_Post_Blog_BlogId1_BlogId2", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "BlogId1", "BlogId2" }, o.Columns);
                    Assert.Equal("Blog", o.ReferencedTable);
                    Assert.Equal(new[] { "Id1", "Id2" }, o.ReferencedColumns);
                });
        }

        [Fact]
        public void AddPrimaryKey_required_args()
        {
            Test(
                new AddPrimaryKeyOperation
                {
                    Name = "PK_Post",
                    Table = "Post",
                    Columns = new[] { "Id" }
                },
                "mb.AddPrimaryKey(" + EOL +
                "    name: \"PK_Post\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"Id\");" + EOL,
                o =>
                {
                    Assert.Equal("PK_Post", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Id" }, o.Columns);
                });
        }

        [Fact]
        public void AddPrimaryKey_all_args()
        {
            Test(
                new AddPrimaryKeyOperation
                {
                    Name = "PK_Post",
                    Schema = "dbo",
                    Table = "Post",
                    Columns = new[] { "Id" }
                },
                "mb.AddPrimaryKey(" + EOL +
                "    name: \"PK_Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"Id\");" + EOL,
                o =>
                {
                    Assert.Equal("PK_Post", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Id" }, o.Columns);
                });
        }

        [Fact]
        public void AddPrimaryKey_composite()
        {
            Test(
                new AddPrimaryKeyOperation
                {
                    Name = "PK_Post",
                    Table = "Post",
                    Columns = new[] { "Id1", "Id2" }
                },
                "mb.AddPrimaryKey(" + EOL +
                "    name: \"PK_Post\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    columns: new[] { \"Id1\", \"Id2\" });" + EOL,
                o =>
                {
                    Assert.Equal("PK_Post", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Id1", "Id2" }, o.Columns);
                });
        }

        [Fact]
        public void AddUniqueConstraint_required_args()
        {
            Test(
                new AddUniqueConstraintOperation
                {
                    Name = "AK_Post_AltId",
                    Table = "Post",
                    Columns = new[] { "AltId" }
                },
                "mb.AddUniqueConstraint(" + EOL +
                "    name: \"AK_Post_AltId\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"AltId\");" + EOL,
                o =>
                {
                    Assert.Equal("AK_Post_AltId", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "AltId" }, o.Columns);
                });
        }

        [Fact]
        public void AddUniqueConstraint_all_args()
        {
            Test(
                new AddUniqueConstraintOperation
                {
                    Name = "AK_Post_AltId",
                    Schema = "dbo",
                    Table = "Post",
                    Columns = new[] { "AltId" }
                },
                "mb.AddUniqueConstraint(" + EOL +
                "    name: \"AK_Post_AltId\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"AltId\");" + EOL,
                o =>
                {
                    Assert.Equal("AK_Post_AltId", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "AltId" }, o.Columns);
                });
        }

        [Fact]
        public void AddUniqueConstraint_composite()
        {
            Test(
                new AddUniqueConstraintOperation
                {
                    Name = "AK_Post_AltId1_AltId2",
                    Table = "Post",
                    Columns = new[] { "AltId1", "AltId2" }
                },
                "mb.AddUniqueConstraint(" + EOL +
                "    name: \"AK_Post_AltId1_AltId2\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    columns: new[] { \"AltId1\", \"AltId2\" });" + EOL,
                o =>
                {
                    Assert.Equal("AK_Post_AltId1_AltId2", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "AltId1", "AltId2" }, o.Columns);
                });
        }

        [Fact]
        public void AlterColumnOperation_required_args()
        {
            Test(
                new AlterColumnOperation
                {
                    Name = "Id",
                    Table = "Post",
                    Type = "int"
                },
                "mb.AlterColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: false);" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("int", o.Type);
                });
        }

        [Fact]
        public void AlterColumnOperation_all_args()
        {
            Test(
                new AlterColumnOperation
                {
                    Name = "Id",
                    Schema = "dbo",
                    Table = "Post",
                    Type = "int",
                    IsNullable = true,
                    DefaultValue = 1
                },
                "mb.AlterColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: true," + EOL +
                "    defaultValue: 1);" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("int", o.Type);
                    Assert.True(o.IsNullable);
                    Assert.Equal(1, o.DefaultValue);
                });
        }

        [Fact]
        public void AlterColumnOperation_DefaultExpression()
        {
            Test(
                new AlterColumnOperation
                {
                    Name = "Id",
                    Table = "Post",
                    Type = "int",
                    DefaultExpression = "1"
                },
                "mb.AlterColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    type: \"int\"," + EOL +
                "    nullable: false," + EOL +
                "    defaultExpression: \"1\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("1", o.DefaultExpression);
                });
        }

        [Fact]
        public void AlterSequenceOperation_required_args()
        {
            Test(
                new AlterSequenceOperation { Name = "DefaultSequence" },
                "mb.AlterSequence(" + EOL +
                "    name: \"DefaultSequence\");" + EOL,
                o => Assert.Equal("DefaultSequence", o.Name));
        }

        [Fact]
        public void AlterSequenceOperation_all_args()
        {
            Test(
                new AlterSequenceOperation
                {
                    Name = "DefaultSequence",
                    Schema = "dbo",
                    IncrementBy = 1,
                    MinValue = 2,
                    MaxValue = 4,
                    Cycle = true
                },
                "mb.AlterSequence(" + EOL +
                "    name: \"DefaultSequence\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    incrementBy: 1," + EOL +
                "    minValue: 2L," + EOL +
                "    maxValue: 4L," + EOL +
                "    cycle: true);" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal(1, o.IncrementBy);
                    Assert.Equal(2, o.MinValue);
                    Assert.Equal(4, o.MaxValue);
                    Assert.True(o.Cycle);
                });
        }

        [Fact]
        public void CreateIndexOperation_required_args()
        {
            Test(
                new CreateIndexOperation
                {
                    Name = "IX_Post_Title",
                    Table = "Post",
                    Columns = new[] { "Title" }
                },
                "mb.CreateIndex(" + EOL +
                "    name: \"IX_Post_Title\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"Title\");" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Title" }, o.Columns);
                });
        }

        [Fact]
        public void CreateIndexOperation_all_args()
        {
            Test(
                new CreateIndexOperation
                {
                    Name = "IX_Post_Title",
                    Schema = "dbo",
                    Table = "Post",
                    Columns = new[] { "Title" },
                    IsUnique = true
                },
                "mb.CreateIndex(" + EOL +
                "    name: \"IX_Post_Title\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    column: \"Title\"," + EOL +
                "    unique: true);" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Title" }, o.Columns);
                    Assert.True(o.IsUnique);
                });
        }

        [Fact]
        public void CreateIndexOperation_composite()
        {
            Test(
                new CreateIndexOperation
                {
                    Name = "IX_Post_Title_Subtitle",
                    Table = "Post",
                    Columns = new[] { "Title", "Subtitle" }
                },
                "mb.CreateIndex(" + EOL +
                "    name: \"IX_Post_Title_Subtitle\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    columns: new[] { \"Title\", \"Subtitle\" });" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title_Subtitle", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal(new[] { "Title", "Subtitle" }, o.Columns);
                });
        }

        [Fact]
        public void CreateSchemaOperation_required_args()
        {
            Test(
                new CreateSchemaOperation { Name = "my" },
                "mb.CreateSchema(\"my\");" + EOL,
                o => Assert.Equal("my", o.Name));
        }

        [Fact]
        public void CreateSequenceOperation_required_args()
        {
            Test(
                new CreateSequenceOperation { Name = "DefaultSequence" },
                "mb.CreateSequence(" + EOL +
                "    name: \"DefaultSequence\");" + EOL,
                o => Assert.Equal("DefaultSequence", o.Name));
        }

        [Fact]
        public void CreateSequenceOperation_all_args()
        {
            Test(
                new CreateSequenceOperation
                {
                    Name = "DefaultSequence",
                    Schema = "dbo",
                    Type = "bigint",
                    StartWith = 3,
                    IncrementBy = 1,
                    MinValue = 2,
                    MaxValue = 4,
                    Cycle = true
                },
                "mb.CreateSequence(" + EOL +
                "    name: \"DefaultSequence\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    type: \"bigint\"," + EOL +
                "    startWith: 3L," + EOL +
                "    incrementBy: 1," + EOL +
                "    minValue: 2L," + EOL +
                "    maxValue: 4L," + EOL +
                "    cycle: true);" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("bigint", o.Type);
                    Assert.Equal(3, o.StartWith);
                    Assert.Equal(1, o.IncrementBy);
                    Assert.Equal(2, o.MinValue);
                    Assert.Equal(4, o.MaxValue);
                    Assert.True(o.Cycle);
                });
        }

        [Fact]
        public void CreateTableOperation_Columns_required_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Id",
                            Table = "Post",
                            Type = "int"
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        Id = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Name);
                    Assert.Equal(1, o.Columns.Count);

                    Assert.Equal("Id", o.Columns[0].Name);
                    Assert.Equal("Post", o.Columns[0].Table);
                    Assert.Equal("int", o.Columns[0].Type);
                });
        }

        [Fact]
        public void CreateTableOperation_Columns_all_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Schema = "dbo",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Post Id",
                            Schema = "dbo",
                            Table = "Post",
                            Type = "int",
                            IsNullable = true,
                            DefaultValue = 1
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        PostId = table.Column(name: \"Post Id\", type: \"int\", nullable: true, defaultValue: 1)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal(1, o.Columns.Count);

                    Assert.Equal("Post Id", o.Columns[0].Name);
                    Assert.Equal("dbo", o.Columns[0].Schema);
                    Assert.Equal("Post", o.Columns[0].Table);
                    Assert.Equal("int", o.Columns[0].Type);
                    Assert.True(o.Columns[0].IsNullable);
                    Assert.Equal(1, o.Columns[0].DefaultValue);
                });
        }

        [Fact]
        public void CreateTableOperation_Columns_DefaultExpression()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation
                        {
                            Name = "Id",
                            Table = "Post",
                            Type = "int",
                            DefaultExpression = "1"
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        Id = table.Column(type: \"int\", nullable: false, defaultExpression: \"1\")" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.Columns.Count);

                    Assert.Equal("Id", o.Columns[0].Name);
                    Assert.Equal("Post", o.Columns[0].Table);
                    Assert.Equal("int", o.Columns[0].Type);
                    Assert.Equal("1", o.Columns[0].DefaultExpression);
                });
        }

        [Fact]
        public void CreateTableOperation_ForeignKeys_required_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "BlogId", Type = "int" }
                    },
                    ForeignKeys =
                    {
                        new AddForeignKeyOperation
                        {
                            Name = "FK_Post_Blog_BlogId",
                            Table = "Post",
                            Columns = new[] { "BlogId" },
                            ReferencedTable = "Blog"
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        BlogId = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.ForeignKey(" + EOL +
                "            name: \"FK_Post_Blog_BlogId\"," + EOL +
                "            columns: x => x.BlogId," + EOL +
                "            referencedTable: \"Blog\");" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.ForeignKeys.Count);

                    var fk = o.ForeignKeys.First();
                    Assert.Equal("FK_Post_Blog_BlogId", fk.Name);
                    Assert.Equal("Post", fk.Table);
                    Assert.Equal(new[] { "BlogId" }, fk.Columns.ToArray());
                    Assert.Equal("Blog", fk.ReferencedTable);
                });
        }

        [Fact]
        public void CreateTableOperation_ForeignKeys_all_args()
        {
            Test(
                new CreateTableOperation
                {
                    Schema = "dbo",
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "BlogId", Type = "int" }
                    },
                    ForeignKeys =
                    {
                        new AddForeignKeyOperation
                        {
                            Schema = "dbo",
                            Table = "Post",
                            Name = "FK_Post_Blog_BlogId",
                            Columns = new[] { "BlogId" },
                            ReferencedTable = "Blog",
                            ReferencedSchema = "my",
                            ReferencedColumns = new[] { "Id" },
                            OnUpdate = ReferentialAction.SetNull,
                            OnDelete = ReferentialAction.SetDefault
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        BlogId = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.ForeignKey(" + EOL +
                "            name: \"FK_Post_Blog_BlogId\"," + EOL +
                "            columns: x => x.BlogId," + EOL +
                "            referencedSchema: \"my\"," + EOL +
                "            referencedTable: \"Blog\"," + EOL +
                "            referencedColumn: \"Id\"," + EOL +
                "            onUpdate: ReferentialAction.SetNull," + EOL +
                "            onDelete: ReferentialAction.SetDefault);" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.ForeignKeys.Count);

                    var fk = o.ForeignKeys.First();
                    Assert.Equal("Post", fk.Table);
                    Assert.Equal("dbo", fk.Schema);
                    Assert.Equal("FK_Post_Blog_BlogId", fk.Name);
                    Assert.Equal(new[] { "BlogId" }, fk.Columns.ToArray());
                    Assert.Equal("Blog", fk.ReferencedTable);
                    Assert.Equal("my", fk.ReferencedSchema);
                    Assert.Equal(new[] { "Id" }, fk.ReferencedColumns);
                    Assert.Equal(ReferentialAction.SetNull, fk.OnUpdate);
                    Assert.Equal(ReferentialAction.SetDefault, fk.OnDelete);
                });
        }

        [Fact]
        public void CreateTableOperation_ForeignKeys_composite()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "BlogId1", Type = "int" },
                        new AddColumnOperation { Name = "BlogId2", Type = "int" }
                    },
                    ForeignKeys =
                    {
                        new AddForeignKeyOperation
                        {
                            Name = "FK_Post_Blog_BlogId1_BlogId2",
                            Table = "Post",
                            Columns = new[] { "BlogId1", "BlogId2" },
                            ReferencedTable = "Blog",
                            ReferencedColumns = new[] { "Id1", "Id2" }
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        BlogId1 = table.Column(type: \"int\", nullable: false)," + EOL +
                "        BlogId2 = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.ForeignKey(" + EOL +
                "            name: \"FK_Post_Blog_BlogId1_BlogId2\"," + EOL +
                "            columns: x => new { x.BlogId1, x.BlogId2 }," + EOL +
                "            referencedTable: \"Blog\"," + EOL +
                "            referencedColumns: new[] { \"Id1\", \"Id2\" });" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.ForeignKeys.Count);

                    var fk = o.ForeignKeys.First();
                    Assert.Equal("Post", fk.Table);
                    Assert.Equal(new[] { "BlogId1", "BlogId2" }, fk.Columns.ToArray());
                    Assert.Equal("Blog", fk.ReferencedTable);
                    Assert.Equal(new[] { "Id1", "Id2" }, fk.ReferencedColumns);
                });
        }

        [Fact]
        public void CreateTableOperation_PrimaryKey_required_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "Id", Type = "int" }
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Name = "PK_Post",
                        Table = "Post",
                        Columns = new[] { "Id" }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        Id = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.PrimaryKey(\"PK_Post\", x => x.Id);" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.NotNull(o.PrimaryKey);

                    Assert.Equal("PK_Post", o.PrimaryKey.Name);
                    Assert.Equal("Post", o.PrimaryKey.Table);
                    Assert.Equal(new[] { "Id" }, o.PrimaryKey.Columns);
                });
        }

        [Fact]
        public void CreateTableOperation_PrimaryKey_all_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Schema = "dbo",
                    Columns =
                    {
                        new AddColumnOperation { Name = "Id", Type = "int" }
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Name = "PK_Post",
                        Schema = "dbo",
                        Table = "Post",
                        Columns = new[] { "Id" }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        Id = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.PrimaryKey(\"PK_Post\", x => x.Id);" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.NotNull(o.PrimaryKey);

                    Assert.Equal("PK_Post", o.PrimaryKey.Name);
                    Assert.Equal("dbo", o.PrimaryKey.Schema);
                    Assert.Equal("Post", o.PrimaryKey.Table);
                    Assert.Equal(new[] { "Id" }, o.PrimaryKey.Columns);
                });
        }

        [Fact]
        public void CreateTableOperation_PrimaryKey_composite()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "Id1", Type = "int" },
                        new AddColumnOperation { Name = "Id2", Type = "int" }
                    },
                    PrimaryKey = new AddPrimaryKeyOperation
                    {
                        Name = "PK_Post",
                        Table = "Post",
                        Columns = new[] { "Id1", "Id2" }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        Id1 = table.Column(type: \"int\", nullable: false)," + EOL +
                "        Id2 = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.PrimaryKey(\"PK_Post\", x => new { x.Id1, x.Id2 });" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.NotNull(o.PrimaryKey);

                    Assert.Equal("PK_Post", o.PrimaryKey.Name);
                    Assert.Equal("Post", o.PrimaryKey.Table);
                    Assert.Equal(new[] { "Id1", "Id2" }, o.PrimaryKey.Columns);
                });
        }

        [Fact]
        public void CreateTableOperation_UniqueConstraints_required_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "AltId", Type = "int" }
                    },
                    UniqueConstraints =
                    {
                        new AddUniqueConstraintOperation
                        {
                            Name = "AK_Post_AltId",
                            Table = "Post",
                            Columns = new[] { "AltId" }
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        AltId = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.Unique(\"AK_Post_AltId\", x => x.AltId);" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.UniqueConstraints.Count);

                    Assert.Equal("AK_Post_AltId", o.UniqueConstraints[0].Name);
                    Assert.Equal("Post", o.UniqueConstraints[0].Table);
                    Assert.Equal(new[] { "AltId" }, o.UniqueConstraints[0].Columns);
                });
        }

        [Fact]
        public void CreateTableOperation_UniqueConstraints_all_args()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Schema = "dbo",
                    Columns =
                    {
                        new AddColumnOperation { Name = "AltId", Type = "int" }
                    },
                    UniqueConstraints =
                    {
                        new AddUniqueConstraintOperation
                        {
                            Name = "AK_Post_AltId",
                            Schema = "dbo",
                            Table = "Post",
                            Columns = new[] { "AltId" }
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        AltId = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.Unique(\"AK_Post_AltId\", x => x.AltId);" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.UniqueConstraints.Count);

                    Assert.Equal("AK_Post_AltId", o.UniqueConstraints[0].Name);
                    Assert.Equal("dbo", o.UniqueConstraints[0].Schema);
                    Assert.Equal("Post", o.UniqueConstraints[0].Table);
                    Assert.Equal(new[] { "AltId" }, o.UniqueConstraints[0].Columns);
                });
        }

        [Fact]
        public void CreateTableOperation_UniqueConstraints_composite()
        {
            Test(
                new CreateTableOperation
                {
                    Name = "Post",
                    Columns =
                    {
                        new AddColumnOperation { Name = "AltId1", Type = "int" },
                        new AddColumnOperation { Name = "AltId2", Type = "int" }
                    },
                    UniqueConstraints =
                    {
                        new AddUniqueConstraintOperation
                        {
                            Name = "AK_Post_AltId1_AltId2",
                            Table = "Post",
                            Columns = new[] { "AltId1", "AltId2" }
                        }
                    }
                },
                "mb.CreateTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    columns: table => new" + EOL +
                "    {" + EOL +
                "        AltId1 = table.Column(type: \"int\", nullable: false)," + EOL +
                "        AltId2 = table.Column(type: \"int\", nullable: false)" + EOL +
                "    }," + EOL +
                "    constraints: table =>" + EOL +
                "    {" + EOL +
                "        table.Unique(\"AK_Post_AltId1_AltId2\", x => new { x.AltId1, x.AltId2 });" + EOL +
                "    });" + EOL,
                o =>
                {
                    Assert.Equal(1, o.UniqueConstraints.Count);

                    Assert.Equal("AK_Post_AltId1_AltId2", o.UniqueConstraints[0].Name);
                    Assert.Equal("Post", o.UniqueConstraints[0].Table);
                    Assert.Equal(new[] { "AltId1", "AltId2" }, o.UniqueConstraints[0].Columns);
                });
        }

        [Fact]
        public void DropColumnOperation_required_args()
        {
            Test(
                new DropColumnOperation
                {
                    Name = "Id",
                    Table = "Post"
                },
                "mb.DropColumn(name: \"Id\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropColumnOperation_all_args()
        {
            Test(
                new DropColumnOperation
                {
                    Name = "Id",
                    Schema = "dbo",
                    Table = "Post"
                },
                "mb.DropColumn(name: \"Id\", schema: \"dbo\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropForeignKeyOperation_required_args()
        {
            Test(
                new DropForeignKeyOperation
                {
                    Name = "FK_Post_BlogId",
                    Table = "Post"
                },
                "mb.DropForeignKey(name: \"FK_Post_BlogId\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("FK_Post_BlogId", o.Name);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropForeignKeyOperation_all_args()
        {
            Test(
                new DropForeignKeyOperation
                {
                    Name = "FK_Post_BlogId",
                    Schema = "dbo",
                    Table = "Post"
                },
                "mb.DropForeignKey(name: \"FK_Post_BlogId\", schema: \"dbo\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("FK_Post_BlogId", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropIndexOperation_required_args()
        {
            Test(
                new DropIndexOperation
                {
                    Name = "IX_Post_Title",
                    Table = "Post"
                },
                "mb.DropIndex(name: \"IX_Post_Title\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title", o.Name);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropIndexOperation_all_args()
        {
            Test(
                new DropIndexOperation
                {
                    Name = "IX_Post_Title",
                    Schema = "dbo",
                    Table = "Post"
                },
                "mb.DropIndex(name: \"IX_Post_Title\", schema: \"dbo\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropPrimaryKeyOperation_required_args()
        {
            Test(
                new DropPrimaryKeyOperation
                {
                    Name = "PK_Post",
                    Table = "Post"
                },
                "mb.DropPrimaryKey(name: \"PK_Post\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("PK_Post", o.Name);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropPrimaryKeyOperation_all_args()
        {
            Test(
                new DropPrimaryKeyOperation
                {
                    Name = "PK_Post",
                    Schema = "dbo",
                    Table = "Post"
                },
                "mb.DropPrimaryKey(name: \"PK_Post\", schema: \"dbo\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("PK_Post", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropSchemaOperation_required_args()
        {
            Test(
                new DropSchemaOperation { Name = "my" },
                "mb.DropSchema(\"my\");" + EOL,
                o => Assert.Equal("my", o.Name));
        }

        [Fact]
        public void DropSequenceOperation_required_args()
        {
            Test(
                new DropSequenceOperation { Name = "DefaultSequence" },
                "mb.DropSequence(\"DefaultSequence\");" + EOL,
                o => Assert.Equal("DefaultSequence", o.Name));
        }

        [Fact]
        public void DropSequenceOperation_all_args()
        {
            Test(
                new DropSequenceOperation
                {
                    Name = "DefaultSequence",
                    Schema = "dbo"
                },
                "mb.DropSequence(name: \"DefaultSequence\", schema: \"dbo\");" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal("dbo", o.Schema);
                });
        }

        [Fact]
        public void DropTableOperation_required_args()
        {
            Test(
                new DropTableOperation { Name = "Post" },
                "mb.DropTable(\"Post\");" + EOL,
                o => Assert.Equal("Post", o.Name));
        }

        [Fact]
        public void DropTableOperation_all_args()
        {
            Test(
                new DropTableOperation
                {
                    Name = "Post",
                    Schema = "dbo"
                },
                "mb.DropTable(name: \"Post\", schema: \"dbo\");" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Name);
                    Assert.Equal("dbo", o.Schema);
                });
        }

        [Fact]
        public void DropUniqueConstraintOperation_required_args()
        {
            Test(
                new DropUniqueConstraintOperation
                {
                    Name = "AK_Post_AltId",
                    Table = "Post"
                },
                "mb.DropUniqueConstraint(name: \"AK_Post_AltId\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("AK_Post_AltId", o.Name);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void DropUniqueConstraintOperation_all_args()
        {
            Test(
                new DropUniqueConstraintOperation
                {
                    Name = "AK_Post_AltId",
                    Schema = "dbo",
                    Table = "Post"
                },
                "mb.DropUniqueConstraint(name: \"AK_Post_AltId\", schema: \"dbo\", table: \"Post\");" + EOL,
                o =>
                {
                    Assert.Equal("AK_Post_AltId", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                });
        }

        [Fact]
        public void RenameColumnOperation_required_args()
        {
            Test(
                new RenameColumnOperation
                {
                    Name = "Id",
                    Table = "Post",
                    NewName = "PostId"
                },
                "mb.RenameColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    newName: \"PostId\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("PostId", o.NewName);
                });
        }

        [Fact]
        public void RenameColumnOperation_all_args()
        {
            Test(
                new RenameColumnOperation
                {
                    Name = "Id",
                    Schema = "dbo",
                    Table = "Post",
                    NewName = "PostId"
                },
                "mb.RenameColumn(" + EOL +
                "    name: \"Id\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    newName: \"PostId\");" + EOL,
                o =>
                {
                    Assert.Equal("Id", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("PostId", o.NewName);
                });
        }

        [Fact]
        public void RenameIndexOperation_required_args()
        {
            Test(
                new RenameIndexOperation
                {
                    Name = "IX_Post_Title",
                    Table = "Post",
                    NewName = "IX_Post_PostTitle"
                },
                "mb.RenameIndex(" + EOL +
                "    name: \"IX_Post_Title\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    newName: \"IX_Post_PostTitle\");" + EOL,
                o =>
                {
                    Assert.Equal("IX_Post_Title", o.Name);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("IX_Post_PostTitle", o.NewName);
                });
        }

        [Fact]
        public void RenameIndexOperation_all_args()
        {
            Test(
                new RenameIndexOperation
                {
                    Name = "IX_dbo.Post_Title",
                    Schema = "dbo",
                    Table = "Post",
                    NewName = "IX_dbo.Post_PostTitle"
                },
                "mb.RenameIndex(" + EOL +
                "    name: \"IX_dbo.Post_Title\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    table: \"Post\"," + EOL +
                "    newName: \"IX_dbo.Post_PostTitle\");" + EOL,
                o =>
                {
                    Assert.Equal("IX_dbo.Post_Title", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Post", o.Table);
                    Assert.Equal("IX_dbo.Post_PostTitle", o.NewName);
                });
        }

        [Fact]
        public void RenameSequenceOperation_required_args()
        {
            Test(
                new RenameSequenceOperation { Name = "DefaultSequence" },
                "mb.RenameSequence(" + EOL +
                "    name: \"DefaultSequence\");" + EOL,
                o => Assert.Equal("DefaultSequence", o.Name));
        }

        [Fact]
        public void RenameSequenceOperation_all_args()
        {
            Test(
                new RenameSequenceOperation
                {
                    Name = "DefaultSequence",
                    Schema = "dbo",
                    NewName = "MySequence",
                    NewSchema = "my"
                },
                "mb.RenameSequence(" + EOL +
                "    name: \"DefaultSequence\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    newName: \"MySequence\"," + EOL +
                "    newSchema: \"my\");" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("MySequence", o.NewName);
                    Assert.Equal("my", o.NewSchema);
                });
        }

        [Fact]
        public void RenameTableOperation_required_args()
        {
            Test(
                new RenameTableOperation { Name = "Post" },
                "mb.RenameTable(" + EOL +
                "    name: \"Post\");" + EOL,
                o => Assert.Equal("Post", o.Name));
        }

        [Fact]
        public void RenameTableOperation_all_args()
        {
            Test(
                new RenameTableOperation
                {
                    Name = "Post",
                    Schema = "dbo",
                    NewName = "Posts",
                    NewSchema = "my"
                },
                "mb.RenameTable(" + EOL +
                "    name: \"Post\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    newName: \"Posts\"," + EOL +
                "    newSchema: \"my\");" + EOL,
                o =>
                {
                    Assert.Equal("Post", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal("Posts", o.NewName);
                    Assert.Equal("my", o.NewSchema);
                });
        }

        [Fact]
        public void RestartSequenceOperation_required_args()
        {
            Test(
                new RestartSequenceOperation
                {
                    Name = "DefaultSequence",
                    RestartWith = 1
                },
                "mb.RestartSequence(" + EOL +
                "    name: \"DefaultSequence\"," + EOL +
                "    with: 1L);" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal(1, o.RestartWith);
                });
        }

        [Fact]
        public void RestartSequenceOperation_all_args()
        {
            Test(
                new RestartSequenceOperation
                {
                    Name = "DefaultSequence",
                    Schema = "dbo",
                    RestartWith = 1
                },
                "mb.RestartSequence(" + EOL +
                "    name: \"DefaultSequence\"," + EOL +
                "    schema: \"dbo\"," + EOL +
                "    with: 1L);" + EOL,
                o =>
                {
                    Assert.Equal("DefaultSequence", o.Name);
                    Assert.Equal("dbo", o.Schema);
                    Assert.Equal(1, o.RestartWith);
                });
        }

        [Fact]
        public void SqlOperation_required_args()
        {
            Test(
                new SqlOperation { Sql = "-- I <3 DDL" },
                "mb.Sql(" + EOL +
                "    \"-- I <3 DDL\");" + EOL,
                o => Assert.Equal("-- I <3 DDL", o.Sql));
        }

        [Fact]
        public void SqlOperation_all_args()
        {
            Test(
                new SqlOperation
                {
                    Sql = "-- I <3 DDL",
                    SuppressTransaction = true
                },
                "mb.Sql(" + EOL +
                "    sql: \"-- I <3 DDL\"," + EOL +
                "    suppressTransaction: true);" + EOL,
                o =>
                {
                    Assert.Equal("-- I <3 DDL", o.Sql);
                    Assert.True(o.SuppressTransaction);
                });
        }

        private void Test<T>(T operation, string expectedCode, Action<T> assert)
            where T : MigrationOperation
        {
            var generator = new CSharpMigrationOperationGenerator(new CSharpHelper());

            var builder = new IndentedStringBuilder();
            generator.Generate("mb", new[] { operation }, builder);
            var code = builder.ToString();

            Assert.Equal(expectedCode, code);

            var build = new BuildSource
            {
                References =
                {
                    BuildReference.ByName("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
                    BuildReference.ByName("System.Linq.Expressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                    BuildReference.ByName("System.Runtime, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                    BuildReference.ByName("EntityFramework.Relational"),
                    BuildReference.ByName("EntityFramework.Relational.Design")
                },
                Source = @"
                    using System.Collections.Generic;
                    using Microsoft.Data.Entity.Relational.Migrations.Builders;
                    using Microsoft.Data.Entity.Relational.Migrations.Operations;

                    public static class OperationsFactory
                    {
                        public static void Create(MigrationBuilder mb)
                        {
                            " + code + @"
                        }
                    }
                "
            };

            var assembly = build.BuildInMemory();
            var factoryType = assembly.GetType("OperationsFactory");
            var createMethod = factoryType.GetMethod("Create");
            var mb = new MigrationBuilder();
            createMethod.Invoke(null, new[] { mb });
            var result = mb.Operations.Cast<T>().Single();

            assert(result);
        }
    }
}
