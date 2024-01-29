# Spatial C# API Reference

Documentation is generated using [docfx](https://github.com/dotnet/docfx).

## Usage

Install docfx based on their [installation instructions](https://dotnet.github.io/docfx/index.html). We set `allowCompilationErrors` to true so you will see a lot of warnings in your terminal, that's ok!

To build docs locally, run the following command from the root of the repository:

```bash
docfx Documentation/docfx.json --serve
```

### Pitfalls

When running locally, you may want to open Chrome DevTools and disable cache. This will ensure that you see the latest changes when you refresh the page. Docfx caches aggressively and does not always invalidate properly.

## Template

We fork the docfx modern template. The only change is in `templates/modern/layout/_master.tmpl`, where we hard-code the logo in the navbar as an SVG in order to make it theme-aware.