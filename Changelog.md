# Changelog

### v2.0.0

V2 is complete rewrite of the system, instead of dynamically injecting stuff into MicroInject object, 
MicroInject generates container scripts containing all dependencies + any other components you want.

This allows for much easier access to dependencies becouse you no longer need to remember all the dependencies names.

The only feature that got removed is dynamic dependencies, now you need to manually assign dependency in container from your code, 
this feature got removed becouse only MicroInject containers are allowed to be injected into the system. (tho this is only artificial limitation, to not create big mess in the system)

### v1.1.0

- Added bucket system (now Micro Inject is treated like object)

### v1.0.6
- Fixed error during in-editor injection when no MicroInjectLoadDepenencies component exist
- Fixed error during in-editor injection when any scene is unloaded
- Now you can inject dependencies into private fields

### v1.0.5
- Internal improvement

### v1.0.4
- Changes in DependencyName and DynamicInject to allow setting name from inspector
- Fixed package throwing error when building project

### v1.0.3
- Changes in DependencyName to allow setting name from inspector

### v1.0.2
- Fixes for dynamic dependencies and named dependencies

### v1.0.1
- Added support for multiple scenes loaded

### v1.0.0
- Initial release