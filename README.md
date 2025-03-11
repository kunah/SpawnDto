# SpawnDto
.NET tool for automatic generation of DTOs


# Install
## Package
For annotations.
```bash
dotnet add package SpawnDto.Core
```

## Tool
For generating DTOs.
```bash
dotnet tool install SpawnDto --create-manifest-if-needed
```

# Run

```bash
dotnet spawndto -a [assembly] --dto [dto-output-path] -n [dto-namespace] --convertorOutputPath [convertor-path] --convertorNamespace [convertor-namespace]
```

## Post build event
Add this to you .csproj file.
```csharp
<Target Name="Generate DTOs" AfterTargets="PostBuildEvent">
    <Exec Command="dotnet spawndto -a $(OutDir)$(TargetName).dll  --dto [dto-output-path] -n [dto-namespace] --convertorOutputPath [convertor-path] --convertorNamespace [convertor-namespace]" ContinueOnError="false" />
</Target>
```

This will generate DTOs every time after the project is build.
