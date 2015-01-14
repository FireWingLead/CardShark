
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/08/2014 12:03:17
-- Generated from EDMX file: C:\Users\PerrinL\documents\visual studio 2012\Projects\CardShark\CardShark.PCShark\DAL\SQLiteLocal\LocalCardData.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [LocalCardData];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CardCardVariation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CardVariations] DROP CONSTRAINT [FK_CardCardVariation];
GO
IF OBJECT_ID(N'[dbo].[FK_SetCardVariation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CardVariations] DROP CONSTRAINT [FK_SetCardVariation];
GO
IF OBJECT_ID(N'[dbo].[FK_ManaSymbolSetSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sets] DROP CONSTRAINT [FK_ManaSymbolSetSet];
GO
IF OBJECT_ID(N'[dbo].[FK_ManaSymbolSetManaSymbol]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ManaSymbols] DROP CONSTRAINT [FK_ManaSymbolSetManaSymbol];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Cards]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cards];
GO
IF OBJECT_ID(N'[dbo].[CardVariations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CardVariations];
GO
IF OBJECT_ID(N'[dbo].[Sets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sets];
GO
IF OBJECT_ID(N'[dbo].[ManaSymbols]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ManaSymbols];
GO
IF OBJECT_ID(N'[dbo].[ManaSymbolSets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ManaSymbolSets];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Cards'
CREATE TABLE [dbo].[Cards] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(200)  NULL,
    [ManaCost] nvarchar(200)  NULL,
    [CardType] nvarchar(200)  NULL,
    [RuleText] nvarchar(max)  NULL,
    [PowerVal] int  NULL,
    [PowerVar] nvarchar(4)  NULL,
    [ToughnessVal] int  NULL,
    [ToughnessVar] nvarchar(4)  NULL,
    [LoyaltyVal] int  NULL,
    [LoyaltyVar] nvarchar(4)  NULL,
    [Author] nvarchar(200)  NULL
);
GO

-- Creating table 'CardVariations'
CREATE TABLE [dbo].[CardVariations] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CardId] bigint  NOT NULL,
    [SetCode] nvarchar(10)  NULL,
    [FlavorText] nvarchar(max)  NULL,
    [Author] nvarchar(200)  NULL,
    [Artist] nvarchar(200)  NULL,
    [FullCardImage] varbinary(max)  NULL,
    [NumberOwned] int  NOT NULL
);
GO

-- Creating table 'Sets'
CREATE TABLE [dbo].[Sets] (
    [Code] nvarchar(10)  NOT NULL,
    [Name] nvarchar(200)  NULL,
    [Description] nvarchar(max)  NULL,
    [CommonSymbol] varbinary(max)  NULL,
    [UncommonSymbol] varbinary(max)  NULL,
    [RareSymbol] varbinary(max)  NULL,
    [MythicSymbol] varbinary(max)  NULL,
    [SpecialSymbol] varbinary(max)  NULL,
    [IsOfficial] bit  NOT NULL,
    [ManaSymbolSetId] bigint  NULL
);
GO

-- Creating table 'ManaSymbols'
CREATE TABLE [dbo].[ManaSymbols] (
    [Code] nvarchar(100)  NOT NULL,
    [ManaSymbolSetId] bigint  NULL,
    [Image] varbinary(max)  NULL
);
GO

-- Creating table 'ManaSymbolSets'
CREATE TABLE [dbo].[ManaSymbolSets] (
    [Id] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Cards'
ALTER TABLE [dbo].[Cards]
ADD CONSTRAINT [PK_Cards]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CardVariations'
ALTER TABLE [dbo].[CardVariations]
ADD CONSTRAINT [PK_CardVariations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Code] in table 'Sets'
ALTER TABLE [dbo].[Sets]
ADD CONSTRAINT [PK_Sets]
    PRIMARY KEY CLUSTERED ([Code] ASC);
GO

-- Creating primary key on [Code] in table 'ManaSymbols'
ALTER TABLE [dbo].[ManaSymbols]
ADD CONSTRAINT [PK_ManaSymbols]
    PRIMARY KEY CLUSTERED ([Code] ASC);
GO

-- Creating primary key on [Id] in table 'ManaSymbolSets'
ALTER TABLE [dbo].[ManaSymbolSets]
ADD CONSTRAINT [PK_ManaSymbolSets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CardId] in table 'CardVariations'
ALTER TABLE [dbo].[CardVariations]
ADD CONSTRAINT [FK_CardCardVariation]
    FOREIGN KEY ([CardId])
    REFERENCES [dbo].[Cards]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CardCardVariation'
CREATE INDEX [IX_FK_CardCardVariation]
ON [dbo].[CardVariations]
    ([CardId]);
GO

-- Creating foreign key on [SetCode] in table 'CardVariations'
ALTER TABLE [dbo].[CardVariations]
ADD CONSTRAINT [FK_SetCardVariation]
    FOREIGN KEY ([SetCode])
    REFERENCES [dbo].[Sets]
        ([Code])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SetCardVariation'
CREATE INDEX [IX_FK_SetCardVariation]
ON [dbo].[CardVariations]
    ([SetCode]);
GO

-- Creating foreign key on [ManaSymbolSetId] in table 'Sets'
ALTER TABLE [dbo].[Sets]
ADD CONSTRAINT [FK_ManaSymbolSetSet]
    FOREIGN KEY ([ManaSymbolSetId])
    REFERENCES [dbo].[ManaSymbolSets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ManaSymbolSetSet'
CREATE INDEX [IX_FK_ManaSymbolSetSet]
ON [dbo].[Sets]
    ([ManaSymbolSetId]);
GO

-- Creating foreign key on [ManaSymbolSetId] in table 'ManaSymbols'
ALTER TABLE [dbo].[ManaSymbols]
ADD CONSTRAINT [FK_ManaSymbolSetManaSymbol]
    FOREIGN KEY ([ManaSymbolSetId])
    REFERENCES [dbo].[ManaSymbolSets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ManaSymbolSetManaSymbol'
CREATE INDEX [IX_FK_ManaSymbolSetManaSymbol]
ON [dbo].[ManaSymbols]
    ([ManaSymbolSetId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------