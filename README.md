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
- Import package through unity Import Custom Asset option in assets.

    *Assets/Import Package/Custom Package*
- You are good to go.

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

**Troubleshooting**

Coming Soon
