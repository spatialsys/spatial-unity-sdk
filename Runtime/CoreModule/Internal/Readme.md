# Spatial Unity SDK Core Module Internals

This namespace contains types that are used internally but are not supported to be directly referenced by external developers.

The `InternalType` attribute is used to mark internal types that cannot be moved or renamed easily without breaking backwards compatibility.

Referencing any internal types directly will cause package validation to fail.
