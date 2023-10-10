# Mono Game Example

## What's in this example
This is a simple example on how you could utilise SRGEnt ECS to manage a set of
entities update their properties and render their representation to the screen.

The example is pretty basic as it creates an Entity and it's managing Domain in the
```SRGEnt/Systems/ECS_Definitions.cs``` file. 
Those then drive the generator to create the actual domain class with it's entity
type (Domains create their own entity type so that more than one Domain can
use the same EntityDefinition).

Then there are 2 Systems (Execute system type as the reactive system
still needs a bit of work to be 100% reliable) a RotatorSystem (which increments
the rotation of the entity based on time passed) and the Renderer system which
draws all the entities to the screen in a single sprite batch.

Lastly the ```SpinnerExampleGame.cs``` shows how to tie all of those together
by creating an instance of a domain, then creating a bunch of entities and
initializing them with initial data, and instantiating both systems and calling
them to process the entities. With the Rotator being called in the Update and
Rendered in the Draw phases.

## How to build
Once you have the repository checked out you can build the example in few ways.

### Using IDE

#### Rider or Visual Studio
If you are using Rider or Visual Studio building the example should be as easy as opening the .csproj
and running build and run inside of the IDE as both of the above should resolve all nuget dependencies.

#### Visual Studio Code
For visual studio code I would still recommend using the DotNet CLI approach (you can just run it from
the VSCode terminal). There are too many different plugins in VSCode for me to try and troubleshoot all
combos and terminal build will work 100%

#### DotNet CLI
After checking out the repository open the MonoGame example folder in the terminal of your choice.
Once there run.
``` shell
dotnet restore
dotnet build
dotnet run
```
That combination should first pull all the NuGet dependencies for the project.

Then build it (hopefully without any errors).

And once the build is done the last command should launch the "Game".
