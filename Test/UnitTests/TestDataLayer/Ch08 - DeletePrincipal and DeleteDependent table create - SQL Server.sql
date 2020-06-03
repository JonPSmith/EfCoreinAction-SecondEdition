
CREATE TABLE [DeletePrincipals] (
    [DeletePrincipalId] int NOT NULL IDENTITY,
    CONSTRAINT [PK_DeletePrincipals] PRIMARY KEY ([DeletePrincipalId])
);

CREATE TABLE [DeleteDependentCascade] (
    [Id] int NOT NULL IDENTITY,
    [DeletePrincipalId] int NULL,
    CONSTRAINT [PK_DeleteDependentCascade] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeleteDependentCascade_DeletePrincipals_DeletePrincipalId] FOREIGN KEY ([DeletePrincipalId]) REFERENCES [DeletePrincipals] ([DeletePrincipalId]) ON DELETE CASCADE
);

CREATE TABLE [DeleteDependentDefault] (
    [Id] int NOT NULL IDENTITY,
    [DeletePrincipalId] int NULL,
    CONSTRAINT [PK_DeleteDependentDefault] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeleteDependentDefault_DeletePrincipals_DeletePrincipalId] FOREIGN KEY ([DeletePrincipalId]) REFERENCES [DeletePrincipals] ([DeletePrincipalId]) ON DELETE NO ACTION
);

CREATE TABLE [DeleteDependentRestrict] (
    [Id] int NOT NULL IDENTITY,
    [DeletePrincipalId] int NULL,
    CONSTRAINT [PK_DeleteDependentRestrict] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeleteDependentRestrict_DeletePrincipals_DeletePrincipalId] FOREIGN KEY ([DeletePrincipalId]) REFERENCES [DeletePrincipals] ([DeletePrincipalId]) ON DELETE NO ACTION
);

CREATE TABLE [DeleteDependentSetNull] (
    [Id] int NOT NULL IDENTITY,
    [DeletePrincipalId] int NULL,
    CONSTRAINT [PK_DeleteDependentSetNull] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeleteDependentSetNull_DeletePrincipals_DeletePrincipalId] FOREIGN KEY ([DeletePrincipalId]) REFERENCES [DeletePrincipals] ([DeletePrincipalId]) ON DELETE SET NULL
);