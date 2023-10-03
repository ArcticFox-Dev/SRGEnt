<p align="center">
    <img src="https://raw.githubusercontent.com/ArcticFox-Dev/SRGEnt/develop/Docs/Images/SRGENT_Logo.svg" alt="SRGEnt" title="SRGEnt"/>
</p>

<p align="center">
    <a href="https://discord.gg/aMUBu6t5">
        <img src="https://img.shields.io/discord/922877870311886848.svg?logo=discord&logoColor=FFFFFF&label=Discord&labelColor=6A7EC2&color=7389D8" alt="SRGEnt on Discord"></a>
</p>

# SRGEnt

SRGEnt (pronounced Sergeant) is a Simple, Roslyn Generator assisted, Entity Component Systems library for C#.

It is meant to be a no fuss, easy to use library, providing a simple (both to use and to learn) API for creating entities and systems.

# Overview

SRGEnt has been originally created as a library for Unity and was heavily inspired by another ECS framework called [Entitas](https://github.com/sschmid/Entitas-CSharp). 

SRGEnt is not as mature as Entitas or some of the other libraries out there and I would probably not recommend using it for production just yet.

That said it is easy to set up and the minimal amount of boilerplate to write makes it ideal library for creating prototypes or for use at GameJams.

## Why create another ECS Framework ?

There is indeed plenty other ECS frameworks/libraries for C#, but none of them were a 100% fit for me.

Most of them either require a lot of boilerplate, are complicated to use, debug, or their code is hard to read. Others use tools that are hard to configure/use or are not easy to adapt to custom workflows.

When I decided to write SRGEnt I had two main objectives in mind.

- Create a no fuss library that will be as simple to use as importing a NuGet or Unity package, and will be able to work with custom build pipelines that a lot of bigger projects have.
- Create a simple api which would give you the benefits of an ECS architecture but would be somewhat familiar to a developer who wants to learn ECS but is coming from an OOP world.

If either of those are on your mind when choosing libraries to work with give SRGEnt a go and let me know what you think in the discord channel.

# Getting Started

## Installation

### DotNet Projects

If you are working in a DotNet ecosystem that supports NuGet, importing SRGEnt should be as simple as adding any of your other packages.

If you are using an IDE and it has a NuGet dependency manager just use that and search for SRGEnt. You only need to import the top level package as it will pull in all of it's dependencies and it will do so with the versions with which it was built and tested.

If you're a console dotnet dev then your trusty ```dotnet add package SRGEnt``` will get you all ready to go.

### Unity

**Prerequisites:**

Limiting Factors
- SRGEnt uses roslyn generators and in order for it to work you will need to use a Unity version that supports them.
- On top of roslyn generators the library also uses C# Spans to send data around, and that further narrows down the versions of Unity that will be compatible with the library.

If you want a no fuss experience I'd sugest that you use any Unity from the 2021.2 release forward as it added official unity support for dotnet standard 2.1 and with it support for Spans.

You can technically use SRGEnt with Unity versions going back to 2020.3 but it requires some extra dll wizardry which is beyond the scope of my projects.
I will try and add extra packages with the support for the previous Unity versions in future releases if there will be enough interest (let me know on discord).
If you can't wait for the next release and need it working now, hit me up on discord and I will try to help you out with the setup as it depends on which Unity version the project is built on.

**Adding to your project**

***Manual Approach (Not recommended)***

- Download the package you need from the latest release on github.
    
    ![Github-Releases](/Docs/Images/Installation_Unity/Releases_On_Github.png)
    - Download the file with the .unitypackage extention from the release you need.

        ![Download-Package](/Docs/Images/Installation_Unity/Pick_Release.png)
- Import package through unity Import Custom Asset option in assets.

    *Assets/Import Package/Custom Package*
    
    ![Import-Custom-Assets-Unity](/Docs/Images/Installation_Unity/Importing_Custom_Assets.png)
    - After importing you should see a SRGEnt folder in your project.

        ![Project-Content](/Docs/Images/Installation_Unity/Project_Content_After_Import.png)

    
- If you do. You are good to go.

***Using UPM (Recommended)***

SRGEnt is now available via Open UPM making its installation almost as easy as downloading a native Unity package!!  
I strongly recommend using it to add it to your project. Instructions can be found on [**this**](https://openupm.com/packages/net.srgent.generator/) page.

##  Your first SRGEnt Code

To test the library out I would suggest creating a simple Console app and pull in the SRGEnt dependency into it.

After that we can get down to coding.

Any self respecting ECS library will let you define your components and systems, some of them will ask you to define entities as well.
SRGEnt is slightly different, in order to work with the library you will have to define your Entities, Domains and of course write your Systems.

**Entities**

Think of entities as containers for components. In SRGEnt entity is a structure that holds some data necessary for the encompassing domain to work.

It also provides the user with an API to interact with it's components.

To define an entity along with all of it's potential components you will need to create an interface and decorate it with a EntityDefinition Attribute.

Like so:

``` C#
[EntityDefinition]
public interface IPersonEntity
{
    string FirstName { get; }
    string LastName { get; }
    string Nickname { get; }
    bool IsFriend { get; }
}
```

**Domains**

Once you have your entity defined you will need to define a domain.

Domains ar managers of entities if you like. In some ECS frameworks they are called worlds or contexts. I went with a domain as for me world is a concept that can be confusing when working with physics engines and contexts are already used in a lot of other patterns.

As mentioned before in order to work with your entities you will need a domain that will manage them. 

To define one you will need another interface this time decorated with a DomainDefinition Attribute, the attributes argument has to be the type of the entity you want this domain to manage.

Like so:

``` C#
[DomainDefinition(typeof(IPersonEntity))]
public interface IPeopleDomain
{}
```

**Systems**

If everything works correctly roslyn should find those interfaces and write a lot of code behind the scenes so we can start writing a system that would operate on the data.

The generated code will include the Entity, Domain and some abstract system base classes that in our case would be called:
``` C#
public class PersonEntity
{}

public class PeopleDomain
{}

public abstract class PeopleExecuteSystem
{}

public abstract class PeopleReactiveSystem
{}
```

An example ExecuteSystem (A simple system that operates on all entities that match it's selection criteria) would look something like the one below.

``` C#
using SRGEnt.Generated;

public class FormalGreeter : PeopleDomainExecuteSystem
{
    public FormalGreeter(PeopleDomain domain, bool shouldSort = false) : base(domain, shouldSort)
    { }

    protected override void SetMatcher(ref PersonMatcher matcher)
    { }

    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    { }
}

public class CasualGreeter : PeopleDomainExecuteSystem
{
    public CasualGreeter(PeopleDomain domain, bool shouldSort = false) : base(domain, shouldSort)
    { }

    protected override void SetMatcher(ref PersonMatcher matcher)
    { }

    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    { }
}
```

In the SetMatcher method we are deciding what types of entities we would want our execute method to receive.

An example of that could look something like that:
``` C#
    // For FormalGreeter
    protected override void SetMatcher(ref PersonMatcher matcher)
    {
        matcher.Requires
            .FirstName()
            .LastName()
            .CannotHave
            .IsFriend();
    }
    
    // For CasualGreeter
    protected override void SetMatcher(ref PersonMatcher matcher)
    {
        matcher.Requires
            .IsFriend()
            .ShouldHaveAtLeastOneOf
            .FirstName()
            .Nickname();
    }
```

This will give us all entities that have the component Speed defined that at the same time do not have the flag Grounded set to true.

After that we would move on to implementing our systems logic in the Execute method.

An example of which could look like this:
``` C#
    // For FormalGreeter
    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    {
        foreach (var person in entities)
        {
            Console.WriteLine($"Hello {person.FirstName} {person.LastName}");
        }
    }
    
    //For CasualGreeter
    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    {
        foreach (var friend in entities)
        {
            Console.WriteLine($"Hey {(friend.HasNickname ? friend.Nickname : friend.FirstName)}");
        }
    }
```

And that's about it for the basics of custom code.

Your Systems code should look something like this now:
``` C#
using SRGEnt.Generated;

public class FormalGreeter : PeopleDomainExecuteSystem
{
    public FormalGreeter(PeopleDomain domain, bool shouldSort = false) : base(domain, shouldSort)
    { }

    protected override void SetMatcher(ref PersonMatcher matcher)
    {
        matcher.Requires
            .FirstName()
            .LastName()
            .CannotHave
            .IsFriend();
    }

    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    {
        foreach (var person in entities)
        {
            Console.WriteLine($"Hello {person.FirstName} {person.LastName}");
        }
    }
}

public class CasualGreeter : PeopleDomainExecuteSystem
{
    public CasualGreeter(PeopleDomain domain, bool shouldSort = false) : base(domain, shouldSort)
    { }

    protected override void SetMatcher(ref PersonMatcher matcher)
    {
        matcher.Requires
            .IsFriend()
            .ShouldHaveAtLeastOneOf
            .FirstName()
            .Nickname();
    }
    
    protected override void Execute(ReadOnlySpan<PersonEntity> entities)
    {
        foreach (var friend in entities)
        {
            Console.WriteLine($"Hey {(friend.HasNickname ? friend.Nickname : friend.FirstName)}");
        }
    }
}
```

**Making it Work**

Now that you have all of your entities and systems ready we can modify
our little console app to be a fully blown ECS Hello world example
we deserve.

First we need to get our domain and systems set up:

``` C#
using SRGEnt.Generated;

var peopleDomain = new PeopleDomain(5);
var formalGreeter = new FormalGreeter(peopleDomain);
var casualGreeter = new CasualGreeter(peopleDomain);
```

After that we need to create some entities so that our systems have something to work on.

``` C#
void CreatePeople(PeopleDomain domain, int count)
{
    for (var i = 0; i < count; i++)
    {
        var person = domain.CreateEntity();
        person.FirstName = "Bob";
        person.LastName = "NotAFriend";
    }
}

void CreateFriends(PeopleDomain domain, int count)
{
    for (var i = 0; i < count; i++)
    {
        var friend = domain.CreateEntity();
        friend.IsFriend = true;
        friend.Nickname = $"Best Bud Nr.{i + 1}";
    }
}

CreatePeople(peopleDomain, 3);
CreateFriends(peopleDomain, 2);
```

And finally we can execute our systems to see the glorious ECS at work.

``` C#
formalGreeter.Execute();
casualGreeter.Execute();
```

After all of this your Program.cs should look somewhat like the one below 
(I reorganised some code so it is not 100% copy paste of the above) 

``` C#
// See https://aka.ms/new-console-template for more information

using SRGEnt.Generated;

var peopleDomain = new PeopleDomain(5);
var formalGreeter = new FormalGreeter(peopleDomain);
var casualGreeter = new CasualGreeter(peopleDomain);

CreatePeople(peopleDomain, 3);
CreateFriends(peopleDomain, 2);

formalGreeter.Execute();
casualGreeter.Execute();

// Method Definitions

void CreatePeople(PeopleDomain domain, int count)
{
    for (var i = 0; i < count; i++)
    {
        var person = domain.CreateEntity();
        person.FirstName = "Bob";
        person.LastName = $"Not A Friend Nr.{i + 1}";
    }
}

void CreateFriends(PeopleDomain domain, int count)
{
    for (var i = 0; i < count; i++)
    {
        var friend = domain.CreateEntity();
        friend.IsFriend = true;
        friend.Nickname = $"Best Bud Nr.{i + 1}";
    }
}
```

**If Using SRGEnt in Unity**

If you are using unity all of the steps will be the same
but you will need to modify your systems to use 
``` C#
Debug.Log($"GREETING");
```
instead of the
``` C#
Console.WriteLine($"GREETING");
```
and instead of the console app you will need to create
a MonoBehaviour that will set things up and run them for you.

Here is an example of how it could look.
``` C#
using UnityEngine;
using SRGEnt.Core;
using SRGEnt.Generated;

public class ECSBootstrap : MonoBehaviour
{
    [Range(5,10)]
    [SerializeField] int _numberOfEntities = 5;
    private PeopleDomain _domain;
    private FormalGreeter _formalGreeter;
    private CasualGreeter _casualGreeter;
    
    private void Start()
    {
        _domain = new PeopleDomain(_numberOfEntities);
        _formalGreeter = new FormalGreeter(_domain);
        _casualGreeter = new CasualGreeter(_domain);
        
        CreatePeople(3);
        CreateFriends(2);
        
        _formalGreeter.Execute();
        _casualGreeter.Execute();
    }
    
    private void CreatePeople(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var person = _domain.CreateEntity();
            person.FirstName = "Bob";
            person.LastName = $"Not A Friend Nr.{i + 1}";
        }
    }
    
    private void CreateFriends(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var friend = _domain.CreateEntity();
            friend.IsFriend = true;
            friend.Nickname = $"Best Bud Nr.{i + 1}";
        }
    }
}

```
After you'll attach this mono behaviour to a scene in your project and hit play you should see a nice set of debug log messages greeting everyone.
If you do then you are all set and ready to work with the library if not check below for some common problems.

**How to inspect entities in Unity**

Entities are internally stored as Struct to help minimize garbage allocations when copying data around, unfortunately that means that it's not easy to inspect details of entities in Unity inspector as it doesn't play nice with structures.

To help with viewing the state of your domains a custom inspector will be generated for every domains that you will define.

It can be found in the unity toolbar:

![Entity-Inspector-Location](/Docs/Images/Entity_Inspector_Unity/Where_To_Find_Inspector.png)

Once opened during runtime (It needs the data to be in memory) it will look somewhat like the one below.

![Entity-Inspector-Example](/Docs/Images/Entity_Inspector_Unity/Inspector_View.png)

```
The inspector is still in it's early alpha stage and is very slow and buggy (Especially if there are hundreds of entities).

It doesn't refresh the list of entities after it has been opened so new entities will not show until reopening.
Editing values can also be tricky as the editor will reset focus every second.
That said it should help with visualising the state of the domain outside of debugging in the IDE.

Those issues are quite high on the list of priorities for me to fix and I will update the documentation once the work is done.
```

**Troubleshooting**

Coming Soon

# Closer Look

## SRGEnt Core
---

SRGEnt Core provides the base types, attributes and interfaces that are used both in runtime code and by the code generator.

If you are looking into using SRGEnt you will be mostly interested in the IEntityDomain as it's the first thing you need to instantiate and it provides you with a way of creating and destroying entities.

Some other areas of interest might be the two base system types ExecuteSystem and ReactiveSystem both of which will be used under the hood in the system base classes that will be automatically generated for your domains. It is worth noting that at the moment every time either of those systems runs they call Domain.CleanupEntities for you in the background.
That call will trigger removal of all entities that were marked for destruction and rearangement (if necessary) of all the other entities so that they form a dense array.
If you don't want that behaviour due to it's sometimes high computational cost you can implement your own systems as long as they implement the ISystem interface.

There are few more classes that probably should be of intereste and those are Groups and Matchers.
Groups are wrapper classes that the Domains use under the hood to provide you access to the entities that you need for your systems.
Matchers are classes that are generated for you so that you can define the constraints for those entities.

### Systems
---
- **ExecuteSystem** is the bread and butter of the ECS world. They execute their code on all the entities that match their matcher criteria.
- **ReactiveSystem** is a bit more complicated as it will execute its code only on entities that have changed since the last time entities the matching group have been requested.

**Reactive Systems Example Problem**

If there are two Reactive system both of which share a group (their matcher criteria are the same)
``` C#

public class MoveDivisibleByTwo : GameReactiveSystem
{
    //....//
    protected override void SetMatcher(ref GameMatcher matcher)
    {
        matcher.Requires
        .Position();
    }
    //....//
}

public class MoveDivisibleByThree : GameReactiveSystem
{
    //....//
    protected override void SetMatcher(ref GameMatcher matcher)
    {
        matcher.Requires
        .Position();
    }
    //....//
}

```

If both of the systems are executed directly one after another
``` C#
public void Update()
{
    _moveDivisibleByTwo.Execute();
    _moveDivisibleByThree.Execute();
}
```
The MoveDivisibleByTwo system will get all the entities that have changed since the last Update loop.
But if all of them had a position of 9 (divisible by 3 but not by 2)
The changed flag would be removed after the system finishes processing and the MoveDivisibleByThree system would not receive any entities to operate on even though there are definitely entities that have position divisible by three.

Because of that I would advise to use Reactive systems only for specific occasions where there will definitely be no collision and preferably when their matcher is fairly unique.
Otherwise you might experience behaviours that will be hard to debug.

I will try to provide some examples of when using reactive systems can be a good idea in the future but for now you will have to figure it out yourself or reach out and I'll be happy to share my experience.

**One additional note to remember regarding ReactiveSystems**

At the moment the system will get entities that had any changes happen to them since the last group entity request. That means that even if component that are not covered by the systems matcher have been added/removed/modified the entities will end up in the execution list.

### Matchers
---
Matchers allow you to define the constraints on the entities you would want to receive in your systems.
They provide three separate categories of constraint:
- **Requires** constraint
    
    Any component under that constraint must be present (have a set value) on the entity for it to be passed to the execute method.
- **ShouldHaveAtLeastOneOf** constraint

    The entity will be passed to the execute method if the Entity has at least one of the component listed under this constraint type.
- **CannotHave** constraint

    The entity cannot have any of the components listed under this constraint to be passed into the execute method.

Bellow is an example of using a matcher to define a set of constraints for entities.
``` C#
// Entity definition
[EntityDefinition]
public interface ICharacterEntity
{
    bool Alive {get;}
    bool Undead {get;}
    bool TrullyDead {get;}
    bool InfectedByUndeadRot {get;}
    int Health {get;}
    Vector3 Position {get;}
    Vector3 Heading {get;}
    float Speed {get;}
}

public class MoveCharacters : CharacterExecuteSystem
{
   public MoveCharacters(MyFirstDomain domain) : base(domain)
    {}

    protected override void SetMatcher(ref CharacterMatcher matcher)
    {
        matcher.Requires
        .Position()
        .Heading()
        .Speed()
        .ShouldHaveAtLeastOneOf
        .Alive()
        .Undead()
        .CannotHave
        .TrullyDead();
    }

    protected override void Execute(ReadOnlySpan<CharacterEntity> entities)
    {
        foreach(var entity in entities)
        {
            entity.Position += entityt.Heading * entity.Speed;
        }
    }
}
```

Matchers are generated as part of the generator processing so the base class present in the Core part of the library can help in understanding what is happening under the hood but will not have the API as presented in the example above.

## SRGEnt Generator
---

SRGEnt Generator is the part that does all the heavy lifting behind the scenes but luckily for an average user it can remain a black box.

Below is a short list of the things that the generator will build for you when you will define your domain and it's entity interface definitions.

- Concrete Domain
- Concrete Entity
- Domain Matcher
- Domain AspectSetter (helper class used under the hood to deliver clean fluent API)
- An editor window that allows inspecting all entities (Alpha state and really slow and buggy)
- Base classes for execute and reactive systems for your domains.
- Bunch of interfaces you can use if you would want to create more generic systems that can work across domains as long as the entities have the right interfaces implemented.

## Changelog

### 0.5.5
- Split the Unity specific generation into its own generator so that it can be easier removed if not needed.
### 0.5.4
- Switched groups to not sort entities by default.
- Added a flag to systems constructor to enable sorting if needed.
### 0.5.3
- Removed '-', ',' and '.' from generated assembly names as it was causing problems in Unity