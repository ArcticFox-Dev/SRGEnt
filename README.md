![SRGEnt-Logo](/Docs/Images/SRGENT_Logo.svg)

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

Most of them either require a lot of boilerplate, are complicated to use, debug, or their code is hard to read. Others use tools that are hard to configure/use or are not easy to adapt to cusom workflows.

When I decided to write SRGEnt I had two main objectives in mind.

- Create a no fuss library that will be as simple to use as importing a NuGet or Unity package, and will will be able to work with custom build pipelines that a lot of bigger projects have.
- Create a simple api which would give you the benefits of an ECS architecture but would be somewhat familiar to a developer who wants to learn ECS but is coming from an OOP world.

If either of those are on your mind when choosing libraries to work with give SRGEnt a go and let me know what you think in the discord channel.

# Getting Started

## Installation
---
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

### DotNet Projects

**Comming Soon**

##  Code

Any self respecting ECS library will let you define your components and systems, some of them will ask you to define entities as well.
SRGEnt is slightly different, in order to work with the library you will have to define your Entities, Domains and of course write your Systems.

**Entities**

Think of entities as containers for components. In SRGEnt entity is a structure that holds some data necessary for the encompassing domain to work.

It also provides the user with an API to interact with it's components.

To define an entity along with all of it's potential components you will need to create an interface and decorate it with a EntityDefinition Attribute.

Like so:

``` C#
[EntityDefinition]
public interface IMyFirstEntity
{
    string Name {get;}
    float Speed {get;}
    float DistanceTraveled {get;}
    bool Grounded {get;}
}
```

**Domains**

Once you have your entity defined you will need to define a domain.

Domains ar managers of entities if you like. In some ECS frameworks they are called worlds or contexts. I went with a domain as for me world is a concept that can be confusing when working with physics engines and contexts are already used in a lot of other patterns.

As mentioned before in order to work with your entities you will need a domain that will manage them. 

To define one you will need another interface this time decorated with a DomainDefinition Attribute, the attributes argument has to be the type of the entity you want this domain to manage.

Like so:

``` C#
[DomainDefinition(typeof(IMyEntity))]
public interface IMyFirstDomain
{}
```

**Systems**

If everything works correctly roslyn should find those interfaces and write a lot of code behind the scenes so we can start writing a system that would operate on the data.

The generated code will include the Entity, Domain and some abstract system base classes that in our case would be called:
``` C#
public class MyFirstEntity
{}

public class MyFirstDomain
{}

public abstract class MyFirstExecuteSystem
{}

public abstract class MyFirstReactiveSystem
{}
```

An example ExecuteSystem (A simple system that operates on all entities that match it's selection criteria) would look something like the one below.

``` C#
public class MoveForwardSystem : MyFirstExecuteSystem
{
    public MoveForwardSystem(MyFirstDomain domain) : base(domain)
    {}

    protected override void SetMatcher(ref MyFirstMatcher matcher)
    {}

    protected override void Execute(ReadOnlySpan<MyFirstEntity> entities)
    {}
}
```

In the SetMatcher method we are deciding what types of entities we would want our execute method to receive.

An example of that could look something like that:
``` C#
    protected override void SetMatcher(ref MyFirstMatcher matcher)
    {
        matcher.Requires
        .Speed()
        .CannotHave
        .Grounded();
    }
```

This will give us all entities that have the component Speed defined that at the same time do not have the flag Grounded set to true.

After that we would move on to implementing our systems logic in the Execute method.

An example of which could look like this:
``` C#
    protected override void Execute(ReadOnlySpan<MyFirstEntity> entities)
    {
        foreach(var entity in entities)
        {
            if(entity.HasDistanceTraveled)
            {
                // If the entity already has distance traveled
                // we can use the += to increment it.
                entity.DistanceTraveled += entity.Speed;
            }
            else
            {
                // Because distance traveled is not yet defined for
                // this entity, we cannot use the += increment 
                // operator and have to set the initial value first.
                entity.DistanceTraveled = entity.Speed;
            }
        }
    }
```

And that's about it for the basics of custom code.

Your Systems code should look something like this now:
``` C#
using UnityEngine;
using SRGEnt.Generated;

public class MoveForwardSystem : MyFirstExecuteSystem
{
    public MoveForwardSystem(MyFirstDomain domain) : base(domain)
    {}

    protected override void SetMatcher(ref MyFirstMatcher matcher)
    {
        matcher.Requires
        .Speed()
        .CannotHave
        .Grounded();
    }

    protected override void Execute(ReadOnlySpan<MyFirstEntity> entities)
    {
        foreach(var entity in entities)
        {
            if(entity.HasDistanceTraveled)
            {
                // If the entity already has distance traveled
                // we can use the += to increment it.
                entity.DistanceTraveled += entity.Speed;
            }
            else
            {
                // Because distance traveled is not yet defined for
                // this entity, we cannot use the += increment 
                // operator and have to set the initial value first.
                entity.DistanceTraveled = entity.Speed;
            }
            Debug.Log($"(Entity - {entity.UId} moved {entity.DistanceTraveled} so far.");
        }
    }
}
```

**Using in Unity**

Now that all of those elements are ready you will need to get it to work in unity.

The easiest way to do that is to create a ECSBootstrap mono behaviour script and set things up in there.

Here is an example of how it could look.
``` C#
using UnityEngine;
using SRGEnt.Core;
using SRGEnt.Generated;

public class ECSBootstrap : MonoBehaviour
{
    [Range(5,10)]
    [SerializeField] int _numberOfEntities = 5;
    private MyFirstDomain _domain;
    private MoveForwardSystem _moveForwardSystem;
    
    private void Start()
    {
        _domain = new MyFirstDomain(_numberOfEntities);
        for(var i = 0; i < _numberOfEntities; i++)
        {
            var entity = _domain.CreateEntity();
            entity.Speed = i + 1;
        }
        _moveForwardSystem = new MoveForwardSystem(_domain);
    }

    private void Update()
    {
        _moveForwardSystem.Execute();
    }
}

```
After you'll attach this mono behaviour to a scene in your project and hit play you should see a stream of debug log messages flowing down in your console.
If you do then you are all set and ready to work with the library if not check below for some common problems.

**How to inspect entities**

Entities are internally storred as Struct to help minimize garbage allocations when copying data around, unfortunately that means that it's not easy to inspect details of entities in Unity inspector as it doesn't play nice with structures.

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

Because of that I would advise to use Reactive systems only for specific occasions where there will definitely be no collision and prefferably when their matcher is fairly unique.
Otherwise you might experience behaviours that will be hard to debug.

I will try to provide some examples of when using reactive systems can be a good idea in the future but for now you will have to figure it out yourself.

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
