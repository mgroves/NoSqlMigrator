![NoSqlMigrator logo](https://user-images.githubusercontent.com/103484/253677010-ea65d4b0-1c7c-4aae-8cf7-970067ec02be.png)

# NoSqlMigrator

[![CI for NoSqlMigrator](https://github.com/mgroves/NoSqlMigrator/actions/workflows/ci.yml/badge.svg)](https://github.com/mgroves/NoSqlMigrator/actions/workflows/ci.yml)

NoSqlMigrator is a migration framework for .NET, similar to Ruby on Rails Migrations, but heavily inspired by [FluentMigrator](https://github.com/fluentmigrator/fluentmigrator) (though it is NOT a fork or extension to that project).
Migrations are a structured way to alter your database ~~schema~~ structure/organization and are an alternative to creating lots of scripts that have to be run manually by every developer involved. Migrations solve the problem of evolving a database for multiple instances of that databases (for example, the developer's local database, the test database and the production database). Database changes are described in classes written in C# that can be checked into a version control system.

This project is VERY new, but if you have suggestions or improvements (even small things!), they are all very welcome.

Right now this tool ONLY supports Couchbase

# Powered By

* JetBrains Rider
* Couchbase (.NET SDK)
* NUnit
* Oakton

# How to use it

If you've used FluentMigrator before, this should be very familiar.

1. Create a new C# class library project, add NoSqlMigrator from NuGet.

2. Create a series of classes (you will likely start with just one, and add them as you need to in future commits).

3. Compile that library into a DLL file.

4. Execute the migrations on that DLL file (you can use the NoSqlMigration.Runner, available in the Releases, or you can use the `MigrationRunner` class, and execute the runner from your own code. I'd recommend sticking to the CLI runner).

## Classes

Here is an example of a migration.

```
[Migration(1)]
public class Migration001_CreateInitialScopeAndCollection : Migrate
{
    public override void Up()
    {
        Create.Scope("myScope")
            .WithCollection("myCollection1");
    }

    public override void Down()
    {
        Delete.Scope("myScope");
    }
}

```

Each class should inherit from `Migrate`. Each class should have a `Migration(...)` attribute. Typically you would number these sequentially (e.g. 1 for your first migration, 2 for the one you create next week, etc). They don't HAVE to be exactly sequential--you could make the first one 100 and the second one 200 if you'd like.

Each class has an "up" and a "down".

"Up" is responsible for making a change. In the above example, it will create a new scope (and create a new collection within that scope).

"Down" is responsible for _reversing_ that change, which is useful if you need to return to previous versions or undo what you're working on. Note that not everything can be neatly "down"ed.

## Migration actions

The migrations within Up and Down use a fluent syntax: Create... Delete... Execute... Update... and so on.

There may be more than one way to do things: for instance, the above example creates a scope and puts a collection in it. You could also just create the scope, and separately run `Create.Collection("...")`. Or you could `Execute.Sql("...")` with the raw SQL++ to create the collection.

## Actions currently available

* Create.Scope
* Create.Collection
* Create.Index
* Delete.Scope
* Delete.Collection
* Delete.FromCollection
* Delete.Index
* Insert.IntoCollection
* Execute.Sql
* Update.Document
* Update.Collection

Each has various optional and required extensions. This is meant to be a fluent interface, so Intellisense will help a lot. You can also refer to the test migrations in the NoSqlMigrator.Tests project (until better documentation comes along).
