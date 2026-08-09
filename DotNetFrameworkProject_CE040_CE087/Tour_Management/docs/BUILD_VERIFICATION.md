# Build Verification

Build execution is performed outside this transformation phase by the platform pipeline. The generated SDK-style project targets `net8.0`, enables nullable reference types, removes System.Web references from active compilation, and uses EF Core 8.0.0 package references.
